Restaurant Booking API
Ett komplett backend-system för restaurangbokningar byggt med ASP.NET Core 9 och Entity Framework Core. Systemet hanterar bokningar, kunder, bord och meny med robust autentisering och optimerad prestanda.
Funktioner
Autentisering

JWT-baserad autentisering för administratörer
Refresh tokens för säker session-hantering
Rollbaserad behörighetskontroll

Bokningssystem

Smart bokningslogik med 2-timmars regel
Automatisk tillgänglighetskontroll
Kundhantering med email som primär identifierare
Konfliktdetektering och validering

Restauranghantering

Bordhantering med kapacitet och status
Menysystem med kategorier och popularitet
Kunddatabas med automatisk dubletthantering
Admin-dashboard funktionalitet

Prestanda

Asynkrona metoder för hög genomströmning
Optimerade databasfrågor med indexering
Repository pattern för testbarhet
Global exception handling

Teknisk Stack

Framework: ASP.NET Core 9.0
Databas: SQL Server med Entity Framework Core
Autentisering: JWT Bearer tokens
Validering: Data Annotations
Dokumentation: Swagger/OpenAPI
Arkitektur: Clean Architecture med Repository pattern

Projektstruktur
├── Controllers/          # API Controllers
│   ├── AuthController.cs      # Autentisering
│   ├── BookingController.cs   # Bokningshantering
│   ├── CustomerController.cs  # Kundhantering (Admin)
│   ├── MenuItemController.cs  # Menyhantering
│   └── TablesController.cs    # Bordhantering
├── DTOs/                 # Data Transfer Objects
├── Models/               # Databasmodeller
├── Services/             # Affärslogik
├── Repositories/         # Dataåtkomst
├── Extensions/           # Konfigurationsextensions
├── Middleware/           # Custom middleware
└── Data/                 # Databaskonfiguration
