# My-program

โปรเจกต์ Avalonia UI Application สำหรับจัดการระบบ (Branch Management System)

## 📋 ข้อกำหนดของระบบ (System Requirements)

ก่อนเริ่มใช้งานโปรเจกต์นี้ คุณต้องติดตั้งสิ่งเหล่านี้ก่อน:

### 1. .NET SDK 9.0 หรือสูงกว่า
- **ดาวน์โหลด**: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
- **ตรวจสอบเวอร์ชัน**:
  ```bash
  dotnet --version
  ```
  ควรแสดงผล `9.0.x` หรือสูงกว่า

### 2. MySQL Server
- **ดาวน์โหลด**: [https://dev.mysql.com/downloads/mysql/](https://dev.mysql.com/downloads/mysql/)
- หรือใช้ **XAMPP**, **MAMP**, **phpMyAdmin** แทนก็ได้
- โปรเจกต์นี้ใช้ `MySql.Data` package เวอร์ชัน 9.5.0

### 3. IDE (ตัวเลือก แต่แนะนำ)
- **Visual Studio 2022** (Windows/Mac) - [Download](https://visualstudio.microsoft.com/)
- **Visual Studio Code** - [Download](https://code.visualstudio.com/)
- **JetBrains Rider** - [Download](https://www.jetbrains.com/rider/)

### 4. Git
- **ดาวน์โหลด**: [https://git-scm.com/downloads](https://git-scm.com/downloads)

---

## 🚀 การติดตั้งและรันโปรเจกต์

### ขั้นตอนที่ 1: Clone โปรเจกต์
```bash
git clone https://github.com/Larkeo-lab/window-server.git
cd window-server
```

### ขั้นตอนที่ 2: Restore Dependencies
โปรเจกต์นี้ใช้ NuGet packages หลายตัว ให้รัน:
```bash
dotnet restore
```

คำสั่งนี้จะดาวน์โหลด packages ทั้งหมดที่จำเป็น:
- Avalonia 11.3.6 (UI Framework)
- Avalonia.Controls.DataGrid
- Avalonia.Desktop
- Avalonia.Themes.Fluent
- CommunityToolkit.Mvvm 8.2.1
- MySql.Data 9.5.0

### ขั้นตอนที่ 3: ตั้งค่า Database
1. สร้างฐานข้อมูล MySQL
2. แก้ไข connection string ในไฟล์ `Views/config/db.cs` หรือไฟล์ที่เกี่ยวข้อง
3. Import schema หรือสร้างตารางที่จำเป็น

### ขั้นตอนที่ 4: Build โปรเจกต์
```bash
dotnet build
```

### ขั้นตอนที่ 5: รันโปรเจกต์
```bash
dotnet run
```

หรือถ้าคุณใช้ IDE ก็สามารถกด **Run** หรือ **F5** ได้เลย

---

## 📁 โครงสร้างโปรเจกต์

โปรเจกต์นี้ใช้สถาปัตยกรรม **MVVM (Model-View-ViewModel)** แบบมาตรฐาน:

```
My-program/
├── Assets/                  # ทรัพยากรทั้งหมด (รูปภาพ, ไอคอน, ฟอนต์)
│   ├── Fonts/              # ฟอนต์
│   ├── Icons/              # ไอคอนต่างๆ
│   └── Images/             # รูปภาพและกราฟิก
│
├── Core/                    # Core functionality และ business logic
│   ├── Config/             # การตั้งค่า (Database connection, etc.)
│   ├── Helpers/            # Helper classes (Encryption, Dialog, Formatter)
│   └── Services/           # Business logic services
│
├── Models/                  # Data models และ entities
│
├── ViewModels/             # MVVM ViewModels
│   ├── Auth/               # ViewModels สำหรับ Authentication
│   ├── Branch/             # ViewModels สำหรับการจัดการสาขา
│   ├── Home/               # ViewModels สำหรับหน้าหลัก
│   ├── Profile/            # ViewModels สำหรับโปรไฟล์
│   ├── Sale/               # ViewModels สำหรับการขาย
│   ├── MainWindowViewModel.cs
│   └── ViewModelBase.cs
│
├── Views/                   # UI Views (AXAML files เท่านั้น)
│   ├── Auth/               # หน้า Login/Authentication
│   ├── Branch/             # หน้าจัดการสาขา
│   ├── Common/             # Components ที่ใช้ร่วมกัน (Navbar, etc.)
│   ├── Home/               # หน้าหลัก
│   ├── MainWindow/         # Main Window
│   ├── Profile/            # หน้าโปรไฟล์
│   └── Sale/               # หน้าขาย
│
├── App.axaml               # Application configuration
├── App.axaml.cs            # Application code-behind
├── Program.cs              # Entry point
├── ViewLocator.cs          # ViewModel to View mapping
└── My-program.csproj       # Project file
```

### 🎯 คำอธิบายโครงสร้าง

- **Assets/**: เก็บทรัพยากรทั้งหมด (ไฟล์รูป ไอคอน ฟอนต์)
- **Core/**: เก็บ business logic, helpers, และ configuration
- **Models/**: เก็บ data models และ entities
- **ViewModels/**: เก็บ MVVM ViewModels (แยกตาม feature)
- **Views/**: เก็บ UI Views เท่านั้น (ไฟล์ AXAML และ code-behind)

---

## 🛠️ Technologies Used

- **Framework**: .NET 9.0
- **UI Framework**: Avalonia UI 11.3.6
- **MVVM**: CommunityToolkit.Mvvm 8.2.1
- **Database**: MySQL (MySql.Data 9.5.0)
- **Architecture**: MVVM (Model-View-ViewModel)

---

## 📝 คำแนะนำเพิ่มเติม

### การ Debug
```bash
dotnet run --configuration Debug
```

### การ Build สำหรับ Production
```bash
dotnet build --configuration Release
dotnet publish -c Release -r win-x64 --self-contained
```
*เปลี่ยน `win-x64` เป็น `osx-x64` (Mac) หรือ `linux-x64` (Linux) ตามระบบปฏิบัติการ*

### การแก้ไขปัญหาทั่วไป

**ปัญหา: `dotnet: command not found`**
- ตรวจสอบว่าติดตั้ง .NET SDK แล้ว
- เพิ่ม .NET SDK เข้า PATH

**ปัญหา: Connection to MySQL failed**
- ตรวจสอบว่า MySQL Server ทำงานอยู่
- ตรวจสอบ connection string ใน `Views/config/db.cs`
- ตรวจสอบ username, password, และชื่อฐานข้อมูล

**ปัญหา: Missing packages**
```bash
dotnet restore --force
dotnet build
```

---

## 🤝 การมีส่วนร่วม

หากต้องการมีส่วนร่วมในโปรเจกต์:
1. Fork โปรเจกต์
2. สร้าง feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit การเปลี่ยนแปลง (`git commit -m 'Add some AmazingFeature'`)
4. Push ไปยัง branch (`git push origin feature/AmazingFeature`)
5. เปิด Pull Request

---

## 📞 ติดต่อ

หากมีคำถามหรือปัญหา กรุณาติดต่อผ่าน GitHub Issues

---

## 📄 License

โปรเจกต์นี้สร้างขึ้นเพื่อการศึกษา
