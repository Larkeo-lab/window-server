using System;
using MySql.Data.MySqlClient;

namespace My_program.Views.helper
{
    public static class DatabaseConfig
    {
        public static string GetConnectionString()
        {
            string host = "localhost";
            string database = "pos_workshop";
            string username = "root";
            string password = "";
            string port = "3306";
            
            string connection_string = $"Server={host};Port={port};Database={database};Uid={username};Pwd={password};CharSet=utf8;Allow User Variables=True;";
            
            return connection_string;
        }
    }
}

