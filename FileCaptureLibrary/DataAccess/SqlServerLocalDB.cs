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

            EnsureDatabaseLayout();
        }

        private void EnsureDatabaseLayout()
        {
            // Checks to see if the tables exist and if not will run the create script
        }


        public void InsertVideo(FileInfo File)
        {
            
        }

        public List<object> GetVideos()
        {
            var vids = new List<object>();
            var command = new SqlCommand("select Name, Length, Date from Videos", Connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                vids.Add(new
                {
                    Name = reader.GetString(0),
                    Length = reader.GetFloat(1),
                    Date = reader.GetDateTime(2)
                });
            }

            return vids;
        }


        public static String GetOrCreateDatabaseFile()
        {
            var fileInfo = new DirectoryInfo(Environment.CurrentDirectory).GetFiles("*.*", SearchOption.AllDirectories).Where(x => x.Name == "FileDB.mdf").SingleOrDefault();

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
