
# Volunteer & Ride-Share Management System

Volunteer & Ride-Share Management System — a complete internal platform built in C# (.NET Core) using a clean three-tier architecture (DAL / BL / BO).  
Includes WPF and console clients, modular BL/DAL interfaces, automated data initialization for quick demos, and supporting test utilities.  
Designed and implemented end-to-end following SOLID principles — from requirements and architecture to implementation and testing.

---

## Overview

This project is an internal management platform designed to handle volunteers and ride-sharing operations in a structured and scalable manner.  
The system demonstrates a clean layered architecture with clear separation of concerns between business logic, data access, and domain objects.

The project was developed end-to-end, covering system design, architectural planning, implementation, and validation.

---

## Architecture

The system is implemented using a three-tier layered architecture:

- **BO (Business Objects)** – Domain entities and core data models  
- **BL (Business Logic)** – Application logic, validation, and business rules  
- **DAL (Data Access Layer)** – Data persistence and retrieval  

Key architectural principles:

- Clean separation between layers  
- Interface-based design between BL and DAL  
- High modularity and maintainability  
- SOLID principles applied throughout the system  

---

## Features

- Volunteer management  
- Ride-share and assignment handling  
- Modular business logic layer  
- Automated data initialization for fast demo and testing  
- Multiple clients:
  - WPF desktop client  
  - Console client  
- Supporting test and utility components  

---

## Technologies

- C# (.NET Core)  
- WPF  
- Layered / Three-Tier Architecture  
- Interfaces & Abstraction  
- SOLID principles  

---

## Project Structure

/BO
Domain entities and core models

/BL
Business logic, validations, and rules

/DAL
Data access implementations

/UI
WPF and Console clients

/Tests
Test utilities and validation helpers

This structure enforces strict separation between domain, logic, data access, and presentation layers.

---

## How to Run

1. Clone the repository  
2. Open the solution in Visual Studio  
3. Set the desired client project (WPF or Console) as Startup Project  
4. Run the application  

Automated data initialization will populate sample volunteers and vehicles for immediate testing and demonstration.

---

## Design Notes

This project emphasizes:

- Clean architecture and strict separation of concerns  
- Interface-driven development between system layers  
- Maintainable and extensible codebase  
- Full ownership of the development process — from design and architecture to implementation and validation  

The system was intentionally designed as a realistic internal management platform rather than a simple academic assignment.
