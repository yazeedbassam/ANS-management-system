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
- **SQL Server**
- **Entity Framework Core**
- **Bootstrap 5**
- **Font Awesome**
- **jQuery**

## 📦 النشر على Railway

### الخطوة 1: إعداد GitHub
```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/username/repository-name.git
git push -u origin main
```

### الخطوة 2: إعداد Railway
1. اذهب إلى [Railway.app](https://railway.app)
2. سجل دخول بحساب GitHub
3. انقر على "New Project"
4. اختر "Deploy from GitHub repo"
5. اختر المستودع الخاص بك

### الخطوة 3: إعداد قاعدة البيانات
1. في Railway، انقر على "New"
2. اختر "Database" → "PostgreSQL"
3. انقر على قاعدة البيانات الجديدة
4. انسخ Connection String

### الخطوة 4: إعداد متغيرات البيئة
في Railway، أضف المتغيرات التالية:

```
DATABASE_URL=your_postgresql_connection_string
SMTP_SERVER=smtp-relay.brevo.com
SMTP_PORT=587
SMTP_USERNAME=your_smtp_username
SMTP_PASSWORD=your_smtp_password
RECEIVER_EMAIL=your_email@example.com
```

### الخطوة 5: النشر
1. Railway سيقوم تلقائياً بنشر التطبيق
2. انتظر حتى يكتمل البناء
3. انقر على الرابط المولّد للوصول للتطبيق

## 🔧 التطوير المحلي

### المتطلبات
- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 أو VS Code

### التشغيل
```bash
dotnet restore
dotnet build
dotnet run
```

## 📝 ملاحظات مهمة

1. **قاعدة البيانات**: يجب تعديل الكود ليدعم PostgreSQL بدلاً من SQL Server
2. **الملفات**: تأكد من إعداد مسارات الملفات بشكل صحيح
3. **الأمان**: تأكد من إعداد HTTPS في الإنتاج
4. **النسخ الاحتياطي**: قم بإعداد نسخ احتياطي لقاعدة البيانات

## 🆘 الدعم

للمساعدة أو الاستفسارات، يرجى التواصل عبر:
- Email: your-email@example.com
- GitHub Issues: [رابط المستودع]

## 📄 الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE). 