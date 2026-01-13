# 📊 System Flow - ແຜນຜັງການທຳງານຂອງລະບົບ

## 🎯 ພາບລວມ (Overview)

ລະບົບນີ້ສ້າງດ້ວຍ **Avalonia UI Framework** ໃຊ້ pattern **MVVM** ແລະເຊື່ອມຕໍ່ກັບ **MySQL Database**

---

## 🚀 Flow การทำงานทั้งหมด

```
Program.cs (Entry Point)
    ↓
App.axaml.cs (Initialize App)
    ↓
Login.axaml.cs (Authentication)
    ↓
MainForm.axaml.cs (Main Window)
    ↓
Home.axaml.cs (Default Page)
```

---

## 📝 รายละเอียด Flow แต่ละขั้นตอน

### ขั้นตอนที่ 1: Program.cs (Entry Point)
**ไฟล์:** `Program.cs`

```csharp
[STAThread]
public static void Main(string[] args) => BuildAvaloniaApp()
    .StartWithClassicDesktopLifetime(args);

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();
```

**หน้าที่:**
- เป็นจุดเริ่มต้นของโปรแกรม (Entry Point)
- สร้างและ configure Avalonia application
- เรียก `App.axaml.cs` เพื่อเริ่มต้น application

**ไปต่อที่:** `App.axaml.cs`

---

### ขั้นตอนที่ 2: App.axaml.cs (Application Initialization)
**ไฟล์:** `App.axaml.cs`

```csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        DisableAvaloniaDataAnnotationValidation();
        // เปิดหน้า Login เป็นหน้าแรก
        desktop.MainWindow = new Login();
    }

    base.OnFrameworkInitializationCompleted();
}
```

**หน้าที่:**
- Initialize framework ของ Avalonia
- สร้างหน้า **Login** เป็นหน้าแรกที่แสดง
- ปิด data annotation validation

**ไปต่อที่:** `Views/Auth/Login.axaml.cs`

---

### ขั้นตอนที่ 3: Login.axaml.cs (Authentication)
**ไฟล์:** `Views/Auth/login.axaml.cs`

#### 3.1 เมื่อเปิดหน้า Login
```csharp
public Login()
{
    InitializeComponent();
    this.Opened += async (s, e) => await TestDatabaseConnection();
    this.Opened += (s, e) => InitializeUI();
}
```

**การทำงาน:**
1. ทดสอบการเชื่อมต่อ Database
2. Initialize UI components (Button, TextBox, CheckBox)

---

#### 3.2 ทดสอบการเชื่อมต่อ Database
```csharp
private async Task TestDatabaseConnection()
{
    try
    {
        var db = new Connection_db();
        await db.TestConnection();
        await ShowSuccessDialogHelper.ShowSuccessDialog(this, "ເຊື່ອມຕໍ່ຖານຂໍ້ມູນສຳເລັດ!");
    }
    catch (Exception ex)
    {
        await ShowErrorDialogHelper.ShowErrorDialog(this, ex.Message);
    }
}
```

**เรียกใช้:**
- `Core/Helpers/Connection_db.cs` → สร้างการเชื่อมต่อ MySQL
- `Core/Config/db.cs` → ดึง connection string

---

#### 3.3 เมื่อกดปุ่ม Login
```csharp
private async void LoginButton_Click(object? sender, RoutedEventArgs e)
{
    string username = usernameTextBox.Text?.Trim() ?? "";
    string password = passwordTextBox.Text?.Trim() ?? "";

    try
    {
        var db = new Connection_db();
        using (var con = db.connectdb)
        {
            if (con.State == ConnectionState.Closed)
                await con.OpenAsync();

            string sql = "SELECT emp_id, emp_name, status FROM employee WHERE username=@username AND password=@password";

            using (var command = new MySqlCommand(sql, con))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", Encryptor.MD5Hash(password));

                using (var rd = await command.ExecuteReaderAsync())
                {
                    if (await rd.ReadAsync())
                    {
                        // ✅ Login สำเร็จ - สร้าง MainForm
                        MainForm mf = new MainForm(
                            rd.GetString(0),  // emp_id
                            rd.GetString(1),  // emp_name
                            rd.GetString(2)   // status
                        );
                        mf.Show();
                        this.Close();
                    }
                    else
                    {
                        // ❌ Login ไม่สำเร็จ
                        await ShowErrorDialogHelper.ShowErrorDialog(this, "ບັນຊີເຂົ້າໃຊ້ ແລະ ລະຫັດຜ່ານບໍ່ຖືກຕ້ອງ!");
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        await ShowErrorDialogHelper.ShowErrorDialog(this, "Error: " + ex.Message);
    }
}
```

**เรียกใช้:**
- `Core/Helpers/Connection_db.cs` → เชื่อมต่อ Database
- `Core/Helpers/Encryptor.cs` → เข้ารหัส password ด้วย MD5
- `Core/Helpers/ShowErrorDialog.cs` → แสดง error dialog

**Database Query:**
- ตาราง: `employee`
- เงื่อนไข: `username` และ `password` (MD5 hash)
- ดึงข้อมูล: `emp_id`, `emp_name`, `status`

**ถ้า Login สำเร็จ ไปต่อที่:** `Views/MainForm/mainForm.axaml.cs`

---

### ขั้นตอนที่ 4: MainForm.axaml.cs (Main Application Window)
**ไฟล์:** `Views/MainForm/mainForm.axaml.cs`

```csharp
public string emp_id;
public string emp_name;
public string emp_status;

public MainForm(string id, string name, string status)
{
    InitializeComponent();
    this.emp_id = id;
    this.emp_name = name;
    this.emp_status = status;

    // สร้าง Navbar พร้อมส่ง status และส่ง reference ของ MainForm
    var navbar = new Navbar(status, this);
    DockPanel.SetDock(navbar, Dock.Top);

    // เพิ่ม Navbar เข้าไปใน DockPanel ที่ตำแหน่งแรก
    var mainDockPanel = this.FindControl<DockPanel>("MainDockPanel");
    if (mainDockPanel != null)
    {
        mainDockPanel.Children.Insert(0, navbar);
    }

    // ✅ แสดงหน้า Home เป็นค่าเริ่มต้น
    ShowPage(new Home());
}

// Method สำหรับแสดงหน้าต่างๆ ใน ContentControl
public void ShowPage(UserControl page)
{
    var contentArea = this.FindControl<ContentControl>("MainContentArea");
    if (contentArea != null)
    {
        contentArea.Content = page;
    }
}
```

**หน้าที่:**
1. **รับข้อมูล User** จากหน้า Login (`emp_id`, `emp_name`, `status`)
2. **สร้าง Navbar** → `Views/Common/Navbar.axaml.cs`
3. **แสดงหน้า Home** เป็นค่าเริ่มต้น → `Views/Home/home.axaml.cs`

**Components ที่ใช้:**
- `DockPanel` → จัดวาง Navbar ด้านบน
- `ContentControl` → แสดงหน้าต่างๆ (Home, Profile, Sale, etc.)

**ไปต่อที่:** 
- `Views/Common/Navbar.axaml.cs` (Navigation)
- `Views/Home/home.axaml.cs` (Default Page)

---

### ขั้นตอนที่ 5: Navbar.axaml.cs (Navigation Bar)
**ไฟล์:** `Views/Common/navbar.axaml.cs`

```csharp
public partial class Navbar : UserControl
{
    private MainForm mainForm;
    
    public Navbar(string status, MainForm form)
    {
        InitializeComponent();
        this.mainForm = form;
        InitializeNavigation();
    }
    
    private void InitializeNavigation()
    {
        // เมื่อกดปุ่ม Home
        var homeButton = this.FindControl<Button>("toolStripMenuItemHome");
        if (homeButton != null)
        {
            homeButton.Click += (s, e) => mainForm.ShowPage(new Home());
        }
        
        // เมื่อกดปุ่ม Profile
        var profileButton = this.FindControl<Button>("toolStripMenuItemProfile");
        if (profileButton != null)
        {
            profileButton.Click += (s, e) => mainForm.ShowPage(new Profile());
        }
        
        // เมื่อกด Logout
        var logoutButton = this.FindControl<Button>("toolStripMenuItemLogout");
        if (logoutButton != null)
        {
            logoutButton.Click += async (s, e) => await Logout();
        }
    }
    
    private async Task Logout()
    {
        var login = new Login();
        login.Show();
        mainForm.Close();
    }
}
```

**หน้าที่:**
- สร้าง Navigation Menu สำหรับเปลี่ยนหน้า
- เรียก `ShowPage()` จาก MainForm เพื่อเปลี่ยนหน้า
- จัดการ Logout กลับไปหน้า Login

**สามารถเปลี่ยนไปหน้า:**
- `Home.axaml.cs`
- `Profile.axaml.cs`
- `Sale.axaml.cs`
- `Branch.axaml.cs`
- และอื่นๆ

---

### ขั้นตอนที่ 6: Home.axaml.cs (Home Page)
**ไฟล์:** `Views/Home/home.axaml.cs`

```csharp
public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
    }
}
```

**หน้าที่:**
- แสดงหน้า Dashboard/Homepage
- อาจมีการแสดงสถิติ, ข้อมูลสรุป, หรือข้อมูลอื่นๆ

---

## 🗄️ Core Components

### 1. Database Configuration
**ไฟล์:** `Core/Config/db.cs`

```csharp
public static class DatabaseConfig
{
    public static string GetConnectionString()
    {
        string host = "localhost";
        string database = "pos_workshop";
        string username = "root";
        string password = "";
        string port = "3306";
        
        string connection_string = $"Server={host};Port={port};Database={database};Uid={username};Pwd={password};CharSet=utf8;Allow User Variables=True;";
        
        return connection_string;
    }
}
```

**หน้าที่:** ให้ connection string สำหรับเชื่อมต่อ MySQL

---

### 2. Database Connection
**ไฟล์:** `Core/Helpers/Connection_db.cs`

```csharp
public class Connection_db
{
    public MySqlConnection connectdb;

    public Connection_db()
    {
        string connection_string = DatabaseConfig.GetConnectionString();
        connectdb = new MySqlConnection(connection_string);
    }

    public async Task<bool> TestConnection()
    {
        try
        {
            connectdb.Open();
            Console.WriteLine("✅ ເຊື່ອມຕໍ່ຖານຂໍ້ມູນສຳເລັດ!");
            connectdb.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            string errorMessage = ex.Number switch
            {
                0 => "ບໍ່ສາມາດເຊື່ອມຕໍ່ກັບ Server ໄດ້",
                1045 => "Username ຫຼື Password ບໍ່ຖືກຕ້ອງ",
                1049 => "ບໍ່ພົບຖານຂໍ້ມູນທີ່ລະບຸ",
                _ => $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}"
            };
            throw new Exception(errorMessage);
        }
    }
}
```

**หน้าที่:** 
- สร้าง MySQL connection
- ทดสอบการเชื่อมต่อ

---

### 3. Password Encryption
**ไฟล์:** `Core/Helpers/Encryptor.cs`

```csharp
public static class Encryptor
{
    public static string MD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
```

**หน้าที่:** เข้ารหัส password ด้วย MD5 ก่อนส่งไป query database

---

## 🔄 Flow Chart แบบละเอียด

```
┌─────────────────────────────────────────────────────────────┐
│                    1. Program.cs                             │
│                    Main() Entry Point                         │
└───────────────────────┬─────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│                    2. App.axaml.cs                           │
│        OnFrameworkInitializationCompleted()                  │
│        desktop.MainWindow = new Login();                     │
└───────────────────────┬─────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│                 3. Login.axaml.cs                            │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ • TestDatabaseConnection()                           │   │
│  │   └→ Connection_db.cs                                │   │
│  │      └→ DatabaseConfig.cs                            │   │
│  │                                                       │   │
│  │ • LoginButton_Click()                                │   │
│  │   └→ Encryptor.MD5Hash(password)                     │   │
│  │   └→ MySQL Query: SELECT FROM employee               │   │
│  └──────────────────────────────────────────────────────┘   │
└───────────────────────┬─────────────────────────────────────┘
                        ↓
              ┌─────────────────┐
              │ Login Success?  │
              └─────┬───────┬───┘
                 ❌ │       │ ✅
                    │       ↓
                    │  ┌─────────────────────────────────────┐
                    │  │     4. MainForm.axaml.cs            │
                    │  │  Constructor(emp_id, name, status)  │
                    │  │  ┌──────────────────────────────┐   │
                    │  │  │ • Create Navbar              │   │
                    │  │  │   └→ Navbar.axaml.cs         │   │
                    │  │  │                              │   │
                    │  │  │ • ShowPage(new Home())       │   │
                    │  │  │   └→ Home.axaml.cs           │   │
                    │  │  └──────────────────────────────┘   │
                    │  └─────────────────────────────────────┘
                    │                   ↓
                    │  ┌─────────────────────────────────────┐
                    │  │      5. Navbar.axaml.cs             │
                    │  │  • Home Button → ShowPage(Home)     │
                    │  │  • Profile → ShowPage(Profile)      │
                    │  │  • Sale → ShowPage(Sale)            │
                    │  │  • Logout → new Login()             │
                    │  └─────────────────────────────────────┘
                    │                   ↓
                    │  ┌─────────────────────────────────────┐
                    │  │      6. Home.axaml.cs               │
                    │  │    (หรือหน้าอื่นๆ)                   │
                    │  │  Displayed in MainContentArea       │
                    │  └─────────────────────────────────────┘
                    │
                    └──→ (แสดง Error Dialog และอยู่ที่หน้า Login)
```

---

## 📂 ไฟล์ที่เกี่ยวข้องทั้งหมด

### Entry Point & Configuration
1. **Program.cs** → จุดเริ่มต้นของโปรแกรม
2. **App.axaml.cs** → Initialize application และเปิดหน้า Login

### Views (UI層)
3. **Views/Auth/login.axaml.cs** → หน้า Login
4. **Views/MainForm/mainForm.axaml.cs** → หน้าต่างหลัก
5. **Views/Common/navbar.axaml.cs** → Navigation bar
6. **Views/Home/home.axaml.cs** → หน้า Home
7. **Views/Profile/** → หน้า Profile
8. **Views/Sale/** → หน้า Sale
9. **Views/Branch/** → หน้า Branch

### Core Components
10. **Core/Config/db.cs** → Database configuration
11. **Core/Helpers/Connection_db.cs** → Database connection
12. **Core/Helpers/Encryptor.cs** → Password encryption
13. **Core/Helpers/ShowErrorDialog.cs** → แสดง error dialog
14. **Core/Helpers/ShowSuccessDialog.cs** → แสดง success dialog
15. **Core/Helpers/ShowConfirmDialog.cs** → แสดง confirm dialog

---

## 🎯 สรุปการทำงาน

1. **เริ่มต้น:** `Program.cs` → `App.axaml.cs` → เปิดหน้า `Login`
2. **Login:** ตรวจสอบ username/password จาก database
3. **สำเร็จ:** สร้าง `MainForm` พร้อมส่งข้อมูล user (emp_id, emp_name, status)
4. **MainForm:** สร้าง `Navbar` และแสดงหน้า `Home` เป็นค่าเริ่มต้น
5. **Navigation:** ใช้ `Navbar` เปลี่ยนหน้าผ่าน `ShowPage()` method
6. **Logout:** ปิด MainForm และเปิดหน้า `Login` ใหม่

---

## 🔗 Database Schema

### ตาราง `employee`
```sql
CREATE TABLE employee (
    emp_id VARCHAR(50) PRIMARY KEY,
    emp_name VARCHAR(100),
    username VARCHAR(50) UNIQUE,
    password VARCHAR(255),  -- เก็บเป็น MD5 hash
    status VARCHAR(20)      -- เช่น "admin", "user", "manager"
);
```

---

## 💡 หมายเหตุ

- โปรเจคใช้ **MVVM Pattern** แต่ยังไม่แยก ViewModel ออกจาก Code-behind อย่างเต็มรูปแบบ
- Password เข้ารหัสด้วย **MD5** (ควรเปลี่ยนเป็น bcrypt หรือ argon2 สำหรับ production)
- การเปลี่ยนหน้าใช้ `ContentControl` และ `ShowPage()` method
- Navbar มี reference ของ MainForm เพื่อเรียก `ShowPage()` ได้โดยตรง
