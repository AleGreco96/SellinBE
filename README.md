# SellinBE

**SellinBE** is the backend prototype for a mountain bike e-commerce platform.  
It is a **modular ASP.NET Core backend** designed to handle customers, products, authentication, real-time chat, and error management.  
The project leverages your custom libraries (**ErrorLib**, **SecurityLib**, **ChatLib**) to provide a clean, secure, and scalable architecture.

---

## Key Features

- **E-commerce Functionality**: CRUD operations for customers, orders, and products (based on AdventureWorksLT2019).
- **Authentication & Security**: JWT-based login, password hashing & encryption, role management using **SecurityLib**.
- **Error Handling & Logging**: Centralized exception and validation logging with **ErrorLib**.
- **Real-Time Chat**: SignalR chat hub integrated with **ChatLib**, supporting streaming AI responses.
- **Database Integration**: Configurable connection to a SQL Server production database.
- **CORS & Middleware**: Ready-to-use middlewares for CORS, HTTPS redirection, authorization, and custom services.
- **Extensible Architecture**: Easy to add new controllers, services, or integrate additional libraries.

---
