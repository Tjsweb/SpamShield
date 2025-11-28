## 🚨 SpamShield — Contact Search & Spam Detection Backend

SpamShield is a backend REST API built with .NET 8 and MSSQL, designed to replicate core functionalities of apps like Truecaller.
It enables global phone number search, contact aggregation, and spam detection with secure JWT-based authentication.

## 📌 Features
### 🔐 Authentication

- Register & login using phone number, name, email, and password

- JWT token-based authentication for all protected routes

### 📱 Contact Management

- Upload personal contacts (registered or non-registered)

- Contact data contributes to the global search graph

- Normalized data model for fast lookup

### 🔍 Search System

- Search by name (starts-with priority, then contains)

- Search by phone number

- Email visibility allowed only if:

- The searched number belongs to a registered user, and

- The requester is in their contact list

### 🚩 Spam Detection

- Users can mark any number as spam

- Spam-likelihood score shown in search results

- Works for both registered & unregistered numbers

## 📡 API Endpoints

| Method   | Endpoint         | Protected | Description                                                 |
| -------- | ---------------- | --------- | ----------------------------------------------------------- |
| **POST** | `/auth/register` | No        | Register a new user (name, phone, password, optional email) |
| **POST** | `/auth/login`    | No        | Login with phone + password and receive a JWT token         |
| **POST** | `/contacts/upload` | Yes       | Upload user’s personal contacts (registered & non-registered) |
| **GET** | `/search/phone/{number}` | Yes       | Search global database by phone number                        |
| **GET** | `/search/name/{name}`    | Yes       | Search global database by name (prefix-first, then substring) |
| **POST** | `/spam/report`         | Yes       | Mark any number as spam                      |
| **GET**  | `/spam/score/{number}` | Yes       | Get spam-likelihood score for a phone number |


## 🛠 Tech Stack

- ASP.NET Core 8 — Web API

- Entity Framework Core 8

- SQL Server (MSSQL)

- Swagger / OpenAPI

- JWT Authentication

## ⚙️ Local Setup Instructions

### 1. Install Required Software

- Visual Studio 2022

- .NET 8 SDK

- SQL Server / Azure Data Studio / SSMS

### 2. Create Database

- Create a database named SpamShieldDB:

- CREATE DATABASE SpamShieldDB;

- Update the connection string in appsettings.json (if needed):

```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SpamShieldDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```


### 3. Run EF Core Migrations

Open Package Manager Console:

```
add-migration "Initial"
update-database
```

### 4️. Run the Project

Click Run / Start Without Debugging in Visual Studio.

Swagger UI will open automatically at:

https://localhost:{port}/swagger/index.html