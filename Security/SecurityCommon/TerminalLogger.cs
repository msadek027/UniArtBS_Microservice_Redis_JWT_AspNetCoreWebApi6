namespace Security.WorkflowCommon
{
    public class TerminalLogger
    {
        DBHelper dbHelper = new DBHelper();
        DBConnection dbConn = new DBConnection();
        public String GetLanIPAddress()
        {
            //String ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //if (ip == "::1")
            //{
            //    ip = "127.0.0.1";
            //}
            //return ip;
            return "";        
        }

        public void OperationalLogTaleCallFromCtrl(string OperationalCode, string setEmployeeId, string currentUrl, string setTerminal, string SourceColumn, string SourceTable, string Mode)
        {
            try
            {
                bool isTrue = false;
                string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Uri uri = new Uri(currentUrl);
                // Get the root part of the URL
                string rootUrl = uri.GetLeftPart(UriPartial.Authority);
                // Get the rest of the URL (excluding the root part)
                string restOfUrl = currentUrl.Substring(rootUrl.Length);
                string ViaChannel = restOfUrl;
                string Qry = @"INSERT INTO dbSA.dbo.Sa_OperationalLogTale(OperationalCode,SourceColumn,SourceTable,Mode,ViaChannel,SetEmployeeId,SetDateTime,SetTerminal)
                         Values('" + OperationalCode + "','" + SourceColumn + "','" + SourceTable + "','" + Mode + "','" + ViaChannel + "','" + setEmployeeId + "','" + CntDate + "','" + setTerminal + "')";
                if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry))
                {
                    isTrue = true;
                }
            }
            catch (Exception ex)
            {
                string exp = ex.ToString();
            }
        }
        public void OperationalLogTaleCallFromDAO(string OperationalCode, string setEmployeeId, string currentUrl, string setTerminal, string SourceColumn, string SourceTable, string Mode)
        {
            try
            {
                bool isTrue = false;
                string CntDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                Uri uri = new Uri(currentUrl);
                // Get the root part of the URL
                string rootUrl = uri.GetLeftPart(UriPartial.Authority);
                // Get the rest of the URL (excluding the root part)
                string restOfUrl = currentUrl.Substring(rootUrl.Length);
                string ViaChannel = restOfUrl;
                string Qry = @"INSERT INTO dbSA.dbo.Sa_OperationalLogTale(OperationalCode,SourceColumn,SourceTable,Mode,ViaChannel,SetEmployeeId,SetDateTime,SetTerminal)
                               Values('" + OperationalCode + "','" + SourceColumn + "','" + SourceTable + "','" + Mode + "','" + ViaChannel + "','" + setEmployeeId + "','" + CntDate + "','" + setTerminal + "')";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    isTrue = true;
                }
            }
            catch (Exception ex)
            {
                string exp = ex.ToString();
            }
        }
        public void FtpServerLocationLog(string DocumentId, string ftpServerIP, string ftpPort, string ftpUserId, string ftpPassword, string setEmployeeId, string sourceColumn, string sourceTable)
        {
            try
            {
                bool isTrue = false;
                string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string QrySelect = "Select * from Doc_Document_FtpServer Where DocumentId='" + DocumentId + "'";
                var tuple = dbHelper.CmdExecuteGetDataTable(dbConn.DocConnStrReader(), QrySelect);
                if (tuple.Item1)
                {
                    isTrue = true;
                }
                else
                {
                    string Qry = "Insert Into Doc_Document_FtpServer(DocumentId,FtpServerIP,FtpPort,FtpUserId,FtpPassword,SourceColumn,SourceTable) Values('" + DocumentId + "','" + ftpServerIP + "','" + ftpPort + "','" + ftpUserId + "','" + ftpPassword + "','" + sourceColumn + "','" + sourceTable + "')";
                    if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                    {
                        isTrue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string exp = ex.ToString();
            }
        }


    }
}
