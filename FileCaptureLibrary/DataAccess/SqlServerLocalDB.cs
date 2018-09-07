using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace DataAccess
{
    public class SqlServerLocalDB
    {
        public SqlConnection Connection = new SqlConnection();

        public SqlServerLocalDB(String DbPath)
        {
            var connBuilder = new SqlConnectionStringBuilder
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = DbPath,
                IntegratedSecurity = true
            };
            Connection.ConnectionString = connBuilder.ConnectionString;
            Connection.Open();
        }

        private void EnsureDatabaseLayout()
        {
            // Checks to see if the tables exist and if not will run the create script
        }


        public void InsertFile(FileInfo File)
        {

        }


        public static String GetOrCreateDatabaseFile()
        {

            var fileInfo = new DirectoryInfo(Environment.CurrentDirectory).GetFiles().Where(x => x.Name == "FileDB").SingleOrDefault();

            if (fileInfo == null)
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, "Data", "BlankDB.mdf"), Path.Combine(Environment.CurrentDirectory, "Data", "FileDB.mdf"));
                fileInfo = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Data", "FileDB.mdf"));
            }

            // Look in the default location if the file isn't there create and return that!
            return fileInfo.FullName;
        }
    }
}
