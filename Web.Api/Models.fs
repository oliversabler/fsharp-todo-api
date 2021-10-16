module Models

open System

type TodoId = Guid

type NewTodo = {
    Description: string
}

type Todo = {
    Id : TodoId
    Description: string
    Created: DateTime
    IsCompleted: bool
}