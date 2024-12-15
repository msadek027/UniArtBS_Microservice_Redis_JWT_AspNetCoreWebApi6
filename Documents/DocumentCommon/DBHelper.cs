using Microsoft.Data.SqlClient;
using System.Data;

namespace Documents.DocumentCommon
{
    public class DBHelper
    {
        public Boolean CmdExecute(string ConnString, string Qry)
        {
            bool isTrue = false;
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                int noOfRows = cmd.ExecuteNonQuery();

                if (noOfRows > 0)
                {
                    isTrue = true;

                }
            }
            return isTrue;
        }

        public Tuple<Boolean, string, string> CmdExecuteGetMultipleData(string ConnString, string Qry)
        {
            bool isTrue = false;
            string MaxID = "";
            string MaxCode = "";
            string IUMode = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        isTrue = true;
                        MaxID = rdr[0].ToString();
                        //MaxCode = rdr[1].ToString();
                        IUMode = rdr[rdr.FieldCount - 1].ToString();
                    }
                }
            }

            return Tuple.Create(isTrue, MaxID, IUMode);
        }
        public Tuple<Boolean, string, string, string> CmdExecuteGetMultipleData2(string ConnString, string Qry)
        {
            bool isTrue = false;
            string MaxID = "";
            string MaxCode = "";
            string IUMode = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        isTrue = true;
                        MaxID = rdr[0].ToString();
                        MaxCode = rdr[1].ToString();
                        IUMode = rdr[rdr.FieldCount - 1].ToString();
                    }
                }
            }

            return Tuple.Create(isTrue, MaxID, MaxCode, IUMode);
        }
        public Tuple<Boolean, string, string, string> CmdExecuteGetMultiple4Data(string ConnString, string Qry)
        {
            bool isTrue = false;
            string MaxID = "";
            string IUMode = "";
            string MaxCode = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        isTrue = true;
                        MaxID = rdr[0].ToString();
                        MaxCode = rdr[1].ToString();
                        IUMode = rdr[rdr.FieldCount - 1].ToString();
                    }
                }
            }

            return Tuple.Create(isTrue, MaxID, MaxCode, IUMode);
        }
        public Boolean TransactionLog(string ConnString, string TransactionID, string TransactionTable,string UserID)
        {
            bool isTrue = false;
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                string Qry = "Sa_UserTransactionLogSP @TransactionID='" + TransactionID + "',@TransactionTable='" + TransactionTable + "', " +
                    " @UserID='" + UserID + "',@SetTerminal ='121212'";
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        isTrue = true;
                    }
                }
            }

            return isTrue;
        }
        public Tuple<Boolean, DataTable> CmdExecuteGetDataTable(string ConnString, string Qry)
        {
            bool isTrue = false;
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand(Qry, con);
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        isTrue = true;
                        dt.Load(rdr);

                    }
                }
            }

            return Tuple.Create(isTrue, dt);
        }
        public DataTable MergeDataTable(DataTable dt1, DataTable dt2)
        {
            DataTable newDt = new DataTable();

            List<DataRow> rows = dt1.Rows.OfType<DataRow>().ToList();
            rows.AddRange(dt2.Rows.OfType<DataRow>().ToList());

            foreach (DataRow dataRow in rows)
            {
                newDt.Rows.Add(dataRow);
            }

            return newDt;
        }
        public DataSet GetDataSet(string ConnString, string Qry)
        {
            SqlDataAdapter odbcDataAdapter = new SqlDataAdapter(Qry, ConnString);
            DataSet ds = new DataSet();
            odbcDataAdapter.Fill(ds, "Results");
            return ds;
        }
        public DataTable GetDataTable(string ConnString, string Qry)
        {
            DataTable dt = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConnString))
            {
                SqlCommand objCmd = new SqlCommand();
                objCmd.CommandText = Qry;
                objCmd.Connection = objConn;
                objConn.Open();
                objCmd.ExecuteNonQuery();

                using (SqlDataReader rdr = objCmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        dt.Load(rdr);
                    }
                }
            }

            return dt;
        }
        public DataTable dtIncremented(DataTable dt)
        {
            DataTable dtIncremented = new DataTable(dt.TableName);
            DataColumn dc = new DataColumn("Col1");
            dc.AutoIncrement = true;
            dc.AutoIncrementSeed = 1;
            dc.AutoIncrementStep = 1;
            dc.DataType = typeof(Int32);
            dtIncremented.Columns.Add(dc);

            dtIncremented.BeginLoadData();

            DataTableReader dtReader = new DataTableReader(dt);
            dtIncremented.Load(dtReader);

            dtIncremented.EndLoadData();

            return dtIncremented;
        }
        public DataTable dtDecremented(DataTable dt)
        {
            DataTable dtDecremented = new DataTable(dt.TableName);

            // Create a new column with AutoIncrement settings for decrementing
            DataColumn dc = new DataColumn("S/L");
            dc.AutoIncrement = true;
            dc.AutoIncrementSeed = dt.Rows.Count; // Start with the row count
            dc.AutoIncrementStep = -1;           // Decrement by 1
            dc.DataType = typeof(Int32);
            dtDecremented.Columns.Add(dc);

            // Load data from the input DataTable
            dtDecremented.BeginLoadData();
            DataTableReader dtReader = new DataTableReader(dt);
            dtDecremented.Load(dtReader);
            dtDecremented.EndLoadData();

            return dtDecremented;
        }

        public DataTable GetDataTableWithSL(string ConnString, string Qry)
        {
            DataTable dt = new DataTable();
            using (SqlConnection objConn = new SqlConnection(ConnString))
            {
                SqlCommand objCmd = new SqlCommand();
                objCmd.CommandText = Qry;
                objCmd.Connection = objConn;
                objConn.Open();
                objCmd.ExecuteNonQuery();

                using (SqlDataReader rdr = objCmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        dt.Load(rdr);
                    }
                }
                DataColumn newCol = new DataColumn("SL", typeof(Int32));
                dt.Columns.Add(newCol);
                int counter = 1;
                foreach (DataRow row in dt.Rows)
                {
                    row["SL"] = counter;
                    counter++;
                }
            }

            return dt;
        }
        public bool HasRow(string ConnString, string Qry)
        {
            bool isTrue = false;
            DataTable dt = new DataTable();
            SqlDataAdapter odbcDataAdapter = new SqlDataAdapter(Qry, ConnString);
            odbcDataAdapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                isTrue = true;
            }
            else
            {
                isTrue = false;
            }
            return isTrue;
        }


        public string GetValue(string ConnString, string Qry)
        {
            string Value = "";
            SqlConnection odbcConnection = new SqlConnection(ConnString);
            odbcConnection.Open();
            SqlCommand odbcCommand = new SqlCommand(Qry, odbcConnection);
            SqlDataReader rdr = odbcCommand.ExecuteReader();
            if (rdr.Read())
            {
                Value = rdr[0].ToString();
            }
            rdr.Close();
            odbcConnection.Close();
            return Value;
        }
        //public string GetMAXID(string ConnString,string SqlStatement, string length)
        //{
        //    string MAXID = "";
        //    string QueryString = "select dbo.f_NumberFormate((" + SqlStatement + ")," + length + ")";
        //    SqlConnection odbcConnection = new SqlConnection(ConnString);
        //    odbcConnection.Open();
        //    SqlCommand odbcCommand = new SqlCommand(QueryString, odbcConnection);
        //    SqlDataReader rdr = odbcCommand.ExecuteReader();
        //    if (rdr.Read())
        //    {
        //        MAXID = rdr[0].ToString();
        //    }
        //    rdr.Close();
        //    odbcConnection.Close();
        //    return MAXID;
        //}



        public List<MenuItemView> GetMenuItem(DataTable dt)
        {
            //Select SLID,ID,NodeName,NodeLevel,RefID  FROM Sa_Menu Order By SLID";
            List<MenuItemView> item;
            //using lamdaexpression
            item = (from DataRow row in dt.Rows
                    select new MenuItemView
                    {
                        MenuID = Convert.ToInt16(row["SLID"].ToString()),
                        MenuName = row["NodeName"].ToString(),
                        ParentID = Convert.ToInt16(row["NodeLevel"].ToString()),
                        FormURL = row["FormURL"].ToString(),

                    }).ToList();

            return item;
        }


        //public void AttachParameter(SqlCommand command, string parameterName, Control control)
        //{
        //    if (control is TextBox && ((TextBox)control).Text != string.Empty)
        //    {
        //        SqlParameter parameter = new SqlParameter(parameterName, ((TextBox)control).Text);
        //        command.Parameters.Add(parameter);
        //    }
        //    else if (control is DropDownList && ((DropDownList)control).SelectedValue != "-1")
        //    {
        //        SqlParameter parameter = new SqlParameter(parameterName, ((DropDownList)control).SelectedValue);
        //        command.Parameters.Add(parameter);
        //    }
        //}



        public DataTable ConvertArrayListToDataTable(List<string> arrayList)
        {
            DataTable dataTable = new DataTable();

            // Check if ArrayList is not empty
            if (arrayList.Count > 0)
            {
                // Assuming all rows have the same structure, use the first row to create columns
                var firstRow = arrayList[0];
                if (firstRow != null)
                {
                    string[] headerSplit = firstRow.Trim().Split('/');
                    for (int i = 0; i <= headerSplit.Length; i++)
                    {
                        dataTable.Columns.Add("Column" + (i + 1));
                    }
                }

                // Add rows to the DataTable
                foreach (var item in arrayList)
                {
                    string[] valueSplit = item.Trim().Split('/');
                    var rowArray = item;
                    if (rowArray != null)
                    {
                        for (int i = 0; i <= valueSplit.Length; i++)
                        {
                            string[] fileWithExtension = valueSplit[5].Split('.');
                            dataTable.Rows.Add(rowArray, valueSplit[0].Replace(":", ""), valueSplit[1], valueSplit[2], valueSplit[3], valueSplit[4], fileWithExtension[0]);
                        }

                    }
                }
            }

            return dataTable;
        }
        public void RemoveColumn(DataTable dt, string columnName)
        {
            if (dt.Columns.Contains(columnName))
            {
                dt.Columns.Remove(columnName);
            }
            else
            {
                Console.WriteLine($"Column '{columnName}' does not exist.");
            }
        }
    }
}
