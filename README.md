# OrderSystem.Net

🛒 Order Management System
A simple web application built for managing customers, products, and orders in a small shop. This project was developed using ASP.NET Core MVC, Dapper.NET, and SQL Server, with a responsive frontend built using Bootstrap and jQuery.

📚 Features
✅ Customer Management
Create, read, update, and delete (CRUD) customers.

Fields: ID, Name, Email, Phone, Registration Date.

List view with search/filter by Name or Email.

📦 Product Management
Full CRUD operations for products.

Fields: ID, Name, Description, Price, Stock Quantity.

Product listing with search/filter by Name.

🧾 Order Management
Register new orders:

Select existing customer.

Add one or more products with desired quantity.

Validate available stock.

Automatically calculate total price.

Save order and order items.

Fields:

Order: ID, CustomerID, OrderDate, TotalAmount, Status (New, Processing, Completed).

Order Item: ID, OrderID, ProductID, Quantity, UnitPrice.

View all orders with filtering by Customer or Status.

View detailed order information including items.

Update order status.

🖥️ Tech Stack
Layer	Technology
Frontend	HTML5, CSS3, Bootstrap 5, jQuery
Backend	ASP.NET Core MVC (C#)
Database	SQL Server
ORM	Dapper.NET

🧱 Architecture
Presentation Layer – ASP.NET Core Views and Controllers

Business/Domain Layer – Services and Entities with business logic

Infrastructure/Data Layer – Repositories using Dapper.NET

Uses Repository Pattern for data access abstraction.

Applies SOLID principles for better maintainability.

Uses native Dependency Injection in ASP.NET Core.

Setup SQL Server Database
Run the provided script located at:

/Scripts/DatabaseSetup.sql
This script will:

Create the required tables (Customers, Products, Orders, OrderItems)

Configure the Connection String
In appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=OrderDb;Trusted_Connection=True;"
}

🛠️ Additional Notes
Clean, readable, and commented code (in English).

Basic error handling for invalid inputs and DB operations.

Dynamic frontend behavior using jQuery (e.g., adding products to orders without full page reloads).

Responsive UI built with Bootstrap.