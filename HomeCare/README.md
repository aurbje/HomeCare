# HomeCareApp

## Teknologi

- .NET 8.0 (ASP.NET Core MVC)
- Bootstrap 5 for styling

## Kjøreprosjekt

1. Installer .NET 8.0 SDK.
2. Kjør `dotnet run` i prosjektmappen.
3. Åpne nettleser på <http://localhost:5000>

## Node.js-versjon

Brukt Node.js v20.12.0 (kun for Bootstrap og avhengigheter, ikke React).

---

# Structure

---

## .gitignore

For å ignorere feilmeldinger som denne

```
error: Your local changes to the following files would be overwritten by checkout:
    HomeCare/obj/Debug/...
Please commit your changes or stash them before you switch branches.”
```

skriv følgende kommandoer i terminalen:

```
git rm -r --cached obj/
git commit -m "Remove obj/ from version control"
```

og lag filen `.gitignore` under `HomeCare`-mappen.

HomeCare/
└── .gitignore

---

## Database

HomeCare/
└── Data/
    └── AppDbContext.cs  <--Appointment, Booking, Users, Reminders (previously "BookingDbContext")
    └── AppDbContextFactory.cs <-- EF core  (previously "BookingDbContextFactory.cs")
    └── DbInitializer.cs <-- Dummy data
└── booking.db <--- named in "AppDbContextFactory.cs"

```
# install og restart VScode
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools

#
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update

# If any errors, see which part is wrong by this command
dotnet build

# RESET migrations (If migration is broken or unnecessary)
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update

# Reflect model changes in database
rm booking.db
dotnet ef migrations add SyncModelChanges
dotnet ef database update

```
