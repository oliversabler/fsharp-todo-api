open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.ViewEngine
open FSharp.Control.Tasks
open Todos
open System
open System.Collections.Generic

type PingModel = {
    Response: string
}

module Handlers =
    let viewTaskHandler (id : Guid) = 
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let store = ctx.GetService<Store>()
                let todo = store.Get(id)
                return! json todo next ctx
            }

    let viewTasksHandler = 
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

let apiTodoRoutes : HttpHandler =
    subRoute "/todo/"
        (choose [
            GET >=> choose [
                routef "/%O" Handlers.viewTaskHandler
                route "" >=> Handlers.viewTasksHandler
            ]
            POST >=> route "" >=> Handlers.createTaskHandler
            PUT >=> route "" >=> Handlers.updateTaskHandler
            DELETE >=> routef "/%O" Handlers.deleteTaskHandler
        ])

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        GET >=> route "/"
        subRoute "/api"
            (choose [
                apiTodoRoutes
                GET >=> route "" >=> json { Response = "Todo List API" }
            ])
        //setStatusCode 404 >=> text "Not Found"
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe()
            .AddSingleton<Store>(Store()) |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHost ->
            webHost
                .Configure(configureApp)
                .ConfigureServices(configureServices)
                |> ignore)
        .Build()
        .Run()
    0