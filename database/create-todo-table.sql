CREATE TABLE Employees_Sample (
    Id uniqueidentifier DEFAULT NEWSEQUENTIALID,
    Description nvarchar(max) NOT NULL,
    Created datetime2 NOT NULL,
    IsCompleted int NOT NULL
);