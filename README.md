# Job Search App

## Prerequisites

1. **Download Node.js** via the official link: [https://nodejs.org/](https://nodejs.org/)

2. **Install Angular CLI** through the Visual Studio Code Terminal by running:
   ```bash
   npm install -g @angular/cli
   ```

3. **Import the SQL Database** — Import the `JobSearchDB.bacpac` file into your SQL database.

4. **Import Postman Collection** — Import `JobSearchApp.postman_collection.json` into Postman.

5. **Configure Angular API URL** — Under the Angular app, update the `ApiURL` value in `environment.development.ts` to match the localhost value of the .NET Core app.

6. **Configure .NET Allowed Origins** — Under the .NET app, update the `AllowedOrigins` value in `appSettings.development.json` to match the localhost value of the Angular app.

7. **Align SQL Server Name** — Update the `ServerName` in `appSettings.development.json` to match the current machine's SQL credentials.

---

> ⚠️ **Important Note**
>
> For the **Customer** and **Contractor** API, the **update** and **delete** functionalities are pending for future development but are present in the UI.

---

## How to Run the App

### Backend

1. Open the .NET Core solution via **Visual Studio**.
2. Run the application.

### Frontend

1. Open a terminal in the Angular project folder via **Visual Studio Code**.
2. Execute:
   ```bash
   ng serve
   ```
3. Open your browser to the localhost link provided in the terminal.

---

## Future Improvements

- Addition of Rate Limiting
- Implementation of Authentication for API calls
- Addition of Index to database tables
- Addition of Caching
