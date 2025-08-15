using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.DataAccess
{
    public class PostgreSQLDb
    {
        private readonly string _connectionString;
        public string ConnectionString => _connectionString;
        private readonly ILogger<PostgreSQLDb> _logger;
        private readonly IPasswordHasher<ControllerUser> _passwordHasher;

        public PostgreSQLDb(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException("Connection string cannot be null.");
            Console.WriteLine($"PostgreSQLDb initialized with connection string: {connectionString}");
        }

        public PostgreSQLDb(IConfiguration configuration, IPasswordHasher<ControllerUser> passwordHasher, ILogger<PostgreSQLDb> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public NpgsqlConnection GetConnection()
            => new NpgsqlConnection(_connectionString);

        public DataTable ExecuteQuery(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            using var adapter = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            try
            {
                adapter.Fill(dt);
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"PostgreSQL Error: {ex.Message}");
                throw;
            }
            return dt;
        }

        public object ExecuteScalar(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteScalar();
        }

        public int ExecuteNonQuery(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd.ExecuteNonQuery();
        }

        // User Management Methods
        public ControllerUser GetUserByUsername(string username)
        {
            var sql = "SELECT * FROM controllerusers WHERE username = @username";
            var dt = ExecuteQuery(sql, new NpgsqlParameter("@username", username));
            
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new ControllerUser
                {
                    Id = Convert.ToInt32(row["id"]),
                    Username = row["username"].ToString(),
                    PasswordHash = row["passwordhash"].ToString(),
                    Role = row["role"].ToString()
                };
            }
            return null;
        }

        public void CreateUser(string username, string password, string role)
        {
            var passwordHasher = new PasswordHasher<ControllerUser>();
            var user = new ControllerUser { Username = username, Role = role };
            var hashedPassword = passwordHasher.HashPassword(user, password);

            var sql = "INSERT INTO controllerusers (username, passwordhash, role) VALUES (@username, @passwordhash, @role)";
            ExecuteNonQuery(sql, 
                new NpgsqlParameter("@username", username),
                new NpgsqlParameter("@passwordhash", hashedPassword),
                new NpgsqlParameter("@role", role));
        }

        public bool ValidateUser(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null) return false;

            var passwordHasher = new PasswordHasher<ControllerUser>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        // Employee Methods
        public List<Employee> GetAllEmployees()
        {
            var sql = "SELECT * FROM employees ORDER BY fullname";
            var dt = ExecuteQuery(sql);
            
            var employees = new List<Employee>();
            foreach (DataRow row in dt.Rows)
            {
                employees.Add(new Employee
                {
                    Id = Convert.ToInt32(row["employeeid"]),
                    FullName = row["fullname"].ToString(),
                    JobTitle = row["job_title"].ToString(),
                    Email = row["email"].ToString(),
                    PhoneNumber = row["phone_number"].ToString(),
                    HireDate = row["hire_date"] != DBNull.Value ? Convert.ToDateTime(row["hire_date"]) : null,
                    EmploymentStatus = row["employment_status"].ToString(),
                    CurrentDepartment = row["current_department"].ToString(),
                    Role = row["role"].ToString()
                });
            }
            return employees;
        }

        // Project Methods
        public List<Project> GetAllProjects()
        {
            var sql = "SELECT * FROM projects ORDER BY projectname";
            var dt = ExecuteQuery(sql);
            
            var projects = new List<Project>();
            foreach (DataRow row in dt.Rows)
            {
                projects.Add(new Project
                {
                    Id = Convert.ToInt32(row["projectid"]),
                    ProjectName = row["projectname"].ToString(),
                    Description = row["description"].ToString(),
                    StartDate = row["startdate"] != DBNull.Value ? Convert.ToDateTime(row["startdate"]) : null,
                    EndDate = row["enddate"] != DBNull.Value ? Convert.ToDateTime(row["enddate"]) : null,
                    Status = row["status"].ToString(),
                    CreatedDate = row["createddate"] != DBNull.Value ? Convert.ToDateTime(row["createddate"]) : null
                });
            }
            return projects;
        }

        // License Methods
        public List<License> GetAllLicenses()
        {
            var sql = "SELECT * FROM licenses ORDER BY licensename";
            var dt = ExecuteQuery(sql);
            
            var licenses = new List<License>();
            foreach (DataRow row in dt.Rows)
            {
                licenses.Add(new License
                {
                    Id = Convert.ToInt32(row["licenseid"]),
                    LicenseName = row["licensename"].ToString(),
                    LicenseNumber = row["licensenumber"].ToString(),
                    IssueDate = row["issuedate"] != DBNull.Value ? Convert.ToDateTime(row["issuedate"]) : null,
                    ExpiryDate = row["expirydate"] != DBNull.Value ? Convert.ToDateTime(row["expirydate"]) : null,
                    Status = row["status"].ToString(),
                    EmployeeId = row["employeeid"] != DBNull.Value ? Convert.ToInt32(row["employeeid"]) : null
                });
            }
            return licenses;
        }

        // Certificate Methods
        public List<Certificate> GetAllCertificates()
        {
            var sql = "SELECT * FROM certificates ORDER BY certificatename";
            var dt = ExecuteQuery(sql);
            
            var certificates = new List<Certificate>();
            foreach (DataRow row in dt.Rows)
            {
                certificates.Add(new Certificate
                {
                    Id = Convert.ToInt32(row["certificateid"]),
                    CertificateName = row["certificatename"].ToString(),
                    CertificateNumber = row["certificatenumber"].ToString(),
                    IssueDate = row["issuedate"] != DBNull.Value ? Convert.ToDateTime(row["issuedate"]) : null,
                    ExpiryDate = row["expirydate"] != DBNull.Value ? Convert.ToDateTime(row["expirydate"]) : null,
                    Status = row["status"].ToString(),
                    EmployeeId = row["employeeid"] != DBNull.Value ? Convert.ToInt32(row["employeeid"]) : null
                });
            }
            return certificates;
        }
    }
} 