# EventHub

EventHub is a full-stack event management platform that lets organizers create and manage events, attendees register and receive PDF tickets, and admins oversee users, categories, and payments. It includes an **AI-powered event summary** feature (OpenAI + Gemini) for quick event overviews.

## Tech Stack

### Backend — .NET 8 Web API

| Technology | Purpose |
|---|---|
| ASP.NET Core 8 | REST API, JWT authentication, Swagger |
| Entity Framework Core | SQL Server ORM, repository pattern |
| AutoMapper | DTO mapping |
| iText7 + QRCoder | PDF ticket generation with QR codes |
| MailKit | Email (verification, password reset) |
| BCrypt | Password hashing |
| HttpClient | OpenAI & Gemini API integration |

**Architecture:** Clean layered design — `EventHub.API` → `EventHub.Core` (entities, DTOs, interfaces) → `EventHub.Infrastructure` (EF Core, services, repositories).

### Frontend — Angular 20

| Technology | Purpose |
|---|---|
| Angular 20 (standalone components) | SPA with lazy-loaded routes |
| TypeScript 5.9 | Type-safe development |
| RxJS | Reactive HTTP calls |
| SCSS | Component styling |
| Angular Guards & Interceptors | JWT auth, role-based access |

**Architecture:** Feature-based components with shared services, models, and guards. Role-aware sidebar navigation for Admin, Organizer, and Attendee.

## Features

- **Authentication** — Register, login, JWT tokens, role-based access (Admin / Organizer / Attendee)
- **Events** — CRUD, status management, category filtering, seat capacity
- **Registrations** — Attendees register/cancel; automatic ticket creation
- **Tickets** — PDF tickets with QR codes; card-based UI with download
- **Payments** — Payment tracking (Admin dashboard)
- **AI Event Summary** — Quick AI-generated summaries via OpenAI or Gemini (toggle switch), shown on event list and detail pages
- **Localization** — English & Arabic support (backend)

## Project Structure

```
EventHub/
├── EventHub.API/              # Controllers, Program.cs, appsettings
├── EventHub.Core/             # Entities, DTOs, interfaces, constants
├── EventHub.Infrastructure/   # EF Core, repositories, services
├── EventHub.web/
│   └── ClientApp/             # Angular SPA
│       └── src/app/
│           ├── components/    # UI components (events, tickets, registrations…)
│           ├── services/      # HTTP services
│           ├── models/        # TypeScript interfaces
│           └── guards/        # Auth & role guards
└── EventHub.sln
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- SQL Server (LocalDB or Express)
- OpenAI and/or Gemini API keys (for AI summaries)

### Backend Setup

```bash
cd EventHub/EventHub.API
dotnet restore
dotnet ef database update   # if migrations exist
dotnet run
```

API runs at `https://localhost:44370` — Swagger UI available in Development mode.

**Configure AI keys** in `appsettings.json` or User Secrets:

```json
"Ai": {
  "DefaultProvider": "openai",
  "OpenAi": {
    "ApiKey": "sk-...",
    "Model": "gpt-4o-mini"
  },
  "Gemini": {
    "ApiKey": "AI...",
    "Model": "gemini-2.0-flash"
  }
}
```

### Frontend Setup

```bash
cd EventHub/EventHub.web/ClientApp
npm install
npm start
```

App runs at `http://localhost:4200`.

### Database

Update the connection string in `EventHub.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EventHubDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

## API Highlights

| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/user/login` | POST | Public | Login & get JWT |
| `/api/event` | GET | Public | List all events |
| `/api/event/{id}` | GET | Public | Event details |
| `/api/event/{id}/summary?provider=openai\|gemini` | GET | Admin, Organizer, Attendee | AI event summary |
| `/api/registration` | POST | Authenticated | Register for event |
| `/api/ticket/my` | GET | Authenticated | User's tickets |
| `/api/ticket/{id}/download` | GET | Authenticated | Download ticket PDF |

## Roles

| Role | Capabilities |
|---|---|
| **Admin** | Manage users, categories, payments; full event access |
| **Organizer** | Create/edit own events, view registrations |
| **Attendee** | Browse events, register, view tickets & registrations, AI summaries |

## License

This project is for educational/portfolio purposes.
