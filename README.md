# 🔍 Job Search App

A full-stack job search application built with **Angular** (Frontend) and **.NET Core** (Backend), backed by a **SQL Server** database.

---

## 📋 Prerequisites

Before running the application, ensure the following steps are completed:

| # | Step | Details |
|---|------|---------|
| 1 | **Node.js** | Download and install from [https://nodejs.org/](https://nodejs.org/) |
| 2 | **Angular CLI** | Install via terminal (see below) |
| 3 | **SQL Database** | Import `JobSearchDB.bacpac` into SQL Server |
| 4 | **Postman Collection** | Import `JobSearchApp.postman_collection.json` into Postman |
| 5 | **Angular API URL** | Update `ApiURL` in `environment.development.ts` |
| 6 | **.NET Allowed Origins** | Update `AllowedOrigins` in `appSettings.development.json` |
| 7 | **SQL Server Name** | Align `ServerName` in `appSettings.development.json` |

### Install Angular CLI

Open the **Visual Studio Code** terminal and run:

```bash
npm install -g @angular/cli
```

### Configure Environment Variables

**Angular** — In `environment.development.ts`, set `ApiURL` to match the localhost port of the .NET Core app:
```ts
export const environment = {
  ApiURL: '{your-dotnetApp-localhostURL}/api'
};
```

**.NET Core** — In `appSettings.development.json`, set `AllowedOrigins` to match the localhost port of the Angular app, and align `ServerName` with your machine's SQL credentials:
```json
{
  "AllowedOrigins": "{your-angularApp-localhosturl}",
  "ConnectionStrings": {
    "DefaultConnection": "Server={your-server-name};Database=JobSearchDB;..."
  }
}
```

---

> ⚠️ **Important Note**
>
> For the **Customer** and **Contractor** APIs, the **Update** and **Delete** functionalities are currently **pending for future development**, but the corresponding UI elements are already present in the application.

---

## 🚀 How to Run the App

### 🖥️ Backend (.NET Core)

1. Open the `.NET Core` solution file (`.sln`) in **Visual Studio**.
2. Ensure the correct startup project is selected.
3. Press **F5** or click **Run** to start the application.
4. Note the localhost port displayed — you will need this for the Angular configuration.

### 🌐 Frontend (Angular)

1. Open the Angular project folder in **Visual Studio Code**.
2. Launch the integrated terminal and run:
   ```bash
   ng serve
   ```
3. Once compiled, open your browser and navigate to the localhost link shown in the terminal (e.g., `http://localhost:4200`).

---

## 🛣️ Future Improvements

The following enhancements are planned for upcoming development cycles:

| Feature | Description |
|---------|-------------|
| 🚦 **Rate Limiting** | Prevent API abuse by throttling excessive requests |
| 🔐 **Authentication** | Secure API calls with token-based authentication |
| 🗂️ **Database Indexing** | Improve query performance by adding indexes to key tables |
| ⚡ **Caching** | Reduce load and improve response times with a caching layer |

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Angular |
| Backend | .NET Core |
| Database | SQL Server |
| API Testing | Postman / Swagger |
