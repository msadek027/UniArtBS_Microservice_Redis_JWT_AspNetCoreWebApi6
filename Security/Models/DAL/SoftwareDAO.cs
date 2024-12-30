using Security.Models.BEL;
using Security.WorkflowCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class SoftwareDAO: ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public bool SaveUpdate(SoftwareBEL master)
        {
            try
            {
                string Qry = "Sa_SoftwareSP @ID='" + master.SoftwareID + "',@ShortName='" + master.SoftwareShortName + "',@FullName='" + master.SoftwareFullName + "',@IsActive='" + (master.IsActive == false ? '0' : '1') + "'";

                var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(), Qry);
                if (tuple.Item1)
                {
                    MaxID = tuple.Item2;
                    IUMode = tuple.Item3;
                    return true;

                }


                else
                {
                    return false;
                }
            }
            catch (Exception errorException)
            {
                throw errorException;
            }
        }


        public List<SoftwareBEL> GetSoftwareList()
        {
            string Qry = "SELECT SoftwareID,SoftwareShortName,SoftwareFullName,IsActive from Sa_Software";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<SoftwareBEL> item;

            item = (from DataRow row in dt.Rows
                    select new SoftwareBEL
                    {
                        SoftwareID = row["SoftwareID"].ToString(),
                        SoftwareShortName = row["SoftwareShortName"].ToString(),
                        SoftwareFullName = row["SoftwareFullName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }

        //public bool DeleteExecute(SoftwareBEL softwareBEL)
        //{
        //    string Qry = " Delete from Sa_Software Where SoftwareID='" + softwareBEL.ID + "'";
        //    if (dbHelper.CmdExecute(Qry))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}