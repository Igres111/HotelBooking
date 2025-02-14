Hotel Booking API
Overview
This project is a Hotel Booking API built using ASP.NET Core with Entity Framework Core for database management. It allows users to book hotels, manage bookings, leave reviews, and handle user authentication.

Features
User Authentication (Register/Login)
Hotel Management (CRUD operations for hotels)
Booking System (Users can book hotels)
Review System (Users can leave reviews for hotels)
Admin Controls (Manage users, hotels, and bookings)
Technologies Used
Backend: ASP.NET Core Web API
Database: Entity Framework Core (SQL Server)
Authentication: JWT Token
Mapping: AutoMapper
Data Validation: FluentValidation

Project Structure:
/Controllers      → API Controllers (Hotel, Booking, Review, User)  
/DTOs            → Data Transfer Objects (Booking, Hotel, Review, User)  
/Data            → Database Context  
/Models          → Entity Models  
/Migrations      → EF Core Migrations  
/Profiles        → AutoMapper Configuration  
