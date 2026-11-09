# 🌙 Hotel Dluna

> **A small hotel-management portal for stays that feel a little closer to paradise.**

Hotel Dluna is an ASP.NET MVC application for managing a boutique hotel from one place. Guests can create accounts, browse the resort experience, reserve available rooms, and review their bookings. Administrators get a practical dashboard for monitoring occupancy, revenue, guests, and room inventory.

The project combines a warm, resort-inspired interface with straightforward server-rendered MVC workflows and SQL Server persistence.

## ✨ What it can do

| Area | Capabilities |
| --- | --- |
| **Guest experience** | Resort landing page, account registration, login/logout, room booking, generated bill view |
| **Reservations** | Finds an available room by type, calculates the stay total, creates a booking, and marks the room unavailable |
| **Customer dashboard** | Shows the signed-in customer’s booking history |
| **Administration** | Admin-only dashboard with total bookings, occupied rooms, total revenue, and booking details |
| **Room inventory** | List, create, edit, and delete rooms with availability and nightly pricing |
| **Presentation** | Bootstrap-based layout, responsive cards, carousel imagery, sidebar navigation, and beach-resort visuals |

## 🧭 The guest journey

```text
Landing page → Create account → Sign in → Choose room type
      → Confirm dates → Receive bill → Review bookings
```

For staff:

```text
Admin sign in → Dashboard → Inspect guests and bookings
                         → Maintain room inventory
```

## 🛠️ Technology

- **C# / ASP.NET MVC 5**
- **.NET Framework 4.8.1**
- **Razor views**
- **SQL Server** via `System.Data.SqlClient`
- **Bootstrap** and custom CSS
- **Visual Studio / IIS Express**

## 🚀 Getting started

### Prerequisites

- Windows with Visual Studio and ASP.NET web development tools
- .NET Framework 4.8.1 developer pack
- SQL Server or SQL Server Express
- NuGet package restore enabled

### 1. Clone and open

```bash
git clone https://github.com/raghava503/HOTELDLUNA.git
cd HOTELDLUNA
```

Open `HOTELDLUNA.sln` in Visual Studio.

### 2. Create the database

Create a SQL Server database named `HotelManagement`, then add the tables expected by the application:

- `Users`
- `Rooms`
- `Bookings`

The application expects the connection string named `DefaultConnection` from `Web.config`:

```xml
<add name="DefaultConnection"
     connectionString="Data Source=.;Initial Catalog=HotelManagement;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

Adjust `Data Source`, authentication, or the database name for your local SQL Server installation. This repository does not currently include a database migration or seed script, so the schema must be created separately.

### 3. Restore and run

Restore NuGet packages, build the solution, and run with IIS Express. Visual Studio will launch the site using the configured local web server.

## 🗺️ Useful routes

| Route | Purpose |
| --- | --- |
| `/Home/Index` | Hotel landing page |
| `/Home/Login` | Sign in |
| `/Home/NewUser` | Create a customer account |
| `/Home/Register` | Book a room |
| `/Home/CustomerDashboard` | View customer bookings |
| `/Home/AdminDashboard` | View administration metrics |
| `/Home/UserList` | List registered users |
| `/Rooms/Index` | Manage rooms |

## 📁 Project map

```text
HOTELDLUNA/
├── Controllers/       Request handling and booking/admin workflows
├── Models/            User, login, room, and booking models
├── Views/             Razor pages grouped by feature
├── Content/            CSS and resort imagery
├── App_Start/         MVC routing and application configuration
├── Web.config          SQL Server connection and runtime settings
└── HOTELDLUNA.sln     Visual Studio solution
```

## 🔐 Before using in production

This is an educational/project-scale application and should be hardened before deployment:

- Hash and salt passwords instead of storing or comparing them as plain text.
- Add authorization checks to every admin-only action, not only the dashboard.
- Add database migrations and parameter/type validation for all persisted data.
- Move connection strings and secrets out of source-controlled configuration.
- Make booking creation transactional so room allocation and booking insertion cannot drift apart.
- Add validation for overlapping dates, duplicate usernames, and concurrent reservations.

## 🌴 The idea

Hotel Dluna is designed around a simple promise: **the operations should be calm, even when the lobby is busy**. The project is a foundation for extending the experience with online payments, email confirmations, housekeeping workflows, richer availability search, and a full reservation calendar.

---

Made for better check-ins, cleaner room management, and stays worth remembering.
