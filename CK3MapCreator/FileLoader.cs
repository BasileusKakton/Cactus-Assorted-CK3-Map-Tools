using CK3MapCreator.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

            if (!fileLoader.files.ContainsKey(key))
            {
                Console.WriteLine(key + " doesn't have a path set - writing to root");
                fileLoader.addFilePath(key, key);
            }

            String outPath;
            if (!fileLoader.files.TryGetValue(key, out outPath))
            {
                Logger logger = Logger.getLogger();
                logger.Log("Cannot open file for { " + key + " } ");
                return "";
            }
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
            //initial map file, definition.csv, terrain.csv, 00_province_terrain, black heightmap
            if (fileLoader.basePath == null)
            {
                Console.WriteLine("Warning: Base Path not set!");
                return;
            }
            fileLoader.addFilePath("input definition.csv", "input\\definition.csv");
            fileLoader.addFilePath("input terrain.csv", "input\\terrain.csv");
            fileLoader.addFilePath("input 00_province_terrain.txt", "working\\00_province_terrain.txt");
            
            fileLoader.addFilePath("input initial map", "input\\initmap.png");
            fileLoader.addFilePath("input post province map", "working\\provmapv1.bmp");
            fileLoader.addFilePath("input pre heightmap", "input\\initheightmap.png");
            fileLoader.addFilePath("input post heightmap", "working\\heightmapv1.png");

            //fileLoader.addFilePath("", "\\input\\");
        }
    }
}
