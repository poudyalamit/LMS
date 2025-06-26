# 📚 Kallu Learning Management System (LMS)

A powerful and clean Learning Management System (LMS) built using **ASP.NET Core MVC**, designed to streamline teaching and learning experiences. Kallu LMS supports **role-based access** for Teachers and Students, enabling secure and organized course/module management.

---

## 🚀 Features

✅ **Role-Based Access Control**  
- Teachers and Students login with different privileges  
- ASP.NET Identity authentication system

✅ **Course & Module Management**  
- Teachers can create, update, and delete courses and modules  
- Add rich descriptions and files to each module

✅ **Student Enrollment**  
- Students can enroll and deroll from courses  
- View assigned modules and course details

✅ **Real-Time Notifications**  
- SignalR-based push notifications for module additions/deletions  
- Toast-based frontend notifications

✅ **Search & Navigation**  
- Easily search courses by name or code  
- Clean Bootstrap UI for a modern and responsive experience

---

## 🛠️ Tech Stack

| Layer       | Technology                  |
|-------------|------------------------------|
| **Backend** | ASP.NET Core MVC (.NET 8+)   |
| **Auth**    | ASP.NET Core Identity         |
| **Frontend**| Razor Views, Bootstrap 5      |
| **Database**| SQL Server (EF Core)         |
| **Realtime**| SignalR (notifications)      |
| **Other**   | DI, Role-based Authorization |

---

## ⚙️ Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/poudyalamit/LMS.git
cd LMS
```

### 2. Open the Project

Open the LMS.sln file in Visual Studio.

### 3.  Configure the Database

Update your connection string in appsettings.json:

``` bash
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=LMSDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 4. Run EF Core Migrations

``` bash
dotnet ef database update
```
Or allow Visual Studio to apply migrations automatically at runtime.


