# resourcerer

## .NET project setup

- From the `root` directory run:
  ```
    dotnet build ./src/Resourcerer.Api/Resourcerer.Api.csproj
  ```
  This will generate `./src/client/src/app/models/dtos` directory, and export C# classes to TypeScript.

### Database (Visual Studio)

- Set `Resourcerer.Api` as startup project.

- Open nuget package manager console and select `Resourcerer.DataAccess` project there. Run `Add-Migration Initial`. Run `Update-Database`.

### Database (CLI)

- Install ef core cli tools: [link](https://learn.microsoft.com/en-us/ef/core/cli/)

- Execute
  ```
  cd ./src/Resourcerer.DataAccess
  dotnet ef migrations add Initial
  dotnet ef database update
  ```

### User setup

The easiest way to seed admin user, is to run the project and execute an endpoint from swagger. Resourcerer uses `ReinforcedTypings` library to scaffold client side models, which may interfere with `dotnet run` command. Use Visual Studio to run the project, or build it first, then run the executable.

#### Seed default admin user

SQLite db file should now be present in `./src/Resourcerer.Api/wwwroot`. Run the api project and open http://localhost:24822/swagger. Find `seed` endpoint and execute it. Mocked admin user should be inserted into the DB. 

#### Seed custom admin user

To modify mocked user, open: `./src/Resourcerer.DataAccess/Utilities/Faking/DF.Database.cs`, and modify the following lines:

```csharp
var appUser = Fake<AppUser>(ctx, x =>
{
    // set username
    x.Name = "shk";
    // set password
    x.PasswordHash = Resourcerer.Utilities.Cryptography.Hasher.GetSha256Hash("123");
    x.Company = company;
    x.Permissions = permissions;
});
```

Usernames are unique, so if the database was seeded, it will have to be recreated from scratch.

## Angular project setup

- Open `./src/client` and run `npm install`. Use `ng s -o` to start client app (make sure the backend project is running as well). Default port is `8080`.

## Other

Check the browser console for certificate errors. If there are any, run:

<pre>
dotnet dev-certs https --clean
dotnet dev-certs https --trust
</pre>