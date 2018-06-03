using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Util
{
    /// <summary>
    /// Save each given content to file of same naming convention.
    /// </summary>
    public class XMLSaver
    {
        int fileNumber = 0;
        string directoryPath;
        string fileName;
        string newDirectoryPath;
        bool deleteDir = false;

        List<string> filePaths = new List<string>();

        public XMLSaver(string directoryPath, string fileName, bool deleteDir = false)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;
            this.deleteDir = deleteDir;

            this.newDirectoryPath = directoryPath + "\\" + fileName + "_" + DateTime.Now.ToString("yyMMdd_Hmmss_fff");
        }

        public List<string> FilePaths { get => filePaths; set => filePaths = value; }

        public string save(String content)
        {
           Directory.CreateDirectory(newDirectoryPath);

           string filePath = newDirectoryPath + "\\" + fileName + ".part" + fileNumber++;
           File.WriteAllText(filePath, content);

           FilePaths.Add(filePath);

           return filePath;
        }

       
    }
}
