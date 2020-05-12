## Jiggswap API Setup

This is the API project for Jiggswap.com.

### Installation / Setup

You should have the following software installed:

- [.NET Core 3.1+](https://dotnet.microsoft.com/download)
- [Visual Studio (Community)](https://visualstudio.microsoft.com/vs/)
- [DBeaver](https://dbeaver.io/download/)
- [Postgres](https://www.postgresql.org/download/)

### Running the app

- First, run migrations (See below)
- Create a new file called Appsettings.json inside ../JiggswapCore/JiggswapApi. Copy the following contents into it:

```
{
  "ConnectionStrings": {
    "JiggswapDb": "User ID=postgres;Password=DB_PASSWORD;Database=testdb2;Host=localhost;Port=5432"
  },
  "Jwt": {
    "Key": "jwt_dev_key_jwt_dev_key"
  },
  "Notifications": {
    "SendGridApiKey": "SENDGRID_API_KEY",
    "FromEmail": "notifications@jiggswap.com",
    "FromName": "Jiggswap Notifications",
    "BaseUrl": "http://localhost:5000"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

    - Update the key "ConnectionStrings.JiggswapDb" with a value of your local Postgresql database connection string.
        - [See here for a template connection string](https://www.connectionstrings.com/postgresql/).
    - (Optional) Update the entry `Jwt:Key` with anything you want (must be at least 16 characters long or it will explode).
    - Create a new API Key for SendGrid.
        - Log in to Azure Portal -> SendGrid Accounts -> andmichaelis -> Manage to get to the SendGrid dashboard.
        - Create an API Key under Settings -> API Keys.
        - Copy the value into `Notifications:SendGridApiKey`

- Then, open ../JiggswapCore/JiggswapApi/ in a terminal (i.e., Powershell).
- Type `dotnet watch run`. - This will run the dotnet web server and reload the server whenever you update a \*.cs file.

### Running Migrations

- Open JiggswapCore.sln in Visual Studio.
- At the top, next to "Debug" / Any CPU, there is a dropdown indicating which project to run.
  - Select JiggswapMigrations here.
- Click the green Play button.
- You will be prompted for the DB Username, password, and database you want to target (on your local machine).
  - Enter the credentials for your local postgresql database
- After entering credentials, it will execute all of the migrations inside of "./Migrations" that have not yet been run.

### Creating a new migration

We use [Fluent Migrator](https://fluentmigrator.github.io/) for migrations. See that link for documentation.

- When you want to create a migration (Add/delete a database table, add/delete rows, etc), create a new \*.cs file in Migrations. - Name the file `YYYYMMDDHHmm_MigrationName.cs`. - Rename the class name to just `MigrationName`. - Extend the `Migration` class from `FluentMigrator`. - Add a class attribute of `Migration(YYYYMMDDHHmm)`. - Check out the other Migration files to see examples.
- The Migration project will automatically locate any class with the `Migration` attribute.

### Publishing to Production

The API is hosted on Digital Ocean droplet. Get access to SSH @api.jiggswap.com
Ensure an up-to-date `appsettings.api.json` and `appsettings.migrations.json` is copied from the appropriate project and placed in `./publish/`
Running `publish.bat` will then:

- Create publishes of the Api and Migrations projects
- Copy the aforementioned appsettings to their appropriate published directory
- Securely transfer via SSH the contents of `./publish` to `api.jiggswap.com`
