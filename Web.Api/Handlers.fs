module Handlers

open System
open Microsoft.AspNetCore.Http
open Giraffe
open Todos
open System.Collections.Generic

let getTaskHandler (id : Guid) = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let store = ctx.GetService<Store>()
            let todo = store.Get(id)
            return! json todo next ctx
        }

let getTasksHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let store = ctx.GetService<Store>()
            let todos = store.GetAll()
            return! json todos next ctx
        }

let createTaskHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! newTodo = ctx.BindJsonAsync<NewTodo>()
            let store = ctx.GetService<Store>()
            let created = store.Create({ 
                Id = Guid.NewGuid(); 
                Description = newTodo.Description; 
                Created = DateTime.UtcNow; 
                IsCompleted = false 
            })
            return! json created next ctx
        }

let updateTaskHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! todo = ctx.BindJsonAsync<Todo>()
            let store = ctx.GetService<Store>()
            let updated = store.Update(todo)
            return! json updated next ctx
        }

let deleteTaskHandler (id : Guid) = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let store = ctx.GetService<Store>()
            let existing = store.Get(id)
            let deleted = store.Delete(KeyValuePair<TodoId, Todo>(id, existing))
            return! json deleted next ctx
        }