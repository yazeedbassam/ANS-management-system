-- سكريبت استيراد البيانات من CSV إلى PostgreSQL
-- قم بتشغيل هذا السكريبت بعد إنشاء الجداول

-- تفعيل امتداد file_fdw (إذا لم يكن مفعلاً)
CREATE EXTENSION IF NOT EXISTS file_fdw;

-- استيراد بيانات المستخدمين
\copy ControllerUsers FROM 'ControllerUsers.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات التراخيص
\copy Licenses FROM 'Licenses.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات الشهادات
\copy Certificates FROM 'Certificates.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات الملاحظات
\copy Observations FROM 'Observations.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات المشاريع
\copy Projects FROM 'Projects.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات الدول
\copy Countries FROM 'Countries.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- استيراد بيانات المطارات
\copy Airports FROM 'Airports.csv' WITH (FORMAT csv, HEADER true, NULL '');

-- التحقق من البيانات
SELECT 'ControllerUsers' as table_name, COUNT(*) as count FROM ControllerUsers
UNION ALL
SELECT 'Licenses', COUNT(*) FROM Licenses
UNION ALL
SELECT 'Certificates', COUNT(*) FROM Certificates
UNION ALL
SELECT 'Observations', COUNT(*) FROM Observations
UNION ALL
SELECT 'Projects', COUNT(*) FROM Projects
UNION ALL
SELECT 'Countries', COUNT(*) FROM Countries
UNION ALL
SELECT 'Airports', COUNT(*) FROM Airports; 