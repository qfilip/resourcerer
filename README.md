# resourcerer

## Setting the project up

- Open `Resourcerer_Sol/Resourcerer_Sol.sln` with Visual Studio and build the project. This will generate some client side files.

- Set `Resourcerer.Api` as startup project.

- Open nuget package manager console and select `Resourcerer.DataAccess` project there. Run `Add-Migration Initial`. Run `Update-Database`.

- In `/Resourcerer_Sol/src/Resourcerer.Api/wwwroot` a database file `resourcerer.db3` should appear. Use Sqlite Studio or similar program to execute the following SQL script to insert admin user:

<pre>
  INSERT INTO AppUser (
    Id,
    Name,
    PasswordHash,
    Permissions,
    EntityStatus,
    CreatedAt,
    ModifiedAt)
    VALUES (
        '11111111-1111-1111-1111-111111111111',
        'admin',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
        '{"User":15,"Category":15,"Element":15,"Event":15}',
        '0',
        '01.01.2000. 0:00:00',
        '01.01.2000. 0:00:00');
</pre>

- Open `/Resourcerer_Sol/client` and run `npm install`. Use `npm run dev` to start client app (make sure the backend project is running as well). Default port is `8080`.

- You will land on login screen. Username: `admin`, password `admin`.

- Check the browser console for certificate errors. If there are any, run:

<pre>
dotnet dev-certs https --clean
dotnet dev-certs https --trust
</pre>