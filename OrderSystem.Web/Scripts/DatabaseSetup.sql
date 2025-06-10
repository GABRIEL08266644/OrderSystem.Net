IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clients')
BEGIN
    CREATE TABLE Clients (
        Id INT PRIMARY KEY IDENTITY,
        Name NVARCHAR(100),
        Email NVARCHAR(100),
        Phone NVARCHAR(20),
        CreatedAt DATETIME DEFAULT GETDATE(),
        RegistrationDate DATETIME NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY,
        Name NVARCHAR(100),
        Description NVARCHAR(255),
        Price DECIMAL(10,2),
        StockQuantity INT
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT PRIMARY KEY IDENTITY,
        ClientId INT FOREIGN KEY REFERENCES Clients(Id),
        OrderDate DATETIME DEFAULT GETDATE(),
        TotalAmount DECIMAL(10,2),
        Quantity INT,
        Status NVARCHAR(20)
    );
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id INT PRIMARY KEY IDENTITY,
        OrderId INT FOREIGN KEY REFERENCES Orders(Id),
        ProductId INT FOREIGN KEY REFERENCES Products(Id),
        Quantity INT,
        UnitPrice DECIMAL(10,2)
    );
END
