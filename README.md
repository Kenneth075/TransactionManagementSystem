# TransactionManagementSystem

Banking Transaction System
A comprehensive banking backend system built with .NET 8, implementing CQRS pattern for handling banking operations including account management, transactions, and external payment integrations.

#Features
#Core Banking Operations:
Account creation and management.
View account details, balances, and transaction history.
Deposits, withdrawals, and account transfers.
Monthly transaction statements.
Atomic, consistent transactions with proper validation.

#Architecture & Patterns
CQRS Pattern implementation using MediatR
Clean Architecture with separated concerns
Domain-Driven Design principles
Entity Framework Core for data persistence
Repository Pattern

#Security & Authentication
JWT Authentication and Authorization
Role-Based Access Control (RBAC)
Password hashing with PBKDF2
HTTPS enforcement and security headers

#Performance & Scalability
Redis caching for frequently accessed data
Database connection pooling
Optimized queries with pagination
Concurrent transaction handling


#External Integrations
Paystack integration for online payments


#Monitoring & Logging
Structured logging with Serilog
Error handling and notifications

#Testing & Documentation
Unit tests with xUnit
API documentation with Swagger/OpenAPI
Comprehensive setup documentation

#Prerequisites
Development Environment
.NET 8 SDK
Postgres Server (LocalDB for development)
Redis (for caching)
Visual Studio 2022 or VS Code with C# extension
Git for version control

Installation & Setup
1. Clone the Repository
git clone https://github.com/Kenneth075/TransactionManagementSystem.git
cd banking-system

API Documentation
Authentication
All endpoints require JWT token authentication.

# Run specific test project
dotnet test tests/BankingSystem.Tests.Unit/
Sample Test Data
The system seeds with the following test accounts in development:

Admin User: admin@bankingsystem.com / Admin123!
Test Customer: kenneth.tibele@example.com / Test123!
Test Account: ACC1000000001 with NGN 1,000.00 balance

