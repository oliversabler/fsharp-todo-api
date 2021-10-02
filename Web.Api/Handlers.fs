module Handlers

open System
open Microsoft.AspNetCore.Http
open Giraffe
open Todos
open System.Collections.Generic
open Microsoft.Extensions.Logging

let getTaskHandler (id : Guid) = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger()

        task {
            logger.Log(LogLevel.Information, $"Fetching task with guid: {id}")
            
            let store = ctx.GetService<Store>()
            let todo = store.Get(id)

            logger.Log(LogLevel.Information, todo.ToString())

            return! json todo next ctx
        }

let getTasksHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger()

        task {
            logger.Log(LogLevel.Information, $"Fetching all tasks")
            
            let store = ctx.GetService<Store>()
            let todos = store.GetAll()

            logger.Log(LogLevel.Information, todos.ToString())

            return! json todos next ctx
        }

let createTaskHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger()

        task {
            let! newTodo = ctx.BindJsonAsync<NewTodo>()
            let todo = { 
                Id = Guid.NewGuid(); 
                Description = newTodo.Description; 
                Created = DateTime.UtcNow; 
                IsCompleted = false 
            }

            logger.Log(LogLevel.Information, $"Creating task with guid: {todo.Id}")

            let store = ctx.GetService<Store>()
            store.Create(todo) |> ignore

            return! json todo next ctx
        }

let updateTaskHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger()

        task {
            let! todo = ctx.BindJsonAsync<Todo>()

            logger.Log(LogLevel.Information, $"Updating task with guid: {todo.Id}")

            let store = ctx.GetService<Store>()
            let updated = store.Update(todo)

            return! json updated next ctx
        }

let deleteTaskHandler (id : Guid) = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let logger = ctx.GetLogger()

        task {
            logger.Log(LogLevel.Information, $"Deleting task with guid: {id}")

            let store = ctx.GetService<Store>()
            let existing = store.Get(id)
            let deleted = store.Delete(KeyValuePair<TodoId, Todo>(id, existing))

            return! json deleted next ctx
        }