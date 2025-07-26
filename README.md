# EduLive – Real-Time Instructor-Led E-Learning Platform

EduLive is a comprehensive learning platform designed to facilitate **live instructor-led education**, enabling real-time class sessions, scheduling, interactive discussions, and automated certificate issuance. It bridges the gap between traditional classroom instruction and online flexibility.

---

## 🧠 Project Overview

EduLive empowers instructors and students to interact in real time, manage lessons, and participate in structured, synchronous educational sessions.

Unlike static, pre-recorded platforms, EduLive focuses on:
- **Live class delivery**
- **Calendar-based scheduling**
- **Real-time messaging**
- **Personalized certificate issuance**
- **Course and lesson management**

---

## 💡 Business Idea

EduLive solves a clear problem in online education — lack of real-time interaction. It enables:
- Instructors to host scheduled classes using Zoom or Google Meet
- Students to engage live and receive real-time updates and support
- Automated generation of PDF certificates upon course completion
- Admin control over users, courses, and content

---

## 🚀 Key Features

- **Live Class Scheduling**: Instructors schedule sessions via Google Calendar with Google Meet or Zoom links
- **Real-Time Chat**: SignalR-powered messaging between instructors and students during active sessions
- **User Roles**: Role-based access and dashboards for admins, instructors, and students
- **Course & Lesson Management**: Instructors can create courses, lessons, and assign content
- **Certificate Generation**: Students receive personalized PDF certificates using dynamic templates
- **Image Processing**: Uploaded lesson images or thumbnails are automatically converted to optimized `.webp` format with safe naming
- **Admin Panel**: Razor-based interface for user/course moderation and system control

---

## 🧱 Architecture & Project Structure

EduLive follows a layered, clean architecture approach for long-term maintainability and scalability:

### 🧩 Core Layer
- `Models`: Domain entities (User, Course, Lesson, Certificate, etc.)
- `DTOs`: Data transfer structures for APIs
- `Interfaces`: Abstractions for repositories and services
- `Enums`: Constant definitions for roles, lesson types, etc.

### 🏗️ Infrastructure Layer
- `Data`: SQL Server context, configuration, migrations
- `Repositories`: Generic and domain-specific implementations
- `Services`: Business logic (certificates, calendar APIs, etc.)
- `Auth`: Identity and JWT handling
- `SignalR`: Real-time messaging hub
- `Images`: File storage and `.webp` conversion
- `Certificates`: PDF generation with DinkToPdf

### 🌐 API Layer
- `Controllers`: RESTful endpoints for auth, course, lesson, chat, etc.
- `Middleware`: JWT pipeline, error handling
- `Extensions`: Dependency injection and app setup

### 📊 Admin Panel
- `Controllers`: Admin MVC logic
- `Views`: Razor UI for managing users, courses, and lessons
- `Models`: Admin-specific view models

---

## 🔧 Technology Stack

| Component        | Technology                         |
|------------------|-------------------------------------|
| Backend          | ASP.NET Core 8.0                    |
| Database         | SQL Server + EF Core                |
| Frontend         | Angular (developed by @abdofathy883)|
| Auth             | ASP.NET Identity + JWT              |
| Realtime         | SignalR                             |
| Certificates     | DinkToPdf (HTML → PDF)              |
| Scheduling       | Google Calendar API + OAuth2        |
| Media Processing | `.webp` image conversion service     |
| Admin UI         | Razor Pages + Bootstrap             |

---

## 🗂️ Repositories

- 🔗 [Frontend Repository](https://github.com/abdofathy883/EduLive-Frontend)

---

## 🚧 Deployment Status

⚠️ Project is complete and locally tested but **deployment is currently blocked** due to:
- Client-side delays in VPS provisioning
- Missing production credentials and SSL certificates required for Zoom and Google Meet integrations

---

## 📄 License

This project is developed for prototyping purposes. Contact the maintainer for commercial use or extensions.
