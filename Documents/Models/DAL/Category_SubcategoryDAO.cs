using Documents.DocumentCommon;
using Documents.Models.BEL;
using System;
using System.Data;

namespace Documents.Models.DAL
{
    public class Category_SubcategoryDAO: ReturnData
    {
        TerminalLogger terminal = new TerminalLogger();
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        public bool SaveUpdateCategory(string categoryId, string categoryName)
        {
            bool isTrue = false;
            if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(categoryName))
            {             
                string Qry = @"Update Doc_Category Set CategoryName='" + categoryName + "' Where  CategoryId='" + categoryId + "'";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    MaxID = categoryId;
                    IUMode = "U";
                    isTrue = true;
                }
            }
            else
            {
                string QryMaxId = @"Select dbHR.dbo.GetID((select isnull(MAX(CategoryId),0)+1  from dbDOC.dbo.Doc_Category),2)";
                categoryId = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxId);             
                string Qry = @"Insert Into Doc_Category(CategoryId,CategoryName) Values ('" + categoryId + "','" + categoryName + "')";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    MaxID = categoryId;
                    IUMode = "I";
                    isTrue = true;
                }
            }
            return isTrue;
        }
        public bool SaveUpdateSubCategory(string CategoryId, string SubcategoryId, string SubcategoryName)
        {
            bool isTrue = false;
            if (!string.IsNullOrEmpty(SubcategoryId) && !string.IsNullOrEmpty(SubcategoryId))
            {
                string Qry = @"Update Doc_CategorySub Set SubcategoryName='" + SubcategoryName + "',CategoryId='" + CategoryId + "' Where  SubcategoryId='" + SubcategoryId + "'";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    MaxID = SubcategoryId;
                    IUMode = "U";
                    isTrue = true;
                }
            }
            else
            {
                string QryMaxId = @"Select dbHR.dbo.GetID((select isnull(MAX(SubcategoryId),0)+1  from dbDOC.dbo.Doc_CategorySub),3)";
                SubcategoryId = dbHelper.GetValue(dbConn.DocConnStrReader(), QryMaxId);
                string Qry = @"Insert Into Doc_CategorySub(CategoryId,SubcategoryId,SubcategoryName) Values ('" + CategoryId + "','" + SubcategoryId + "','" + SubcategoryName + "')";
                if (dbHelper.CmdExecute(dbConn.DocConnStrReader(), Qry))
                {
                    MaxID = SubcategoryId;
                    IUMode = "I";
                    isTrue = true;
                }
            }
            return isTrue;
        }

        public List<Category_SubcategoryBEO> GetSubcategoryList()
        {
            string Qry = "Select a.SubcategoryId,a.SubcategoryName,REPLACE(b.CategoryName+'/'+a.SubcategoryName, ' ', '') as FileDirectory,a.CategoryId,CategoryName From Doc_CategorySub a, Doc_Category b Where a.CategoryId=b.CategoryId --AND a.CategoryId!='02'";
            DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            List<Category_SubcategoryBEO> item;

            item = (from DataRow row in dt.Rows
                    select new Category_SubcategoryBEO
                    {
                        SubcategoryId = row["SubcategoryId"].ToString(),
                        SubcategoryName = row["SubcategoryName"].ToString(),
                        FileDirectory = row["FileDirectory"].ToString(),

                        CategoryId = row["CategoryId"].ToString(),
                        CategoryName = row["CategoryName"].ToString(),
                    }).ToList();
            return item;
        }

        public List<Category_SubcategoryBEO> GetCategoryList()
        {
            string Qry = "Select CategoryId,CategoryName, REPLACE(CategoryName ,' ', '') as FileDirectory From Doc_Category Where CategoryId!='02'";
            DataTable dt = dbHelper.GetDataTable(dbConn.DocConnStrReader(), Qry);
            List<Category_SubcategoryBEO> item;
            item = (from DataRow row in dt.Rows
                    select new Category_SubcategoryBEO
                    {
                        CategoryId = row["CategoryId"].ToString(),
                        CategoryName = row["CategoryName"].ToString(),
                        FileDirectory = row["FileDirectory"].ToString(),
                    }).ToList();
            return item;
        }
    }
}
