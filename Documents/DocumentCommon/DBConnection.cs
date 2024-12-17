namespace Documents.DocumentCommon
{
    public class DBConnection
    {
        private string connectionString = "";
        string computerName = "";
        public DBConnection()
        {
            computerName = Environment.MachineName;
            DocConnStrReader();
            SAConnStrReader();

        }

  
        public string DocConnStrReader()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();
            connectionString = builder.GetSection("ConnectionStrings").GetSection("DocConn").Value;
            connectionString = connectionString.Replace("AHYAAN", computerName);
            return connectionString;
        }
        public string SAConnStrReader()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();
            connectionString = builder.GetSection("ConnectionStrings").GetSection("SAConn").Value;
            connectionString = connectionString.Replace("AHYAAN", computerName);
            return connectionString;
        }
    }
}
