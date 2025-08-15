-- سكريبت تصدير البيانات من SQL Server
-- قم بتشغيل هذا السكريبت على SQL Server المحلي

-- تصدير بيانات المستخدمين
SELECT 
    'INSERT INTO ControllerUsers (Username, PasswordHash, FullName, Job_Title, PhotoPath, Email, Phone_Number, Date_Of_Birth, Marital_Status, Address, Hire_Date, Employment_Status, Current_Department, Education_Level, Emergency_Contact, Role) VALUES (' +
    '''' + ISNULL(Username, '') + ''', ' +
    '''' + ISNULL(PasswordHash, '') + ''', ' +
    '''' + ISNULL(FullName, '') + ''', ' +
    '''' + ISNULL(Job_Title, '') + ''', ' +
    '''' + ISNULL(PhotoPath, '') + ''', ' +
    '''' + ISNULL(Email, '') + ''', ' +
    '''' + ISNULL(Phone_Number, '') + ''', ' +
    CASE WHEN Date_Of_Birth IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, Date_Of_Birth, 23) + '''' END + ', ' +
    '''' + ISNULL(Marital_Status, '') + ''', ' +
    '''' + ISNULL(Address, '') + ''', ' +
    CASE WHEN Hire_Date IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, Hire_Date, 23) + '''' END + ', ' +
    '''' + ISNULL(Employment_Status, '') + ''', ' +
    '''' + ISNULL(Current_Department, '') + ''', ' +
    '''' + ISNULL(Education_Level, '') + ''', ' +
    '''' + ISNULL(Emergency_Contact, '') + ''', ' +
    '''' + ISNULL(Role, 'Controller') + ''');'
FROM ControllerUsers;

-- تصدير بيانات التراخيص
SELECT 
    'INSERT INTO Licenses (ControllerId, LicenseType, IssueDate, ExpiryDate, PdfPath, PhotoPath, Range, Note, FullName, Licensenumber, Username) VALUES (' +
    CAST(ControllerId AS VARCHAR) + ', ' +
    '''' + ISNULL(LicenseType, '') + ''', ' +
    CASE WHEN IssueDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, IssueDate, 23) + '''' END + ', ' +
    CASE WHEN ExpiryDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, ExpiryDate, 23) + '''' END + ', ' +
    '''' + ISNULL(PdfPath, '') + ''', ' +
    '''' + ISNULL(PhotoPath, '') + ''', ' +
    '''' + ISNULL(Range, '') + ''', ' +
    '''' + ISNULL(Note, '') + ''', ' +
    '''' + ISNULL(FullName, '') + ''', ' +
    '''' + ISNULL(Licensenumber, '') + ''', ' +
    '''' + ISNULL(Username, '') + ''');'
FROM Licenses;

-- تصدير بيانات الشهادات
SELECT 
    'INSERT INTO Certificates (ControllerId, CertificateType, IssueDate, ExpiryDate, PdfPath, PhotoPath, Range, Note, FullName, Certificatenumber, Username) VALUES (' +
    CAST(ControllerId AS VARCHAR) + ', ' +
    '''' + ISNULL(CertificateType, '') + ''', ' +
    CASE WHEN IssueDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, IssueDate, 23) + '''' END + ', ' +
    CASE WHEN ExpiryDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, ExpiryDate, 23) + '''' END + ', ' +
    '''' + ISNULL(PdfPath, '') + ''', ' +
    '''' + ISNULL(PhotoPath, '') + ''', ' +
    '''' + ISNULL(Range, '') + ''', ' +
    '''' + ISNULL(Note, '') + ''', ' +
    '''' + ISNULL(FullName, '') + ''', ' +
    '''' + ISNULL(Certificatenumber, '') + ''', ' +
    '''' + ISNULL(Username, '') + ''');'
FROM Certificates;

-- تصدير بيانات الملاحظات
SELECT 
    'INSERT INTO Observations (ControllerId, ObservationType, ObservationDate, PdfPath, PhotoPath, Range, Note, FullName, Observationnumber, Username) VALUES (' +
    CAST(ControllerId AS VARCHAR) + ', ' +
    '''' + ISNULL(ObservationType, '') + ''', ' +
    CASE WHEN ObservationDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, ObservationDate, 23) + '''' END + ', ' +
    '''' + ISNULL(PdfPath, '') + ''', ' +
    '''' + ISNULL(PhotoPath, '') + ''', ' +
    '''' + ISNULL(Range, '') + ''', ' +
    '''' + ISNULL(Note, '') + ''', ' +
    '''' + ISNULL(FullName, '') + ''', ' +
    '''' + ISNULL(Observationnumber, '') + ''', ' +
    '''' + ISNULL(Username, '') + ''');'
FROM Observations;

-- تصدير بيانات المشاريع
SELECT 
    'INSERT INTO Projects (ProjectName, Description, StartDate, EndDate, Status) VALUES (' +
    '''' + ISNULL(ProjectName, '') + ''', ' +
    '''' + ISNULL(Description, '') + ''', ' +
    CASE WHEN StartDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, StartDate, 23) + '''' END + ', ' +
    CASE WHEN EndDate IS NULL THEN 'NULL' ELSE '''' + CONVERT(VARCHAR, EndDate, 23) + '''' END + ', ' +
    '''' + ISNULL(Status, 'Active') + ''');'
FROM Projects; 