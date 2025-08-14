# Air Navigation Services Management System

نظام إدارة خدمات الملاحة الجوية - تطبيق ASP.NET Core MVC لإدارة المراقبين الجويين والتراخيص والشهادات.

## 🚀 الميزات

- ✅ نظام تسجيل دخول وإدارة مستخدمين
- ✅ إدارة المراقبين الجويين والموظفين
- ✅ إدارة التراخيص والشهادات
- ✅ نظام الملاحظات والمراقبة
- ✅ إدارة المشاريع والدورات التدريبية
- ✅ نظام إشعارات متقدم
- ✅ واجهة مستخدم حديثة

## 🛠️ التقنيات المستخدمة

- **ASP.NET Core 8.0**
- **PostgreSQL** (للنشر على Railway)
- **Entity Framework Core**
- **Bootstrap 5**
- **Font Awesome**
- **jQuery**

## 📦 النشر على Railway (مجاني)

### الخطوة 1: إعداد GitHub
```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/username/atc-management-system.git
git push -u origin main
```

### الخطوة 2: إنشاء مستودع GitHub
1. اذهب إلى [GitHub.com](https://github.com)
2. انقر على "New repository"
3. اسم المستودع: `atc-management-system`
4. اختر Public
5. انقر "Create repository"

### الخطوة 3: إعداد Railway
1. اذهب إلى [Railway.app](https://railway.app)
2. انقر على "Start a New Project"
3. اختر "Deploy from GitHub repo"
4. اختر المستودع الخاص بك
5. انقر "Deploy Now"

### الخطوة 4: إعداد قاعدة البيانات
1. في Railway، انقر على "New"
2. اختر "Database" → "PostgreSQL"
3. انقر على قاعدة البيانات الجديدة
4. انسخ Connection String

### الخطوة 5: إعداد متغيرات البيئة
في Railway، أضف المتغيرات التالية:

```
DATABASE_URL=your_postgresql_connection_string
SMTP_SERVER=smtp-relay.brevo.com
SMTP_PORT=587
SMTP_USERNAME=8e2caf001@smtp-brevo.com
SMTP_PASSWORD=3HzgVG7nwKMxqcA2
RECEIVER_EMAIL=yazeedbassam1987@gmail.com
```

### الخطوة 6: النشر
1. Railway سيقوم تلقائياً بنشر التطبيق
2. انتظر حتى يكتمل البناء (قد يستغرق 5-10 دقائق)
3. انقر على الرابط المولّد للوصول للتطبيق

## 🔧 التطوير المحلي

### المتطلبات
- .NET 8.0 SDK
- SQL Server (للتطوير المحلي)
- Visual Studio 2022 أو VS Code

### التشغيل
```bash
dotnet restore
dotnet build
dotnet run
```

## 📝 ملاحظات مهمة

1. **قاعدة البيانات**: المشروع يدعم PostgreSQL في الإنتاج و SQL Server في التطوير
2. **الملفات**: تأكد من إعداد مسارات الملفات بشكل صحيح
3. **الأمان**: تأكد من إعداد HTTPS في الإنتاج
4. **النسخ الاحتياطي**: قم بإعداد نسخ احتياطي لقاعدة البيانات

## 🆘 الدعم

للمساعدة أو الاستفسارات، يرجى التواصل عبر:
- Email: yazeedbassam1987@gmail.com
- GitHub Issues: [رابط المستودع]

## 📄 الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE). 