# HomeCareApp

Technologies used:

- .NET 8.0 (ASP.NET Core MVC)
- Bootstrap 5 for design and layout

How to run the project:

1. Install the .NET 8.0 SDK.
2. Open the backend in the terminal in the project folder 'cd HomeCare/backend/HomeCare.Api'.
3. To create the migrations, run 'dotnet ef migrations add InitialCreate’.
4. To build the database, run 'dotnet ef database update’.
5. Run 'dotnet run' to start the backend.
6. Open the frontend in a new terminal with 'cd HomeCare/frontend'.
7. Install Node.js dependencies: 'npm install'.
8. Run 'npm start' to start the terminal. 
9. When the application starts, open your browser and go to the address shown in the terminal 
(for example, <http://localhost:5000>).

Test users:

- Admin – Username: <admin@oslomet.no> – Password: Admin123!
- Employee – Username: <caregiver@oslomet.no> – Password: Caregiver123!
- User – Username: <user@oslomet.no> – Password: User123!

Node.js version:

- The project uses Node.js v20.12.0 for handling Bootstrap and related dependencies.
