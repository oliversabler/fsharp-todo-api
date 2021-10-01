# Giraffe Test API
## Description
Lightweight Todo API written in F# using [Giraffe](https://github.com/giraffe-fsharp/Giraffe)

## How to use
Create new todo:
POST `http://localhost:5000/api/todo/`
```
{
    "Description": "Some todo"
}
```

Get all todos:
GET `http://localhost:5000/api/todo/`

Get todo with guid:
GET `http://localhost:5000/api/todo/<guid>`

Update a todo:
PUT `http://localhost:5000/api/todo/`
```
{
    "Id": "<guid>",
    "Description": "Updated todo",
    "IsCompleted": true
}
```

Delete a todo:
DELETE `http://localhost:5000/api/todo/<guid>`
