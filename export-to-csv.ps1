# سكريبت تصدير البيانات من SQL Server إلى CSV
# قم بتشغيل هذا السكريبت في PowerShell

# إعدادات الاتصال
$server = "localhost\SQLEXPRESS"
$database = "ATCSM"
$outputPath = ".\exported-data\"

# إنشاء مجلد التصدير
if (!(Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
}

# دالة تصدير الجدول
function Export-TableToCSV {
    param($tableName, $query)
    
    Write-Host "تصدير جدول: $tableName"
    
    $csvPath = "$outputPath\$tableName.csv"
    
    # تصدير البيانات
    sqlcmd -S $server -d $database -Q $query -o $csvPath -s"," -W -h-1
    
    Write-Host "تم التصدير إلى: $csvPath"
}

# تصدير الجداول
Write-Host "بدء تصدير البيانات..."

# تصدير المستخدمين
Export-TableToCSV "ControllerUsers" "SELECT * FROM ControllerUsers"

# تصدير التراخيص
Export-TableToCSV "Licenses" "SELECT * FROM Licenses"

# تصدير الشهادات
Export-TableToCSV "Certificates" "SELECT * FROM Certificates"

# تصدير الملاحظات
Export-TableToCSV "Observations" "SELECT * FROM Observations"

# تصدير المشاريع
Export-TableToCSV "Projects" "SELECT * FROM Projects"

# تصدير الدول
Export-TableToCSV "Countries" "SELECT * FROM Countries"

# تصدير المطارات
Export-TableToCSV "Airports" "SELECT * FROM Airports"

Write-Host "تم الانتهاء من التصدير!"
Write-Host "الملفات موجودة في: $outputPath" 