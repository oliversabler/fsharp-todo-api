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

    member _.Create todo = 
        if conn.State = ConnectionState.Closed then conn.Open()

        insert {
            into todoTable
            value todo
        } |> conn.InsertAsync 
          |> ignore

        conn.Close()

    member _.Read id = 
        if conn.State = ConnectionState.Closed then conn.Open()

        let result = 
            select {
                for t in todoTable do
                where (t.Id = id)
            } |> conn.SelectAsync<Todo>
              |> taskToArray

        conn.Close()
                
        result

    member _.ReadAll () = 
        if conn.State = ConnectionState.Closed then conn.Open()

        let result = 
            select {
                for t in todoTable do
                selectAll
            } |> conn.SelectAsync<Todo>
              |> taskToArray

        conn.Close()
                
        result

    member _.Update todo =
        if conn.State = ConnectionState.Closed then conn.Open()

        let _ = 
            update {
                for t in todoTable do
                setColumn t.Description todo.Description
                setColumn t.IsCompleted todo.IsCompleted
                where (t.Id = todo.Id)
            } |> conn.UpdateAsync<Todo>
              |> Async.AwaitTask
              |> Async.RunSynchronously

        let result = 
            select {
                for t in todoTable do
                where (t.Id = todo.Id)
            } |> conn.SelectAsync<Todo>
              |> taskToArray

        conn.Close()
            
        result

    member _.Delete id =
        if conn.State = ConnectionState.Closed then conn.Open()

        let result = 
            delete {
                for t in todoTable do
                where (t.Id = id)
            } |> conn.DeleteAsync
              |> Async.AwaitTask
              |> Async.RunSynchronously

        conn.Close()
                
        result