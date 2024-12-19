namespace Workflow.WorkflowCommon
{
    public class Encryption
    {
        public string Encrypt(string szPlainText)
        {
            int iCount;
            string szReturn = "";
            foreach (char ch in szPlainText)
            {
                iCount = ((int)ch);
                iCount = iCount + 4;
                szReturn = szReturn + ((char)iCount);
            }
            return szReturn;

        }
        public string Decrypt(string szEncText)
        {
            int iCount;
            string szReturn = "";
            foreach (char ch in szEncText)
            {
                iCount = ((int)ch);
                iCount = iCount - 4;
                szReturn = szReturn + ((char)iCount);
            }
            return szReturn;

        }
    }
}
