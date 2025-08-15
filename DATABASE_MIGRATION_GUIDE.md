# دليل رفع قاعدة البيانات من SQL Server إلى PostgreSQL

## 🗄️ الخطوة 1: إنشاء قاعدة البيانات في Railway

### 1. اذهب إلى Railway
- افتح [Railway.app](https://railway.app)
- اذهب إلى مشروعك `ANS-management-system`

### 2. إنشاء قاعدة البيانات
- انقر على **"New"**
- اختر **"Database"** → **"PostgreSQL"**
- انتظر حتى يتم إنشاء قاعدة البيانات

### 3. الحصول على Connection String
- انقر على قاعدة البيانات الجديدة
- انسخ **Connection String** (سيبدو مثل):
  ```
  postgresql://postgres:password@host:port/database
  ```

## 📊 الخطوة 2: تصدير البيانات من SQL Server

### الطريقة الأولى: استخدام SQL Server Management Studio
1. افتح **SQL Server Management Studio**
2. اتصل بقاعدة البيانات المحلية
3. انسخ محتوى ملف `export-sqlserver-data.sql`
4. شغله على قاعدة البيانات المحلية
5. انسخ النتائج (INSERT statements)

### الطريقة الثانية: استخدام PowerShell
```powershell
# تصدير البيانات إلى ملف
sqlcmd -S "localhost\SQLEXPRESS" -d "ATCSM" -i "export-sqlserver-data.sql" -o "exported-data.sql"
```

## 🔄 الخطوة 3: إنشاء الجداول في PostgreSQL

### الطريقة الأولى: استخدام Railway Dashboard
1. في Railway، انقر على قاعدة البيانات
2. انقر على **"Query"** أو **"SQL Editor"**
3. انسخ محتوى ملف `migration-script.sql`
4. شغله

### الطريقة الثانية: استخدام أداة خارجية
1. استخدم **pgAdmin** أو **DBeaver**
2. اتصل بقاعدة البيانات PostgreSQL
3. شغل سكريبت `migration-script.sql`

## 📥 الخطوة 4: رفع البيانات

### 1. إدخال البيانات المصدرة
- انسخ INSERT statements من الخطوة 2
- شغلها على قاعدة البيانات PostgreSQL

### 2. التحقق من البيانات
```sql
-- التحقق من عدد المستخدمين
SELECT COUNT(*) FROM ControllerUsers;

-- التحقق من عدد التراخيص
SELECT COUNT(*) FROM Licenses;

-- التحقق من عدد الشهادات
SELECT COUNT(*) FROM Certificates;
```

## ⚙️ الخطوة 5: إعداد متغيرات البيئة

### في Railway، أضف المتغيرات التالية:
```
DATABASE_URL=your_postgresql_connection_string
SMTP_SERVER=smtp-relay.brevo.com
SMTP_PORT=587
SMTP_USERNAME=8e2caf001@smtp-brevo.com
SMTP_PASSWORD=3HzgVG7nwKMxqcA2
RECEIVER_EMAIL=yazeedbassam1987@gmail.com
```

## 🔧 الخطوة 6: تحديث الكود لدعم PostgreSQL

### 1. تحديث connection string
- تأكد من أن `DATABASE_URL` يحتوي على connection string الصحيح

### 2. إعادة نشر التطبيق
- Railway سيكتشف التغييرات تلقائياً
- أو ارفع تحديث على GitHub

## ✅ التحقق من النجاح

### 1. اختبار التطبيق
- افتح رابط التطبيق
- جرب تسجيل الدخول بـ `admin` / `123`

### 2. التحقق من البيانات
- تأكد من ظهور المستخدمين
- تأكد من ظهور التراخيص والشهادات

## 🆘 حل المشاكل

### مشكلة: لا يمكن الاتصال بقاعدة البيانات
- تأكد من صحة connection string
- تأكد من أن قاعدة البيانات نشطة

### مشكلة: البيانات لا تظهر
- تأكد من تشغيل سكريبت إنشاء الجداول
- تأكد من رفع البيانات بشكل صحيح

### مشكلة: أخطاء في التطبيق
- راجع سجلات Railway
- تأكد من صحة متغيرات البيئة

## 📞 المساعدة
إذا واجهت أي مشاكل، يمكنك:
1. مراجعة سجلات Railway
2. التحقق من connection string
3. التأكد من صحة البيانات المصدرة 