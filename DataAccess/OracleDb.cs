using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication1.DataAccess
{
    public class OracleDb
    {
        private readonly string _connectionString;
        public string ConnectionString => _connectionString; // إضافة هذه الخاصية

        public OracleDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        // تعريف الاتصال
        public OracleConnection GetConnection()
            => new OracleConnection(_connectionString);

        // دالة لجلب البيانات (Select)
        public DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            using var adapter = new OracleDataAdapter(cmd);
            var dt = new DataTable();
            //adapter.Fill(dt);
            try
            {
                adapter.Fill(dt);
            }
            catch (OracleException ex)
            {
                Console.WriteLine($"Oracle Error: {ex.Message}"); // Log the error for debugging
                throw;
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        // دالة لتنفيذ استعلام وإرجاع قيمة مفردة
        public object ExecuteScalar(string sql, params OracleParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteScalar();
        }

        // دالة لتنفيذ أوامر Insert/Update/Delete مع دعم المعلمات
        public int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }
        //************************************************
        // ... يمكنك إضافة دوال أخرى هنا مثل GetController, CreateController, UpdateController
        // 1) جلب جميع المراقبين (بما فيها الحقول الجديدة)
        public DataTable GetAllControllers()
        {
            const string sql = @"
SELECT 
  c.controllerid,
  c.fullname,
  c.username,
  c.airportid,
  c.photopath,
  c.licensepath,
  c.job_title,
  c.education_level,
  c.date_of_birth,
  c.marital_status,
  c.phone_number,
  c.email,
  c.address,
  c.hire_date,
  c.employment_status,
  c.current_department,
  c.transfer_date,
  c.emergency_contact,
  a.airportname,
  a.icao_code
FROM controllers c
JOIN airports    a ON c.airportid = a.airportid";
            return ExecuteQuery(sql);
        }

        // 2) جلب مراقب واحد بالمعرّف
        public DataTable GetControllerById(int controllerId)
        {
            const string sql = @"
SELECT 
  c.controllerid,
  c.fullname,
  c.username,
  c.airportid,
  c.photopath,
  c.licensepath,
  c.job_title,
  c.education_level,
  c.date_of_birth,
  c.marital_status,
  c.phone_number,
  c.email,
  c.address,
  c.hire_date,
  c.employment_status,
  c.current_department,
  c.transfer_date,
  c.emergency_contact
FROM controllers c
WHERE c.controllerid = :ctrlId";
            return ExecuteQuery(sql, new OracleParameter("ctrlId", controllerId));
        }
        public DataTable GetObservationById(int ObservationId)
        {
            const string sql = @"
SELECT
    o.observationid,
    o.controllerid,
    o.observationno,
    o.travelcount,
    o.duration_days,
    o.travelcountry,
    o.departdate,
    o.returndate,
    o.licensenumber,
    o.filepath,
    o.notes,
    c.fullname AS controllername -- جلب اسم المراقب وتسميته controllername
FROM observations o
JOIN controllers c ON o.controllerid = c.controllerid
WHERE o.observationid = :ObservationId";
            return ExecuteQuery(sql, new OracleParameter("ObservationId", ObservationId));
        }
        public int UpdateController(Models.ControllerUser u)
        {
            const string sql = @"
UPDATE controllers SET
  fullname           = :fullname,
  username           = :username,
  airportid          = :airportid,
  photopath          = :photopath,
  licensepath        = :licensepath,
  job_title          = :job_title,
  education_level    = :education_level,
  date_of_birth      = :date_of_birth,
  marital_status     = :marital_status,
  phone_number       = :phone_number,
  email              = :email,
  address            = :address,
  hire_date          = :hire_date,
  employment_status  = :employment_status,
  current_department = :current_department,
  transfer_date      = :transfer_date,
  emergency_contact  = :emergency_contact
WHERE controllerid = :controllerid";
            return ExecuteNonQuery(sql,
                new OracleParameter("fullname", u.FullName),
                new OracleParameter("username", u.Username),
                new OracleParameter("airportid", u.AirportId),
                new OracleParameter("photopath", (object?)u.PhotoPath ?? DBNull.Value),
                new OracleParameter("licensepath", (object?)u.LicensePath ?? DBNull.Value),
                new OracleParameter("job_title", (object?)u.JobTitle ?? DBNull.Value),
                new OracleParameter("education_level", (object?)u.EducationLevel ?? DBNull.Value),
                new OracleParameter("date_of_birth", (object?)u.DateOfBirth ?? DBNull.Value),
                new OracleParameter("marital_status", (object?)u.MaritalStatus ?? DBNull.Value),
                new OracleParameter("phone_number", (object?)u.PhoneNumber ?? DBNull.Value),
                new OracleParameter("email", (object?)u.Email ?? DBNull.Value),
                new OracleParameter("address", (object?)u.Address ?? DBNull.Value),
                new OracleParameter("hire_date", (object?)u.HireDate ?? DBNull.Value),
                new OracleParameter("employment_status", (object?)u.EmploymentStatus ?? DBNull.Value),
                new OracleParameter("current_department", (object?)u.CurrentDepartment ?? DBNull.Value),
                new OracleParameter("transfer_date", (object?)u.TransferDate ?? DBNull.Value),
                new OracleParameter("emergency_contact", (object?)u.EmergencyContact ?? DBNull.Value),
                new OracleParameter("controllerid", u.ControllerId)
            );
        }



        //***********************************************
        /// <summary>
        /// يعيد عدد الرخص المرتبطة بمراقب جوي محدد.
        /// </summary>
        public int GetLicenseCountByController(int controllerId)
        {
            var result = ExecuteScalar(
                "SELECT COUNT(*) FROM licenses WHERE controllerid = :controllerId",
                new OracleParameter("controllerId", controllerId)
            );
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// يحذف جميع الرخص المرتبطة ثم يحذف سجل المراقب.
        /// الملفات تبقى مخزنة على القرص.
        /// </summary>
        public void DeleteController(int controllerId)
        {
            using var conn = GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // حذف الرخص أولاً
            using var cmd1 = new OracleCommand(
                "DELETE FROM licenses WHERE controllerid = :controllerId", conn);
            cmd1.Parameters.Add(new OracleParameter("controllerId", controllerId));
            cmd1.ExecuteNonQuery();

            // ثم حذف المراقب
            using var cmd2 = new OracleCommand(
                "DELETE FROM controllers WHERE controllerid = :controllerId", conn);
            cmd2.Parameters.Add(new OracleParameter("controllerId", controllerId));
            cmd2.ExecuteNonQuery();

            tx.Commit();
        }


        /// <summary>
        /// يعيد عدد المراقبين الجويين المرتبطين بمطار معيّن.
        /// </summary>
        public int GetControllerCountByAirport(int airportId)
        {
            // ننفّذ استعلام عدّ المراقبين الذين لديهم هذا الـairportId
            var result = ExecuteScalar(
                "SELECT COUNT(*) FROM controllers WHERE airportid = :id",
                new OracleParameter("id", airportId)
            );
            return Convert.ToInt32(result);
        }

        public (int userId, string username, string passwordHash, string role)? GetUserByUsername(string username)
        {
            var dt = ExecuteQuery(
                "SELECT userid, username, passwordhash, rolename FROM users WHERE username = :u",
                new OracleParameter("u", username));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return (
                Convert.ToInt32(r["userid"]),
                r["username"].ToString(),
                r["passwordhash"].ToString(),
                r["rolename"].ToString()
            );
        }

        // 4.2 إضافة مستخدم جديد (مثلاً Admin أولي)
        private readonly IPasswordHasher<ControllerUser> _hasher = new PasswordHasher<ControllerUser>();

        public void CreateUser(string username, string password, string role)
        {
            // هَنّي بنعمل هَش للباسوورد
            var pwdHash = _hasher.HashPassword(null, password);

            const string sql = @"
      INSERT INTO users (userid, username, passwordhash, rolename)
      VALUES (USERS_SEQ.NEXTVAL, :u, :p, :r)";
            ExecuteNonQuery(sql,
                new OracleParameter("u", username),
                new OracleParameter("p", pwdHash),
                new OracleParameter("r", role)
            );
        }


        // 4.3 فحص كلمة المرور
        public bool ValidateCredentials(string username, string password, out int userId, out string role)
        {
            userId = 0;
            role = null;

            var user = GetUserByUsername(username);
            if (user == null)
                return false;

            // افتراضاً GetUserByUsername يرجع (id, uname, pwHash, rolename)
            var (id, uname, pwHash, rl) = user.Value;

            var result = _hasher.VerifyHashedPassword(
                /*user:*/           null,
                /*hashedPassword:*/ pwHash,
                /*providedPassword:*/ password
            );

            if (result == PasswordVerificationResult.Success)
            {
                userId = id;
                role = rl;
                return true;
            }
            return false;
        }



        //******************************Certificate
        // ——— Certificates ———

        // جلب كل الشهادات مع أسماء المراقبين وأنواع الوثائق
        public DataTable GetAllCertificates()
        {
            const string sql = @"
      SELECT cert.certificateid,
             cert.controllerid,
             ctrl.fullname AS controllername,
             cert.typeid,
             dt.typename    AS typename,
             cert.title,
             cert.issuingauthority,
             cert.issuingcountry,
             cert.issuedate,
             cert.expirydate,
             cert.status,
             cert.statusreason,
             cert.filepath,
             cert.notes
        FROM certificates cert
        JOIN controllers   ctrl ON cert.controllerid = ctrl.controllerid
        JOIN documenttypes dt   ON cert.typeid       = dt.typeid
       ORDER BY cert.issuedate DESC";
            return ExecuteQuery(sql);
        }

        public DataTable GetCertificatesByController(int controllerId)
        {
            const string sql = @"
      SELECT certificateid,
             controllerid,
             typeid,
             title,
             issuingauthority,
             issuingcountry,
             issuedate,
             expirydate,
             status,
             statusreason,
             filepath,
             notes
        FROM certificates
       WHERE controllerid = :controllerid
       ORDER BY issuedate DESC";
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter("controllerid", controllerId));
            using var adapter = new OracleDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public int DeleteCertificate(int certificateId)
        {
            const string sql = "DELETE FROM certificates WHERE certificateid = :certId";
            return ExecuteNonQuery(sql, new OracleParameter("certId", certificateId));
        }

        // ——— Observations ———

        public DataTable GetAllObservations()
        {
            const string sql = @"
      SELECT obs.observationid,
             obs.controllerid,
             ctrl.fullname   AS controllername,
             obs.observationno,
             obs.travelcount,
             obs.duration_days,
             obs.travelcountry,
             obs.departdate,
             obs.returndate,
             obs.licensenumber,
             obs.filepath,
             obs.notes
        FROM observations obs
        JOIN controllers  ctrl ON obs.controllerid = ctrl.controllerid
       ORDER BY obs.departdate DESC";
            return ExecuteQuery(sql);
        }

        public DataTable GetAllObservationsbyDashboard()
        {
            const string sql = "SELECT * FROM observations"; // أو أي استعلام تريده
            return ExecuteQuery(sql);
        }

        public DataTable GetObservationsByController(int controllerId)
        {
            const string sql = @"
      SELECT observationid,
             controllerid,
             observationno,
             travelcount,
             duration_days,
             travelcountry,
             departdate,
             returndate,
             licensenumber,
             filepath,
             notes
        FROM observations
       WHERE controllerid = :controllerid
       ORDER BY departdate DESC";
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new OracleCommand(sql, conn);
            cmd.Parameters.Add(new OracleParameter("controllerid", controllerId));
            using var adapter = new OracleDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public int CreateObservation(Observation obs)
        {
            const string sql = @"
      INSERT INTO observations
        (observationid, controllerid, observationno,
         travelcount, duration_days,
         travelcountry, departdate, returndate,
         licensenumber, filepath, notes)
      VALUES
        (observations_seq.NEXTVAL, :ctrlId, :obsNo,
         :travelCount, :duration,
         :country, :depart, :return,
         :licenseNo, :path, :notes)";
            return ExecuteNonQuery(sql,
                new OracleParameter("ctrlId", obs.ControllerId),
                new OracleParameter("obsNo", obs.ObservationNo),
                new OracleParameter("travelCount", obs.TravelCount),
                new OracleParameter("duration", obs.DurationDays),
                new OracleParameter("country", obs.TravelCountry),
                new OracleParameter("depart", obs.DepartDate),
                new OracleParameter("return", obs.ReturnDate),
                new OracleParameter("licenseNo", obs.LicenseNumber),
                new OracleParameter("path", obs.FilePath ?? (object)DBNull.Value),
                new OracleParameter("notes", obs.Notes ?? (object)DBNull.Value)
            );
        }

        public int UpdateObservation(Observation obs)
        {
            const string sql = @"
      UPDATE observations
         SET 
             duration_days   = :duration,
             travelcountry   = :country,
             departdate      = :depart,
             returndate      = :return,
             licensenumber   = :licenseNo,
             filepath        = :path,
             notes           = :notes
       WHERE observationid   = :obsId";
            return ExecuteNonQuery(sql,
                // new OracleParameter("obsNo", obs.ObservationNo),
                //new OracleParameter("travelCount", obs.TravelCount),
                new OracleParameter("duration", obs.DurationDays),
                new OracleParameter("country", obs.TravelCountry),
                new OracleParameter("depart", obs.DepartDate),
                new OracleParameter("return", obs.ReturnDate),
                new OracleParameter("licenseNo", obs.LicenseNumber),
                new OracleParameter("path", obs.FilePath ?? (object)DBNull.Value),
                new OracleParameter("notes", obs.Notes ?? (object)DBNull.Value),
                new OracleParameter("obsId", obs.ObservationId)
            );
        }

        public int DeleteObservation(int observationId)
        {
            const string sql = "DELETE FROM observations WHERE observationid = :obsId";
            return ExecuteNonQuery(sql, new OracleParameter("obsId", observationId));
        }

        public void CreateCertificate(Certificate cert)
        {
            const string sql = @"
      INSERT INTO certificates
        (certificateid, controllerid, typeid, certificatetitle,
         issuingauthority, issuingcountry, issuedate, expirydate,
         status, statusreason, filepath, notes)
      VALUES
        (certificates_seq.NEXTVAL, :controllerid, :typeId, :certificatetitle,
         :issuingauthority, :issuingcountry, :issueDate, :expiryDate,
         :status, :statusReason, :filePath, :notes)";

            ExecuteNonQuery(sql,
                new OracleParameter("controllerid", cert.ControllerId),
                new OracleParameter("typeId", cert.TypeId),
                new OracleParameter("certificatetitle", cert.CertificateTitle),
                new OracleParameter("issuingauthority", cert.IssuingAuthority ?? (object)DBNull.Value),
                new OracleParameter("issuingcountry", cert.IssuingCountry ?? (object)DBNull.Value),
                new OracleParameter("issueDate", cert.IssueDate),
                new OracleParameter("expiryDate", cert.ExpiryDate),
                new OracleParameter("status", cert.Status),
                new OracleParameter("statusReason", cert.StatusReason ?? (object)DBNull.Value),
                 new OracleParameter("filePath", cert.FilePath ?? (object)DBNull.Value),
        new OracleParameter("notes", cert.Notes ?? (object)DBNull.Value)
            );
        }


        public void UpdateCertificate(Certificate c)
        {
            const string sql = @"
        UPDATE certificates SET
            controllerid      = :controllerId,
            typeid            = :typeId,
            certificatetitle  = :title,
            issuingauthority  = :issuingAuthority,
            issuingcountry    = :issuingCountry,
            issuedate         = :issueDate,
            expirydate        = :expiryDate,
            status            = :status,
            statusreason      = :statusReason,
            filepath          = :filePath,
            notes             = :notes
        WHERE certificateid = :certificateId";
            System.Diagnostics.Debug.WriteLine("-- UpdateCertificate SQL --\n" + sql);

            ExecuteNonQuery(sql,
    new OracleParameter("controllerId", c.ControllerId),
    new OracleParameter("typeId", c.TypeId),
    new OracleParameter("title", c.CertificateTitle),
    new OracleParameter("issuingAuthority", (object?)c.IssuingAuthority ?? DBNull.Value),
    new OracleParameter("issuingCountry", (object?)c.IssuingCountry ?? DBNull.Value),
    new OracleParameter("issueDate", c.IssueDate),
    new OracleParameter("expiryDate", c.ExpiryDate),
    new OracleParameter("status", c.Status),
    new OracleParameter("statusReason", (object?)c.StatusReason ?? DBNull.Value),
    new OracleParameter("filePath", (object?)c.FilePath ?? DBNull.Value),
    new OracleParameter("notes", (object?)c.Notes ?? DBNull.Value),
    new OracleParameter("certificateId", c.CertificateId)
);
        }

        /// <summary>
        /// يجلب سجل شهادة واحد بناءً على المعرف
        /// </summary>
        public DataTable GetCertificateById(int certificateId)
        {
            const string sql = @"
      SELECT certificateid,
             controllerid,
             typeid,
            certificatetitle,       -- لازم يرجع هالحقل         
            issuingauthority,
             issuingcountry,
             issuedate,
             expirydate,
             status,
             statusreason,
             filepath,
             notes
        FROM certificates
       WHERE certificateid = :certId";
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(sql, conn);
            cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("certId", certificateId));
            using var adapter = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }



        //*********************Profile****
        public DataRow GetControllerByUsername(string username)
        {
            const string sql = @"
        SELECT *
        FROM controllers
        WHERE username = :username";
            var dt = ExecuteQuery(sql, new OracleParameter("username", username));
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            return null;
        }
        public List<License> GetLicensesByController(string username)
        {
            const string sql = @"
        SELECT l.*, l.licensetype
        FROM licenses l
        JOIN controllers c ON l.controllerid = c.controllerid
        --JOIN documenttypes t ON l.licenseid = l.licenseid
        WHERE c.username = :username
        ORDER BY l.expirydate ASC";
            var dt = ExecuteQuery(sql, new OracleParameter("username", username));
            var list = new List<License>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new License
                {
                    LicenseId = Convert.ToInt32(row["licenseid"]),
                    TypeName = row["licensetype"].ToString(),
                    //IssueDate = Convert.ToDateTime(row["issuedate"]),
                    ExpiryDate = Convert.ToDateTime(row["expirydate"]),
                    FilePath = row["pdfpath"]?.ToString(),
                    // أضف باقي الحقول حسب الحاجة
                });
            }
            return list;
        }

        public List<CertificateViewModel> GetCertificatesByController(string username)
        {
            const string sql = @"
        SELECT cer.*, t.typename
        FROM certificates cer
        JOIN controllers c ON cer.controllerid = c.controllerid
        JOIN documenttypes t ON cer.typeid = t.typeid
        WHERE c.username = :username
        ORDER BY cer.certificateid ASC";
            var dt = ExecuteQuery(sql, new OracleParameter("username", username));
            var list = new List<CertificateViewModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CertificateViewModel
                {
                    CertificateId = Convert.ToInt32(row["certificateid"]),
                    TypeName = row["typename"].ToString(),
                    Title = row["certificatetitle"].ToString(),
                    IssueDate = Convert.ToDateTime(row["issuedate"]),
                    Status = row["status"].ToString(),
                    FilePath = row["filepath"]?.ToString(),
                    // أضف باقي الحقول حسب الحاجة
                });
            }
            return list;
        }
        public List<Observation> GetObservationsByController(string username)
        {
            const string sql = @"
        SELECT o.*
        FROM observations o
        JOIN controllers c ON o.controllerid = c.controllerid
        WHERE c.username = :username
        ORDER BY o.departdate ASC";
            var dt = ExecuteQuery(sql, new OracleParameter("username", username));
            var list = new List<Observation>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Observation
                {
                    ObservationId = Convert.ToInt32(row["observationid"]),
                    Country = row["travelcountry"].ToString(),
                    LicenseNumber = row["licensenumber"].ToString(),
                    DurationDays = Convert.ToInt32(row["duration_days"]),
                    DepartDate = row["departdate"] != DBNull.Value ? Convert.ToDateTime(row["departdate"]) : (DateTime?)null,
                    ReturnDate = row["departdate"] != DBNull.Value ? Convert.ToDateTime(row["returndate"]) : (DateTime?)null,
                    EndDate = Convert.ToDateTime(row["returndate"]),
                    Notes = row["notes"]?.ToString(),
                    // أضف باقي الحقول حسب الحاجة
                });
            }
            return list;
        }
        /////////////////////////Dashboard
        public int GetControllersCount()
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM controllers");
            return Convert.ToInt32(result);
        }

        public int GetExpiredLicensesCount()
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM licenses WHERE expirydate < SYSDATE");
            return Convert.ToInt32(result);
        }

        public int GetSoonExpiringLicensesCount()
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM licenses WHERE expirydate BETWEEN SYSDATE AND SYSDATE + 30");
            return Convert.ToInt32(result);
        }
        public Dictionary<string, int> GetCertificatesStats()
        {
            var dt = ExecuteQuery(@"SELECT status, COUNT(*) cnt FROM certificates GROUP BY status");
            return dt.Rows.Cast<DataRow>().ToDictionary(r => r["status"].ToString(), r => Convert.ToInt32(r["cnt"]));
        }

        public int GetTotalLicensesCount()
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM licenses");
            return Convert.ToInt32(result);
        }

        public Dictionary<string, int> GetExpiredLicensesOverTime()
        {
            var dt = ExecuteQuery(@"
        SELECT TO_CHAR(expirydate, 'YYYY-MM') AS expiry_month, COUNT(*) AS count
        FROM licenses
        WHERE expirydate < SYSDATE
        GROUP BY TO_CHAR(expirydate, 'YYYY-MM')
        ORDER BY TO_CHAR(expirydate, 'YYYY-MM')
    ");
            return dt.Rows.Cast<DataRow>().ToDictionary(r => r["expiry_month"].ToString(), r => Convert.ToInt32(r["count"]));
        }

        public List<string> GetControllerDetails()
        {
            // استعلام لجلب أسماء المراقبين أو تفاصيل أخرى
            var dt = ExecuteQuery("SELECT fullname FROM controllers");
            return dt.Rows.Cast<DataRow>().Select(r => r["fullname"].ToString()).ToList();
        }

        // تأكد من وجود هذا السطر في الأعلى

        // ...
        public List<License> GetExpiredLicensesDetails()
        {
            var dt = ExecuteQuery(@"
        SELECT l.licenseid, l.expirydate, c.fullname AS controllername, l.controllerid
        FROM licenses l
        JOIN controllers c ON l.controllerid = c.controllerid
        WHERE l.expirydate < SYSDATE
    ");
            return dt.Rows.Cast<DataRow>().Select(r => new License
            {
                LicenseId = Convert.ToInt32(r["licenseid"]),
                ExpiryDate = Convert.ToDateTime(r["expirydate"]),
                ControllerName = r["controllername"].ToString(),
                ControllerId = Convert.ToInt32(r["controllerid"])
            }).ToList();
        }
        // ... (باقي دوال OracleDb)
        public List<License> GetAllLicensesDetails()
        {
            var dt = ExecuteQuery(@"
        SELECT l.licenseid, l.expirydate, c.fullname AS controllername, l.controllerid
        FROM licenses l
        JOIN controllers c ON l.controllerid = c.controllerid
    ");
            return dt.Rows.Cast<DataRow>().Select(r => new License
            {
                LicenseId = Convert.ToInt32(r["licenseid"]),
                ExpiryDate = Convert.ToDateTime(r["expirydate"]),
                ControllerName = r["controllername"].ToString(),
                ControllerId = Convert.ToInt32(r["controllerid"])
            }).ToList();
        }
        public List<License> GetSoonExpiringLicensesDetails()
        {
            var dt = ExecuteQuery(@"
        SELECT l.licenseid, l.expirydate, c.fullname AS controllername, l.controllerid
        FROM licenses l
        JOIN controllers c ON l.controllerid = c.controllerid
        WHERE l.expirydate BETWEEN SYSDATE AND SYSDATE + 30
    ");
            return dt.Rows.Cast<DataRow>().Select(r => new License
            {
                LicenseId = Convert.ToInt32(r["licenseid"]),
                ExpiryDate = Convert.ToDateTime(r["expirydate"]),
                ControllerName = r["controllername"].ToString(),
                ControllerId = Convert.ToInt32(r["controllerid"])
            }).ToList();
        }

        public string GetUserEmailById(int userId)
        {
            using (OracleConnection connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SELECT email FROM CONTROLLERS WHERE controllerid = :userid", connection))
                {
                    command.Parameters.Add(new OracleParameter("userid", userId));
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["email"].ToString();
                        }
                        return null; // أو قيمة افتراضية أخرى إذا لم يتم العثور على المستخدم
                    }
                }
            }
        }

        public List<ControllerUser> GetControllers(string filter = "")
        {
            var controllers = new List<ControllerUser>();
            Console.WriteLine("عدد المراقبين: " + controllers.Count); // أو استخدم Debug أو حتى TempData

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT controllerid, fullname, c.username, password, c.airportid, photopath, licensepath, c.userid, licensenumber, job_title, 
                                education_level, date_of_birth, marital_status, phone_number, email, address, hire_date, employment_status, 
                                current_department, transfer_date, emergency_contact ,u.rolename,a.icao_code
                         FROM controllers c, users u ,AIRPORTS a 
                         where c.username =u.username and c.airportid= a.airportid";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += " AND LOWER(fullname) LIKE :filter";
                }

                using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(filter))
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("filter", $"%{filter.ToLower()}%"));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            controllers.Add(new ControllerUser
                            {
                                ControllerId = Convert.ToInt32(reader["controllerid"]),
                                FullName = reader["fullname"]?.ToString(),
                                Username = reader["username"]?.ToString(),
                                Password = reader["password"]?.ToString(),
                                AirportId = Convert.ToInt32(reader["airportid"]),
                                PhotoPath = reader["photopath"]?.ToString(),
                                LicensePath = reader["licensepath"]?.ToString(),
                                UserId = Convert.ToInt32(reader["userid"]),
                                LicenseNumber = reader["licensenumber"]?.ToString(),
                                JobTitle = reader["job_title"]?.ToString(),
                                EducationLevel = reader["education_level"]?.ToString(),
                                DateOfBirth = reader["date_of_birth"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["date_of_birth"]),
                                MaritalStatus = reader["marital_status"]?.ToString(),
                                PhoneNumber = reader["phone_number"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Address = reader["address"]?.ToString(),
                                HireDate = reader["hire_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["hire_date"]),
                                EmploymentStatus = reader["employment_status"]?.ToString(),
                                CurrentDepartment = reader["current_department"]?.ToString(),
                                TransferDate = reader["transfer_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["transfer_date"]),
                                EmergencyContact = reader["emergency_contact"]?.ToString(),
                                Role = reader["rolename"]?.ToString(),
                                AirportName = reader["icao_code"]?.ToString()
                            });
                        }
                    }
                }
            }
            return controllers;
        }

        public List<LicenseModel> GetLicenses(string filter)
        {
            var licenses = new List<LicenseModel>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT l.licenseid, l.controllerid, l.licensetype, l.expirydate, l.pdfpath, l.photopath, l.range, l.note, l.issuedate,c.licensenumber, c.fullname, c.username
                         FROM licenses l
                         JOIN controllers c ON l.controllerid = c.controllerid
                         WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += " AND (LOWER(l.licensetype) LIKE :filter OR LOWER(c.fullname) LIKE :filter OR LOWER(l.licenseid) LIKE :filter)";
                }
                using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(filter))
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("filter", "%" + filter.ToLower() + "%"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            licenses.Add(new LicenseModel
                            {
                                LicenseId = reader["licenseid"].ToString(),
                                ControllerId = reader["controllerid"].ToString(),
                                LicenseType = reader["licensetype"].ToString(),
                                ExpiryDate = reader["expirydate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["expirydate"]),
                                PdfPath = reader["pdfpath"].ToString(),
                                PhotoPath = reader["photopath"].ToString(),
                                Range = reader["range"].ToString(),
                                Note = reader["note"].ToString(),
                                IssueDate = reader["issuedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["issuedate"]),
                                licensenumber = reader["licensenumber"].ToString(),
                                FullName = reader["fullname"].ToString(),
                                Username = reader["username"].ToString()
                            });
                        }
                    }
                }
            }
            return licenses;
        }

        public List<CertificateModel> GetCertificates(string filter)
        {
            var list = new List<CertificateModel>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT s.certificateid, s.controllerid, c.fullname, d.typename, d.typeid, s.certificatetitle, s.issuingauthority,
                   s.issuingcountry, s.issuedate, s.expirydate, s.status, s.statusreason, s.filepath, s.notes
            FROM certificates s
            JOIN controllers c ON s.controllerid = c.controllerid
            JOIN documenttypes d ON s.typeid = d.typeid
            WHERE 1=1
        ";

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += @"
                AND (
                    LOWER(c.fullname) LIKE :filter
                    OR LOWER(d.typename) LIKE :filter
                    OR LOWER(s.certificatetitle) LIKE :filter
                    OR LOWER(s.issuingauthority) LIKE :filter
                    OR LOWER(s.issuingcountry) LIKE :filter
                )";
                }

                using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("filter", $"%{filter.ToLower()}%"));
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CertificateModel
                            {
                                CertificateId = reader["certificateid"].ToString(),
                                ControllerId = reader["controllerid"].ToString(),
                                FullName = reader["fullname"].ToString(),
                                TypeName = reader["typename"].ToString(),
                                TypeId = reader["typeid"].ToString(),
                                CertificateTitle = reader["certificatetitle"].ToString(),
                                IssuingAuthority = reader["issuingauthority"].ToString(),
                                IssuingCountry = reader["issuingcountry"].ToString(),
                                IssueDate = reader["issuedate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["issuedate"]),
                                ExpiryDate = reader["expirydate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["expirydate"]),
                                Status = reader["status"].ToString(),
                                StatusReason = reader["statusreason"].ToString(),
                                FilePath = reader["filepath"].ToString(),
                                Notes = reader["notes"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<ObservationModel> GetObservations(string filter)
{
    var list = new List<ObservationModel>();
    using (var conn = GetConnection())
    {
        conn.Open();
        string query = @"
            SELECT o.observationid, o.controllerid, c.fullname, o.travelcount, o.duration_days, o.travelcountry,
                   o.departdate, o.returndate, o.licensenumber, o.filepath, o.observationno, o.notes
            FROM observations o
            JOIN controllers c ON o.controllerid = c.controllerid
            WHERE 1=1
        ";

        if (!string.IsNullOrWhiteSpace(filter))
        {
            query += @"
                AND (
                    LOWER(c.fullname) LIKE :filter
                    OR LOWER(o.travelcountry) LIKE :filter
                    OR LOWER(o.licensenumber) LIKE :filter
                    OR LOWER(o.notes) LIKE :filter
                )";
        }

        using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(query, conn))
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("filter", $"%{filter.ToLower()}%"));
            }

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new ObservationModel
                    {
                        ObservationId = reader["observationid"].ToString(),
                        ControllerId = reader["controllerid"].ToString(),
                        FullName = reader["fullname"].ToString(),
                        TravelCount = reader["travelcount"].ToString(),
                        DurationDays = reader["duration_days"].ToString(),
                        TravelCountry = reader["travelcountry"].ToString(),
                        DepartDate = reader["departdate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["departdate"]),
                        ReturnDate = reader["returndate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["returndate"]),
                        LicenseNumber = reader["licensenumber"].ToString(),
                        FilePath = reader["filepath"].ToString(),
                        ObservationNo = reader["observationno"].ToString(),
                        Notes = reader["notes"].ToString()
                    });
                }
            }
        }
    }
    return list;
}

        public List<NotificationModel> GetNotifications(string filter)
        {
            var list = new List<NotificationModel>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT n.notification_id, n.userid, n.controllerid, c.fullname, n.message, n.link, n.created_at, 
                                n.is_read, n.note, n.licensetype, n.licenseexpirydate,c.phone_number,c.email,c.current_department,a.airportname
                         FROM notifications n
                         JOIN controllers c ON n.controllerid = c.controllerid 
                         JOIN airports A ON a.airportid= c.airportid
                         WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    query += @" AND (LOWER(n.message) LIKE :filter 
                          OR LOWER(n.note) LIKE :filter 
                          OR LOWER(n.userid) LIKE :filter 
                          OR LOWER(c.fullname) LIKE :filter
                          OR LOWER(n.licensetype) LIKE :filter)";
                }

                using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(query, conn))
                {
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        cmd.Parameters.Add(new Oracle.ManagedDataAccess.Client.OracleParameter("filter", $"%{filter.ToLower()}%"));
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NotificationModel
                            {
                                NotificationId = reader["notification_id"].ToString(),
                                UserId = reader["userid"]?.ToString(),
                                ControllerId = reader["controllerid"]?.ToString(),
                                FullName = reader["fullname"]?.ToString(),
                                Message = reader["message"]?.ToString(),
                                Link = reader["link"]?.ToString(),
                                CreatedAt = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"]),
                                IsRead = reader["is_read"] != DBNull.Value && (reader["is_read"].ToString() == "1" || reader["is_read"].ToString().ToLower() == "true"),
                                Note = reader["note"]?.ToString(),
                                LicenseType = reader["licensetype"]?.ToString(),
                                LicenseExpiryDate = reader["licenseexpirydate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["licenseexpirydate"]),

                                phonenumber = reader["phone_number"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Currentdepartment = reader["current_department"]?.ToString(),
                                Location = reader["airportname"]?.ToString(),
                            });
                        }
                    }
                }
            }
            return list;
        }

        public DataTable GetSoonExpiringLicensesTable()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var cmd = new OracleCommand(@"
            SELECT c.fullname, l.licensetype, l.expirydate, c.phone_number, c.email
            FROM licenses l
            JOIN controllers c ON l.controllerid = c.controllerid
            WHERE l.expirydate BETWEEN SYSDATE AND SYSDATE + 30
        ", connection))
                using (var adapter = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }



    }
}