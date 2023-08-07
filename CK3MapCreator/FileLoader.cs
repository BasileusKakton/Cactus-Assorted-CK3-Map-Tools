using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    //This is going to be a singleton
    internal class FileLoader
    {
        String basePath;
        static FileLoader fileLoader; 
        Dictionary<String, String> files = new Dictionary<String, String>();
        private FileLoader() { }

        public static FileLoader getFileLoader()
        {
            if (fileLoader == null)
            {
                fileLoader = new FileLoader();
            }
            return fileLoader;
        }

        public void addFilePath(String key, String path)
        {
            if (fileLoader == null)
            {
                Console.WriteLine("fileLoader is not initialized");
                return;
            }
            fileLoader.files.Add(key, path);
        }

        public string getFilePath(String key)
        {
            if (fileLoader == null)
            {
                Console.WriteLine("fileLoader is not initialized");
                return "";
            }
            String outPath;
            fileLoader.files.TryGetValue(key, out outPath);
            outPath = basePath + outPath;
            return outPath;
        }

        //Writes out all paths in the dictionary to a file
        public string exportPaths()
        {
			return "Not Done";
        }

        //Reads all paths in a file and loads them in a dictionary
        public string importPaths()
        {
            return "Not Done";
        }
        public void setBasePath(String path)
        {
            fileLoader.basePath = path;
        }
        public String getBasePath()
        {
            if (fileLoader.basePath == null)
            {
                Console.WriteLine("Warning: Base Path not set!");
                return "";
            }
            return fileLoader.basePath;
        }

        public void useDefaultPath()
        {
            //Initialize all the default file paths

        }
    }
}
