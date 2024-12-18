using Documents.DocumentCommon;
using Documents.Models;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;

namespace Documents.Controllers
{
    public class FileUploadController : Controller
    {
        DBConnection dbConn = new DBConnection();
        DBHelper dbHelper = new DBHelper();
        TerminalLogger terminal = new TerminalLogger();
        public IActionResult Index()
        {
            return View();
        }

        public static void MakeFtpFileDirectory(string ftpServerIP, string ftpPort, string pathToCreate, string ftpUserId, string ftpPassword)
        {
            FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            string[] subDirs = pathToCreate.Split('/');
            string currentDir = string.Format("ftp://{0}:" + ftpPort, ftpServerIP);
            foreach (string subDir in subDirs)
            {
                try
                {
                    currentDir = currentDir + "/" + subDir;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
                    reqFTP.Proxy = new WebProxy();
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    ftpStream = response.GetResponseStream();
                    ftpStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    continue;
                    //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
                }
            }
        }
        public string DocUploadSingleFile(DocumentScanningInputBEO obj, string fileExt_fileId, IFormFile file, string sourceColumn, string sourceTable)
        {
            string returnString = "Fail";
            obj.FtpServerIP = "172.16.201.17"; obj.FtpPort = "21";  obj.FtpUserId = "sadequr"; obj.FtpPassword = "Ahyaan@u2";
      

            obj.FileDirectory = obj.FileDirectory.Replace(" ", "");
            MakeFtpFileDirectory(obj.FtpServerIP, obj.FtpPort, obj.FileDirectory, obj.FtpUserId, obj.FtpPassword);

            // Check if the directory exists, if not, create it
            string directoryUrl = String.Format("ftp://{0}/{1}", obj.FtpServerIP + ':' + obj.FtpPort, obj.FileDirectory);
            FtpWebRequest createDirectoryRequest = (FtpWebRequest)WebRequest.Create(directoryUrl);
            createDirectoryRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            createDirectoryRequest.Credentials = new NetworkCredential(obj.FtpUserId, obj.FtpPassword);

            try
            {
                using (FtpWebResponse createDirectoryResponse = (FtpWebResponse)createDirectoryRequest.GetResponse())
                {
                    Console.WriteLine($"Directory created. Status: {createDirectoryResponse.StatusDescription}");
                }
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    // Directory likely already exists, or there's another issue
                    Console.WriteLine($"Error creating directory: {response.StatusDescription}");
                }
            }

            try
            {

                string CntDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var filePath = obj.FileDirectory + "//" + fileExt_fileId;
                string[] tempDocumentId = fileExt_fileId.Split('.');
                string DocumentId = tempDocumentId[0];
                string FileExtension = tempDocumentId[1];

                terminal.FtpServerLocationLog(DocumentId, obj.FtpServerIP, obj.FtpPort, obj.FtpUserId, obj.FtpPassword, obj.SetEmployeeId, sourceColumn, sourceTable);

                String uploadUrl = String.Format("ftp://{0}/{1}", obj.FtpServerIP + ':' + obj.FtpPort, filePath);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                // This example assumes the FTP site uses anonymous logon.  
                request.Credentials = new NetworkCredential(obj.FtpUserId, obj.FtpPassword);
                request.Proxy = null;
                request.KeepAlive = true;
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;
                if (obj.CompressedFileFormat != "" && obj.CompressedFileFormat != null)
                {
                    //   SaveCompressedFile(file, ftpServerIP, ftpPort, ftpUserId, ftpPassword, filePath, compressedFileExtention);
                    SaveCompressedFileUsingThirdPartyDLL(file, obj.FtpServerIP, obj.FtpPort, obj.FtpUserId, obj.FtpPassword, filePath, obj.CompressedFileFormat);
                    returnString = "Success";
                }
                else
                {
                    using (Stream ftpStream = request.GetRequestStream())
                    {
                        using (Stream inputStream = file.OpenReadStream())
                        {
                            inputStream.CopyTo(ftpStream);
                        }
                        returnString = "Success";
                    }
                    // Get the FTP server response
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                    }
                }
                return returnString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void SaveCompressedFileUsingThirdPartyDLL(IFormFile file, string ftpServerIP, string ftpPort, string ftpUserId, string ftpPassword, string filePath, string compressedFileExtension)
        {
            string[] tempDocumentId = filePath.Split('.');
            string DocumentIdWithPath = tempDocumentId[0];
            string FileExtensionOnly = tempDocumentId[1];
            //   filePath = DocumentId + ".gz";
            string fileDestinationPath = DocumentIdWithPath + "." + compressedFileExtension;


            // Extract the original file name and extension
            string documentId = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath).TrimStart('.');

            // Define the path for the compressed file
            // string compressedFileName = documentId + "." + compressedFileExtension; // e.g., "1212.zip"

            // Compress the file into a .zip archive that includes the original file with its extension
            byte[] compressedData = CompressFileToZip(file, $"{documentId}.{fileExtension}"); // e.g., "1212.pdf" inside the zip



            String uploadUrl = String.Format("ftp://{0}/{1}", ftpServerIP + ':' + ftpPort, fileDestinationPath);

            UploadCompressedDataToFTP(compressedData, uploadUrl, ftpUserId, ftpPassword);
        }

        //private byte[] CompressFileToZip(IFormFile file, string internalFileName)
        //{
        //    using (var compressedStream = new MemoryStream())
        //    {
        //        using (var zipStream = new ZipOutputStream(compressedStream))
        //        {
        //            zipStream.SetLevel(9); // Compression level (0-9), 9 is the highest

        //            // Create a new zip entry for the original file name (e.g., "1212.pdf")
        //            var entry = new ZipEntry(internalFileName)
        //            {
        //                DateTime = DateTime.Now,
        //                Size = file.InputStream.Length
        //            };
        //            zipStream.PutNextEntry(entry);

        //            // Copy the file content into the zip entry
        //            file.InputStream.CopyTo(zipStream);
        //            zipStream.CloseEntry();
        //        }

        //        // Return the compressed data as a byte array
        //        return compressedStream.ToArray();
        //    }
        //}
        private byte[] CompressFileToZip(IFormFile file, string internalFileName)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (string.IsNullOrWhiteSpace(internalFileName))
                throw new ArgumentNullException(nameof(internalFileName));

            using (var compressedStream = new MemoryStream())
            {
                using (var zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(compressedStream))
                {
                    zipStream.SetLevel(9); // Compression level (0-9), 9 is the highest compression

                    // Create a new zip entry for the file
                    var entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(internalFileName)
                    {
                        DateTime = DateTime.Now,
                        Size = file.Length
                    };

                    zipStream.PutNextEntry(entry);

                    // Copy the file content into the zip entry
                    using (var fileStream = file.OpenReadStream())
                    {
                        fileStream.CopyTo(zipStream);
                    }

                    zipStream.CloseEntry();
                }

                // Return the compressed data as a byte array
                return compressedStream.ToArray();
            }
        }

        private void UploadCompressedDataToFTP(byte[] data, string uploadUrl, string ftpUserId, string ftpPassword)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        public void SaveCompressedFile(IFormFile file, string ftpServerIP, string ftpPort, string ftpUserId, string ftpPassword, string filePath, string compressedFileExtention)
        {
            string[] tempDocumentId = filePath.Split('.');
            string DocumentId = tempDocumentId[0];
            string FileExtension = tempDocumentId[1];
            //   filePath = DocumentId + ".gz";
            filePath = filePath + "." + compressedFileExtention;

            // Compress the file
            byte[] compressedData = CompressFile(file);

            String uploadUrl = String.Format("ftp://{0}/{1}", ftpServerIP + ':' + ftpPort, filePath);
            // Create an FTP WebRequest
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl); // Use .gz for GZip compressed files
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Set FTP credentials
            request.Credentials = new NetworkCredential(ftpUserId, ftpPassword);

            // Set the request's content length
            request.ContentLength = compressedData.Length;

            using (Stream ftpStream = request.GetRequestStream())
            {
                // Write the compressed data to the FTP stream
                ftpStream.Write(compressedData, 0, compressedData.Length);
            }

            // Optionally, get response to confirm upload success
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }

        }
        //public byte[] CompressFile(IFormFile file)
        //{
        //    using (var originalStream = file.InputStream)
        //    using (var compressedStream = new MemoryStream())
        //    {
        //        // Create a GZipStream to write compressed data to compressedStream
        //        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        //        {
        //            originalStream.CopyTo(gzipStream);
        //        }

        //        // Return the compressed data as a byte array
        //        return compressedStream.ToArray();
        //    }
        //}
        public byte[] CompressFile(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            using (var originalStream = file.OpenReadStream()) // Use OpenReadStream instead of InputStream
            using (var compressedStream = new MemoryStream())
            {
                // Create a GZipStream to write compressed data to compressedStream
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    originalStream.CopyTo(gzipStream);
                }

                // Return the compressed data as a byte array
                return compressedStream.ToArray();
            }
        }

        private DateTime? lastCheckTime = null;
        private bool? lastCheckResult = null;
        public bool IsFtpServerActive(string ftpServerIP, string ftpPort, string ftpUserId, string ftpPassword)
        {
            TimeSpan cacheTimeout = TimeSpan.FromMinutes(1);
            // Return cached result if within the timeout window
            if (lastCheckResult.HasValue && lastCheckTime.HasValue && DateTime.Now - lastCheckTime.Value < cacheTimeout)
            {
                return lastCheckResult.Value;
            }
            bool isTrue = false;
            string testUrl = $"ftp://{ftpServerIP}:{ftpPort}/"; // Root directory for testing
            try
            {
                // Create a request to check the directory listing
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(testUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(ftpUserId, ftpPassword);
                request.Timeout = 5000; // Optional: set a timeout (5 seconds)
                // Attempt to get a response
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    // Check if response status is positive
                    if (response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen)
                    {
                        isTrue = true;
                    }
                    lastCheckTime = DateTime.Now;  // Update the time of the last check
                    lastCheckResult = isTrue;      // Cache the result
                    return isTrue;

                }
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse response)
                {
                    if (response.StatusCode == FtpStatusCode.NotLoggedIn)
                    {
                        Console.WriteLine("Authentication failed: Incorrect username or password.");
                    }
                    else
                    {
                        Console.WriteLine($"FTP Server check failed with status code: {response.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine($"FTP Server check failed: {ex.Message}");
                }
                lastCheckTime = DateTime.Now;  // Update the time of the last failed check
                lastCheckResult = isTrue;      // Cache the result (still false)
                return false;
            }
        }



    }
}
