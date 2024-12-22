using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class ModuleDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public Boolean SaveUpdate(ModuleBEL moduleBEL)
        {
            try
            {
                string Qry = "Sa_ModuleSP @ID='" + moduleBEL.ModuleID + "',@Name='" + moduleBEL.ModuleName + "',@IsActive='" + (moduleBEL.IsActive == false ? '0' : '1') + "'";
                var tuple = dbHelper.CmdExecuteGetMultipleData(dbConn.SAConnStrReader(),Qry);
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

        

        public bool DeleteExecute(ModuleBEL moduleBEL)
        {
            string Qry = " Delete from Sa_Module Where ModuleID='" + moduleBEL.ModuleID + "'";
            if (dbHelper.CmdExecute(dbConn.SAConnStrReader(), Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ModuleBEL> GetModuleList()
        {
            string Qry = "Select ModuleID,ModuleName,IsActive From Sa_Module ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(), Qry);
            List<ModuleBEL> item;

            item = (from DataRow row in dt.Rows
                    select new ModuleBEL
                    {
                        ModuleID = row["ModuleID"].ToString(),
                        ModuleName = row["ModuleName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }
    }
}