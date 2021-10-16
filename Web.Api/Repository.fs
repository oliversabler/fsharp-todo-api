module Repository

open Models

open Dapper.FSharp
open Dapper.FSharp.PostgreSQL
open Npgsql

open Microsoft.Extensions.Configuration
open System.Data
open System.Threading.Tasks

type Store() = 
    let config = (ConfigurationBuilder()).AddJsonFile("settings.json").Build()
    let conn = new NpgsqlConnection(config.["connectionString"]) :> IDbConnection
    let todoTable = table<Todo>

    let taskToList (t:Task<seq<'a>>) = t |> Async.AwaitTask |> Async.RunSynchronously |> Seq.toArray

    member _.Create newTodo = 
        task {
            conn.Open()

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
    member _.GetAll () = 
        task {
            conn.Open()

            let result = 
                select {
                    for t in todoTable do
                    selectAll
                } |> conn.SelectAsync<Todo>
                  |> taskToList

            conn.Close()
            
            return result
        }