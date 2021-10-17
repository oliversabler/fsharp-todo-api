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

    let taskToArray (t:Task<seq<'a>>) = t |> Async.AwaitTask |> Async.RunSynchronously |> Seq.toArray

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

    member _.Read id = 
        task {
            conn.Open()

            let result = 
                select {
                    for t in todoTable do
                    where (t.Id = id)
                } |> conn.SelectAsync<Todo>
                  |> taskToArray

            conn.Close()
                
            return result
        }

    member _.ReadAll () = 
        task {
            conn.Open()

            let result = 
                select {
                    for t in todoTable do
                    selectAll
                } |> conn.SelectAsync<Todo>
                    |> taskToArray

            conn.Close()
                
            return result
        }

    //member _.Update todo = data.TryUpdate(todo.Id, todo, data.[todo.Id])

    //member _.Delete id = data.TryRemove id

