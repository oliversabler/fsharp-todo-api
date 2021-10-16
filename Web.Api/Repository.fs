module Repository

open Models

open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open Npgsql

open Microsoft.Extensions.Configuration
open System.Data

type Store() = 
    let config = (ConfigurationBuilder()).AddJsonFile("settings.json").Build()
    let conn = new NpgsqlConnection(config.["connectionString"]) :> IDbConnection

    member _.Create newTodo = 
        //data.TryAdd(todo.Id, todo)
        task {
            conn.Open()
            let todoTable = table<Todo>

            insert {
                into todoTable
                value newTodo
            } |> conn.InsertAsync 
              |> ignore

            conn.Close()
        }
    //member _.Update todo = data.TryUpdate(todo.Id, todo, data.[todo.Id])
    //member _.Delete id = data.TryRemove id
    //member _.Get id = data.[id]
    //member _.GetAll () = data.ToArray()