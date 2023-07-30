using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CK3MapCreator.Parsers
{
    internal class provincesParser : Parser
    {
        FileLoader fileLoader = FileLoader.getFileLoader();

        //Create a bunch of province objects based on the unq
        public List<ProvinceCK3> createProvincesFromColors()
        {
            HashSet<Color> usedColors = new HashSet<Color>(); //
            List<ProvinceCK3> provinces = new List<ProvinceCK3>();

            Bitmap provincesMap = new Bitmap(fileLoader.getBasePath() + "provinces.png");
            provincesMap.Save(fileLoader.getBasePath() + "provinces.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            for (int x = 0; x < provincesMap.Width; x = x + 3)
            {
                for (int y = 0; y < provincesMap.Height; y = y + 3)
                {
                    Color color = provincesMap.GetPixel(x, y);

                    if (usedColors.Add(color))//Return false if color already present, hashset considerably faster
                    {
                        provinces.Add(new ProvinceCK3(color, x, y));
                    }

                    if (y == 0)
                    {
                        Console.Write("\rx = " + x + "/" + provincesMap.Width);
                        Console.Write(" - unique colors = " + usedColors.Count);
                    }
                }
            }

            //if (usedColors.Count != TOTALPROVINCES)
            //{
            //    return false;
            //}
            return provinces;
            /*Bitmap dots = new Bitmap(path + "provinces.bmp");
            dots = DrawFilledRectangle(dots.Width, dots.Height);
            foreach (province entry in provinces)
            {
                dots.SetPixel(entry.provCoord.X, entry.provCoord.Y, entry.provColor);
            }
            dots.Save(path + "dots2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            */
        }

        public List<ProvinceCK3> createProvincesFromFile()
        {
            List<ProvinceCK3> provinces = new List<ProvinceCK3>();

            using (var reader = new StreamReader(fileLoader.getBasePath() + "provinces.csv"))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] values = line.Split(';');

                    try
                    {
                        ProvinceCK3 tempProv = new ProvinceCK3();
                        //index;color1;color2;color3;name;x;y;culture;religion;duchy;kingdom;empire;
                        //Color of the province
                        tempProv.color = Color.FromArgb(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                        tempProv.ID = int.Parse(values[0]);
                        tempProv.name = values[4];
                        tempProv.x = int.Parse(values[5]);
                        tempProv.y = int.Parse(values[6]);
                        tempProv.Culture = values[7];
                        tempProv.Religion = values[8];
                        tempProv.duchy = values[9];
                        tempProv.kingdom = values[10];
                        tempProv.empire = values[11];

                        provinces.Add(tempProv);
                        //provinces[tempIndex] = tempProv;
                    }
                    catch
                    {
                        Console.Write("Error 2 getting province number");
                        //return false; 
                    }
                }
            }
            return provinces;
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
        
        //
        public void writeProvincesToFile(List<ProvinceCK3> provincesOut)
        {
            using (FileStream fs = File.Create(fileLoader.getBasePath() + OUTPUT + "00_landed_titles.txt"))
            {
                byte[] jsonbytes = JsonSerializer.SerializeToUtf8Bytes(this);
                fs.Write(jsonbytes, 0, jsonbytes.Length);

                /*
                AddText(fs, "@correct_culture_primary_score = 100\n@better_than_the_alternatives_score = 50\n@always_primary_score = 1000\n\n");
                foreach (ProvinceCK3 prov in provincesOut)
                {
                    String s = "fsdf";
                    AddText(fs, "");
                }

                //This code lets you add mutliple lines dynamically and write them out all at once
                List<string> lines = new List<string>(); //Each line will be stored as a string in here
                foreach (string s in lines)
                {
                    AddText(fs, s);
                }
                */
            }
        }
    }
}
