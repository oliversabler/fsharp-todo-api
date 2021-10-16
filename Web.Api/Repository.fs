module Repository

open Models

open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open Npgsql

open System.Data
open System

type Store() = 
    //let data = ConcurrentDictionary<TodoId, Todo>()
    let connectionString = "Host=host.docker.internal;User ID=username;Password=password;Database=todo_database;Port=5432"
    let conn = new NpgsqlConnection(connectionString) :> IDbConnection

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