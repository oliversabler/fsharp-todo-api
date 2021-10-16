create table "Todo" (
    "Id" uuid NOT NULL constraint todo_pk primary key,
    "Description" varchar(120) NOT NULL,
    "Created" date NOT NULL,
    "IsCompleted" boolean NOT NULL
);