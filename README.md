# Todo API with Giraffe
## Description
Lightweight Todo API written in F# using [Giraffe](https://github.com/giraffe-fsharp/Giraffe)

## Prerequisites
* Docker
* VS Code
    * REST Client

## How to use
Run `docker-compose up`

### REST Client
Navigate to rest-client-requests folder and use the requests in the file named `todo.http`

### Postman, etc.
#### Create a new task
POST `http://localhost:5000/api/todo/`
```
{
    "Description": "Some todo"
}
```

#### Get all tasks
GET `http://localhost:5000/api/todo/`

#### Get task with guid
GET `http://localhost:5000/api/todo/<guid>`

#### Update a task
PUT `http://localhost:5000/api/todo/`
```
{
    "Id": "<guid>",
    "Description": "Updated todo",
    "IsCompleted": true
}
```

#### Delete a task
DELETE `http://localhost:5000/api/todo/<guid>`
