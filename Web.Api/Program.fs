open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Repository
open System
open Microsoft.Extensions.Logging
open Dapper.FSharp

/////////////
// Web Api //
/////////////

let apiTodoRoutes : HttpHandler =
    subRoute "/todo/"
        (choose [
            GET >=> choose [
                routef "%O" Handlers.readTaskHandler
                route "" >=> Handlers.readTasksHandler
            ]
            POST >=> route "" >=> Handlers.createTaskHandler
            //PUT >=> route "" >=> Handlers.updateTaskHandler
            //DELETE >=> routef "%O" Handlers.deleteTaskHandler
        ])

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        GET >=> route "/"
        subRoute "/api"
            (choose [
                apiTodoRoutes
            ])
        setStatusCode 404 >=> text "Not Found"
    ]

///////////////////
// Error Handler //
///////////////////

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

///////////////////
// Configuration //
///////////////////

let configureApp (app : IApplicationBuilder) =
    app.UseGiraffeErrorHandler(errorHandler)
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe()
            .AddSingleton<Store>(Store()) |> ignore

let configureLogging (loggingBuilder : ILoggingBuilder) =
    loggingBuilder.AddFilter(fun lvl -> lvl >= LogLevel.Information)
                  .AddConsole()
                  .AddDebug() |> ignore

OptionTypes.register()

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHost ->
            webHost
                .Configure(configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
                |> ignore)
        .Build()
        .Run()
    0