
### Database Set-up
#### 1. Install Dependencies
If you don't already have SQL Server installed:

Download & Install [SQL Server 2025 Express](https://www.microsoft.com/en-us/download/details.aspx?id=104781)
- Run the installer and choose the *Basic* installation type.
- Leave the default instance name (usually SQLEXPRESS).

Download & Install [SQL Server Management Studio](https://learn.microsoft.com/en-us/ssms/install/install)
- Run the installer and follow the prompts to install the database management UI.

Follow: [This video](https://youtu.be/vbJ_p0Zs3Lk?si=DfLektjjN6NhXhUD) if for a more detailed install guide

#### 2. Create the Database
1. Open *SQL Server Management Studio (SSMS)*.
2. Connect to your local server. The server name should be `localhost\SQLEXPRESS` (or `.\SQLEXPRESS`).
3. Choose **Windows Authentication** and click **Connect**.
4. In the Object Explorer on the left, right-click the **Databases** folder and select **New Database...**
5. Name the database *`GLMSDb`* and click **OK**

Open *`appsettings.json`* in the root of the **GLMSMVC** project and ensure the *connection string* looks exactly like this:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=GLMSDb;Integrated Security=true;TrustServerCertificate=True;"
}
```

#### 4. Apply Database Migrations
Once the database is created and the connection string is set, you need to apply the EF Core migrations to generate the tables.

If you open with Visual Studio:
- Open the **Package Manager Console** and run **`Update-Database`**.

If you open with VS Code / .NET CLI
- Run `dotnet ef database update`.

Database Set-up Complete. ✅

---
