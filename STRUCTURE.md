# โครงสร้างโปรเจกต์ (Project Structure)

เอกสารนี้อธิบายโครงสร้างและการจัดระเบียบไฟล์ของโปรเจกต์ My-program

## 📐 สถาปัตยกรรม (Architecture)

โปรเจกต์นี้ใช้สถาปัตยกรรม **MVVM (Model-View-ViewModel)** ซึ่งเป็นมาตรฐานสำหรับ Avalonia UI Applications

### ข้อดีของ MVVM:
- ✅ แยก UI (View) ออกจาก Business Logic (ViewModel)
- ✅ ง่ายต่อการทดสอบ (Testable)
- ✅ สามารถทำงานร่วมกันได้หลายคน (Maintainable)
- ✅ นำโค้ดกลับมาใช้ใหม่ได้ง่าย (Reusable)

---

## 📂 โครงสร้างโฟลเดอร์

### 1️⃣ **Assets/** - ทรัพยากรทั้งหมด

```
Assets/
├── Fonts/          # ไฟล์ฟอนต์ (.ttf, .otf)
├── Icons/          # ไอคอนขนาดเล็ก
└── Images/         # รูปภาพและกราฟิกต่างๆ
```

**วิธีใช้งาน:**
```xml
<!-- ใน AXAML file -->
<Image Source="/Assets/Images/logo-login.jpg"/>
```

---

### 2️⃣ **Core/** - ฟังก์ชันหลักและ Business Logic

#### **Core/Config/**
เก็บไฟล์การตั้งค่าต่างๆ

**ไฟล์:**
- `db.cs` - การตั้งค่าการเชื่อมต่อฐานข้อมูล

**ตัวอย่างการใช้งาน:**
```csharp
using My_program.Core.Config;

var connection = DbConfig.GetConnection();
```

#### **Core/Helpers/**
เก็บ Helper Classes ที่ใช้ทั่วทั้งโปรเจกต์

**ไฟล์:**
- `Connection_db.cs` - จัดการการเชื่อมต่อฐานข้อมูล
- `Encryptor.cs` - เข้ารหัสและถอดรหัสข้อมูล
- `NumberFormatter.cs` - จัดรูปแบบตัวเลข (เช่น สกุลเงิน)
- `ShowDialog.cs` - แสดง Dialog ทั่วไป
- `ShowErrorDialog.cs` - แสดง Error Dialog
- `ShowSuccessDialog.cs` - แสดง Success Dialog
- `ShowConfirmDialog.cs` - แสดง Confirmation Dialog

**ตัวอย่างการใช้งาน:**
```csharp
using My_program.Core.Helpers;

// แสดง Success Dialog
await ShowSuccessDialog.Show("บันทึกข้อมูลสำเร็จ!");

// Encrypt password
var encrypted = Encryptor.Encrypt("mypassword");

// Format number
var formatted = NumberFormatter.FormatCurrency(1250.50); // "1,250.50 ฿"
```

#### **Core/Services/**
เก็บ Business Logic Services (ยังว่างอยู่ - พร้อมสำหรับขยาย)

**ควรใช้สำหรับ:**
- API Services (การเรียก API)
- Authentication Service
- Data Validation Service
- Logging Service

---

### 3️⃣ **Models/** - Data Models

เก็บ Classes ที่แทนข้อมูล (Entities, DTOs)

**ตัวอย่าง:**
```csharp
namespace My_program.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

---

### 4️⃣ **ViewModels/** - MVVM ViewModels

ViewModels ทำหน้าที่เป็นตัวกลางระหว่าง View และ Model

**โครงสร้าง:**
```
ViewModels/
├── Auth/               # ViewModels สำหรับ Authentication
├── Branch/             # ViewModels สำหรับการจัดการสาขา
├── Home/               # ViewModels สำหรับหน้าหลัก
├── Profile/            # ViewModels สำหรับโปรไฟล์
├── Sale/               # ViewModels สำหรับการขาย
├── MainWindowViewModel.cs
└── ViewModelBase.cs    # Base class สำหรับ ViewModels ทั้งหมด
```

**ตัวอย่าง ViewModel:**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace My_program.ViewModels.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [RelayCommand]
        private async Task Login()
        {
            // Login logic here
        }
    }
}
```

---

### 5️⃣ **Views/** - UI Views

Views เก็บไฟล์ UI (AXAML) และ Code-behind เท่านั้น

**โครงสร้าง:**
```
Views/
├── Auth/               # หน้า Login, Register
│   ├── login.axaml
│   └── login.axaml.cs
├── Branch/             # หน้าจัดการสาขา
│   ├── branMagement.axaml
│   └── branMagement.axaml.cs
├── Common/             # Components ที่ใช้ร่วมกัน
│   ├── navbar.axaml
│   └── navbar.axaml.cs
├── Home/               # หน้าหลัก
│   ├── home.axaml
│   └── home.axaml.cs
├── MainWindow/         # Main Application Window
│   ├── mainForm.axaml
│   └── mainForm.axaml.cs
├── Profile/            # หน้าโปรไฟล์ผู้ใช้
│   ├── profiles.axaml
│   └── profiles.axaml.cs
└── Sale/               # หน้าขาย
    ├── sale.axaml
    └── sale.axaml.cs
```

**หลักการตั้งชื่อ:**
- ใช้ **PascalCase** สำหรับชื่อโฟลเดอร์ (Auth, Branch, Common)
- ใช้ **camelCase** หรือ **PascalCase** สำหรับชื่อไฟล์ตาม convention ของคุณ

---

## 🔄 Data Flow (การไหลของข้อมูล)

```
User Input (View)
    ↓
Command/Event (View)
    ↓
ViewModel (Business Logic)
    ↓
Model/Service (Data Access)
    ↓
Database
    ↓
Model (Data)
    ↓
ViewModel (Process)
    ↓
View (Display)
```

---

## 📝 Naming Conventions

### Namespace
```csharp
My_program.ViewModels.Auth      // ViewModel
My_program.Views.Auth           // View
My_program.Core.Helpers         // Helper
My_program.Core.Config          // Config
My_program.Models               // Model
```

### Files
- **Views**: `login.axaml`, `login.axaml.cs`
- **ViewModels**: `LoginViewModel.cs`
- **Models**: `User.cs`, `Branch.cs`
- **Helpers**: `Encryptor.cs`, `ShowDialog.cs`

---

## 🎯 Best Practices

### ✅ DO:
1. **แยกความรับผิดชอบ**: View ทำหน้าที่แสดงผลเท่านั้น, ViewModel จัดการ logic
2. **ใช้ Data Binding**: ใช้ binding แทน code-behind เมื่อทำได้
3. **ใช้ Dependency Injection**: สำหรับ Services
4. **เขียน Unit Tests**: สำหรับ ViewModels และ Services
5. **ใช้ async/await**: สำหรับการทำงานแบบ asynchronous

### ❌ DON'T:
1. ไม่ควรใส่ Business Logic ใน Code-behind
2. ไม่ควรเข้าถึง Database โดยตรงใน ViewModel
3. ไม่ควรใช้ static classes มากเกินไป
4. ไม่ควรมี hard-coded values (ใช้ config แทน)

---

## 🚀 การเพิ่มฟีเจอร์ใหม่

### ตัวอย่าง: เพิ่มฟีเจอร์ "Customer Management"

1. **สร้าง Model**:
   ```
   Models/Customer.cs
   ```

2. **สร้าง ViewModel**:
   ```
   ViewModels/Customer/CustomerListViewModel.cs
   ViewModels/Customer/CustomerDetailViewModel.cs
   ```

3. **สร้าง View**:
   ```
   Views/Customer/CustomerList.axaml
   Views/Customer/CustomerList.axaml.cs
   Views/Customer/CustomerDetail.axaml
   Views/Customer/CustomerDetail.axaml.cs
   ```

4. **สร้าง Service (ถ้าจำเป็น)**:
   ```
   Core/Services/CustomerService.cs
   ```

---

## 📚 อ้างอิง

- [Avalonia UI Documentation](https://docs.avaloniaui.net/)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

---

**เอกสารนี้สร้างขึ้นเพื่อช่วยให้ทีมพัฒนาเข้าใจโครงสร้างและสามารถทำงานร่วมกันได้อย่างมีประสิทธิภาพ** 🎯
