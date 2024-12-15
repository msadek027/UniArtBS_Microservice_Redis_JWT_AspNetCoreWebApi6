namespace Documents.DocumentCommon
{
    public class DBConnection
    {
        private string connectionString = "";

        public DBConnection()
        {
            DocConnStrReader();
            SAConnStrReader();

        }

  
        public string DocConnStrReader()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();
            connectionString = builder.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            return connectionString;
        }
        public string SAConnStrReader()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();
            connectionString = builder.GetSection("ConnectionStrings").GetSection("SAConn").Value;
            return connectionString;
        }
    }
}
