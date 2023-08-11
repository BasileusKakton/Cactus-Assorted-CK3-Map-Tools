using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator.Logging
{
    internal class LogFile : LogStratergy
    {
        public void execute(String output)
        {
            using (FileStream fs = File.OpenWrite(FileLoader.getFileLoader().getFilePath("log.txt")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(output);
                fs.Write(info, 0, info.Length);
            }
        }

        public void execute(String group, String output)
        {
            String s = group + " | " + output;
            using (FileStream fs = File.OpenWrite(FileLoader.getFileLoader().getFilePath("log.txt")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
