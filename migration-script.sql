-- سكريبت تحويل قاعدة البيانات من SQL Server إلى PostgreSQL
-- قم بتشغيل هذا السكريبت على قاعدة البيانات الجديدة

-- إنشاء الجداول الأساسية
CREATE TABLE IF NOT EXISTS ControllerUsers (
    ControllerId SERIAL PRIMARY KEY,
    Username VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(255),
    Job_Title VARCHAR(100),
    PhotoPath VARCHAR(500),
    Email VARCHAR(255),
    Phone_Number VARCHAR(50),
    Date_Of_Birth DATE,
    Marital_Status VARCHAR(50),
    Address TEXT,
    Hire_Date DATE,
    Employment_Status VARCHAR(50),
    Current_Department VARCHAR(100),
    Education_Level VARCHAR(100),
    Emergency_Contact VARCHAR(255),
    Role VARCHAR(50) DEFAULT 'Controller'
);

CREATE TABLE IF NOT EXISTS Countries (
    CountryId SERIAL PRIMARY KEY,
    CountryName VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS Airports (
    AirportId SERIAL PRIMARY KEY,
    AirportName VARCHAR(255) NOT NULL,
    AirportIcao VARCHAR(10),
    CountryId INTEGER REFERENCES Countries(CountryId)
);

CREATE TABLE IF NOT EXISTS Employees (
    EmployeeId SERIAL PRIMARY KEY,
    Username VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(255),
    Job_Title VARCHAR(100),
    PhotoPath VARCHAR(500),
    Email VARCHAR(255),
    Phone_Number VARCHAR(50),
    Date_Of_Birth DATE,
    Marital_Status VARCHAR(50),
    Address TEXT,
    Hire_Date DATE,
    Employment_Status VARCHAR(50),
    Current_Department VARCHAR(100),
    Education_Level VARCHAR(100),
    Emergency_Contact VARCHAR(255),
    Role VARCHAR(50) DEFAULT 'Employee'
);

CREATE TABLE IF NOT EXISTS Licenses (
    LicenseId SERIAL PRIMARY KEY,
    ControllerId INTEGER REFERENCES ControllerUsers(ControllerId),
    LicenseType VARCHAR(100),
    IssueDate DATE,
    ExpiryDate DATE,
    PdfPath VARCHAR(500),
    PhotoPath VARCHAR(500),
    Range VARCHAR(100),
    Note TEXT,
    FullName VARCHAR(255),
    Licensenumber VARCHAR(100),
    Username VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS Certificates (
    CertificateId SERIAL PRIMARY KEY,
    ControllerId INTEGER REFERENCES ControllerUsers(ControllerId),
    CertificateType VARCHAR(100),
    IssueDate DATE,
    ExpiryDate DATE,
    PdfPath VARCHAR(500),
    PhotoPath VARCHAR(500),
    Range VARCHAR(100),
    Note TEXT,
    FullName VARCHAR(255),
    Certificatenumber VARCHAR(100),
    Username VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS Observations (
    ObservationId SERIAL PRIMARY KEY,
    ControllerId INTEGER REFERENCES ControllerUsers(ControllerId),
    ObservationType VARCHAR(100),
    ObservationDate DATE,
    PdfPath VARCHAR(500),
    PhotoPath VARCHAR(500),
    Range VARCHAR(100),
    Note TEXT,
    FullName VARCHAR(255),
    Observationnumber VARCHAR(100),
    Username VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS Projects (
    ProjectId SERIAL PRIMARY KEY,
    ProjectName VARCHAR(255) NOT NULL,
    Description TEXT,
    StartDate DATE,
    EndDate DATE,
    Status VARCHAR(50),
    CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId SERIAL PRIMARY KEY,
    UserId INTEGER,
    ControllerId INTEGER,
    FullName VARCHAR(255),
    Message TEXT,
    Link VARCHAR(500),
    CreatedDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IsRead BOOLEAN DEFAULT FALSE,
    Note TEXT,
    LicenseType VARCHAR(100),
    ExpiryDate DATE,
    Phonenumber VARCHAR(50),
    Email VARCHAR(255),
    Currentdepartment VARCHAR(100),
    Location VARCHAR(255)
);

-- إنشاء مستخدم admin افتراضي
INSERT INTO ControllerUsers (Username, PasswordHash, FullName, Role) 
VALUES ('admin', '$2a$11$YourHashedPasswordHere', 'Administrator', 'Admin')
ON CONFLICT (Username) DO NOTHING;

-- إنشاء بعض البيانات الافتراضية
INSERT INTO Countries (CountryName) VALUES ('Jordan') ON CONFLICT DO NOTHING;
INSERT INTO Airports (AirportName, AirportIcao, CountryId) 
VALUES ('Queen Alia International Airport', 'OJAI', 1) ON CONFLICT DO NOTHING; 