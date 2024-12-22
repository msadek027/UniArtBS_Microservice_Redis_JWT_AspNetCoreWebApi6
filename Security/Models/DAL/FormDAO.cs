using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


namespace Security.Models.DAL
{
    public class FormDAO:ReturnData
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public Boolean SaveUpdate(FormBEL formBEL)
        {
            try
            {
                string Qry = "Sa_FormSP @ID='" + formBEL.FormID + "',@Name='" + formBEL.FormName + "',@FormURL='" + formBEL.FormURL + "',@IsActive='" + (formBEL.IsActive == false ? '0' : '1') + "'";
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



        public bool DeleteExecute(FormBEL formBEL)
        {
            string Qry = " Delete from Sa_Form Where FormID='" + formBEL.FormID + "'";
            if (dbHelper.CmdExecute(dbConn.SAConnStrReader(),Qry))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<FormBEL> GetFormList()
        {
            string Qry = "SELECT FormID,FormName,FormURL,IsActive FROM Sa_Form ";
            DataTable dt = dbHelper.GetDataTable(dbConn.SAConnStrReader(),Qry);
            List<FormBEL> item;

            item = (from DataRow row in dt.Rows
                    select new FormBEL
                    {
                        FormID = row["FormID"].ToString(),
                        FormName = row["FormName"].ToString(),                       
                        FormURL = row["FormURL"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"].ToString())

                    }).ToList();
            return item;
        }
    }
}