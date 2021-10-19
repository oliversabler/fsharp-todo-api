module Models

open System

type NewTodo = {
    Description: string
}

[<CLIMutable>]
type Todo = {
    Id : Guid
    Description : string
    Created : DateTime
    IsCompleted : bool
}