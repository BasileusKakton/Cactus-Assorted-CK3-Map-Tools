using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CK3cultureAutomator
{
    public struct axis2d
    {
        public axis2d(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; }
        public int Y { get; }
    }

    class Program
    {

        static List<province> provinces = new List<province>();
        const string PATH = "C:\\Users\\Damian\\Desktop\\ck3autotestzone\\zone1\\";
        const int TOTALPROVINCES = 9459;
        static string[] provinceHistoryFiles;
        const int STARTDATE = 1066;

        static void Main(string[] args)
        {
            /*
            string[] tempStrList;
            string tempstr = "	culture = ethiopian";
            tempStrList = tempstr.Split(' ');
            foreach (string st in tempStrList) {
                Console.Write(st + "\n");
            }
            return;
            */

            if (!loadProvinces())
            {
                Console.Write("\rFailure loading provinces\n");
                return;
            }
            Console.Write("\rProvinces loaded                        \n");

            
            if (!loadProvinceNumbers())
            {
                Console.Write("\rFailure loading provinces IDs\n");
                return;
            }
            Console.Write("\rProvinces ID's loaded                        \n");

            //foreach (province entry in provinces){Console.WriteLine(entry.provName);}

            loadProvinceHistory();
            //foreach (province entry in provinces) { Console.WriteLine(entry.culture); }

                        

            //Assign culture and relgion based on provence ID. 
            //Read culture or religion edited PNG, assign values
            //Write religions and culture based on values below
        }

        static public bool loadProvinceHistory()
        {
            provinceHistoryFiles = Directory.GetFiles(PATH + "provinces", "*.txt");
            //List<provinceHistory> provincehist = new List<provinceHistory>();

            foreach (string p in provinceHistoryFiles) 
            {
                bool readingProvince = false;
                int lastID = -1;
                //bool dateDetected = false;
                //bool buildingsDetected = false;
                int bracketTracker = 0;

                province tempProv = provinces[0];//Default to prevent compiler crying

                using (var reader = new StreamReader(p))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        line.Replace('\t', ' ');
                        string[] values = line.Split(' ');

                        if (line.Trim().Length == 0)//Prevent crash for next if statement, skip empty line
                        {
                            continue;
                        }
                        if (line.Trim().ElementAt(0) == '#')//Ignore commented out lines, skip
                        {
                            continue;
                        }

                        for (int x = 0; x < values.Length; x++) //Tab fix
                        {
                            if (values[x] == "") {
                                
                            }
                            else if (values[x].ElementAt(0) == '	')
                            {
                                values[x] = values[x].Remove(0, 1);
                            }
                            //Console.WriteLine("testpoint1: " + v);
                        }

                        if (line.Contains("{")) //Start of an object
                        {
                            if (readingProvince)//Already in an object
                            {
                                bracketTracker++;
                            }
                            else //Not already inside object, procede normally
                            {
                                readingProvince = true; //Watch for bad brackets
                                //Console.WriteLine("crashpoint = " + values[0]);
                                int tempID = int.Parse(values[0]); //Get ID
                                tempProv = provinces.Find(x => x.provID == tempID); //Get the province from the ID
                                //Console.Write("name2 = " + provinces.Find(x => x.provID == lastID).provName + "\n");
                                tempProv.provHistoryPath = p; //Get file for future reference
                                lastID = tempID;
                            }
                        }

                        if (readingProvince) //Continue processing object
                        {
                            if (lastID == -1) //Shouldn't be here
                            {
                                Console.Write("Error reading province - province not defined");
                                return false;
                            }
                            if (values.Contains("culture"))
                            {
                                int tempInd = Array.IndexOf(values, "culture");
                                tempProv.culture = values[tempInd + 2];
                                //Console.Write("Culture set: " + values[tempInd + 2]);
                            }
                            if (values.Contains("religion")) {
                                int tempInd = Array.IndexOf(values, "religion");
                                tempProv.religion = values[tempInd + 2];
                            }
                            if (values.Contains("holding"))
                            {
                                int tempInd = Array.IndexOf(values, "holding");
                                tempProv.government = values[tempInd + 2];
                            }
                            if (values.Contains("}")) { //End bracket
                                if (bracketTracker != 0) {
                                    bracketTracker--;
                                }
                                else //Finish building object and export
                                {
                                    int tempIndex = -1; //Reset to prevent overwriting
                                    try
                                    {
                                        tempIndex = provinces.FindIndex(x => x.provID == lastID); //Find where original copy is in list
                                        //Console.Write("tempIndex = " + tempIndex + "\n");
                                        //Console.Write("provID = " + lastID + "\n");
                                        //Console.Write("culture = " + tempProv.culture + "\n");
                                        //Console.Write("religion = " + tempProv.religion + "\n");
                                        //Console.Write("government = " + tempProv.government + "\n\n");
                                        provinces[tempIndex] = tempProv; //Replace old version with new updated version
                                    }
                                    catch
                                    {
                                        Console.Write("Error appending back into list\n");
                                        //Console.Write("\nProvince ID = " + lastID);
                                    }
                                    readingProvince = false;
                                    lastID = -1;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        static public bool loadProvinceNumbers()
        {
            using (var reader = new StreamReader(PATH + "definition.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    try
                    {
                        Color tempColor = Color.FromArgb(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                        province tempProv = provinces.Find(x => x.provColor == tempColor);
                        int tempIndex = provinces.FindIndex(x => x.provColor == tempColor);
                        if (tempProv.provColor == null) 
                        {
                            Console.Write("Error 1 getting province number");
                            return false;
                        }
                        tempProv.provID = int.Parse(values[0]);
                        tempProv.provName = values[4];
                        provinces[tempIndex] = tempProv;
                    }
                    catch
                    {
                        //Console.Write("Error 2 getting province number");
                        //return false; 
                    }
                }
            }
            return true;
        }

        static public bool loadProvinces()
        {
            HashSet<Color> usedColors = new HashSet<Color>();

            Bitmap provincesMap = new Bitmap(PATH + "provinces.png");
            provincesMap.Save(PATH + "provinces.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            for (int x = 0; x < provincesMap.Width; x = x + 3)
            {
                for (int y = 0; y < provincesMap.Height; y = y + 3)
                {
                    Color color = provincesMap.GetPixel(x, y);

                    if (usedColors.Add(color))//Return false if color already present, hashset considerably faster
                    {
                        provinces.Add(new province() { provColor = color, provCoord = new axis2d(x, y) });
                    }

                    if (y == 0)
                    {
                        Console.Write("\rx = " + x + "/" + provincesMap.Width);
                        Console.Write(" - unique colors = " + usedColors.Count);
                    }
                }
            }

            if (usedColors.Count != TOTALPROVINCES) {
                return false;
            }
            return true;
            /*Bitmap dots = new Bitmap(path + "provinces.bmp");
            dots = DrawFilledRectangle(dots.Width, dots.Height);
            foreach (province entry in provinces)
            {
                dots.SetPixel(entry.provCoord.X, entry.provCoord.Y, entry.provColor);
            }
            dots.Save(path + "dots2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            */
        }

        //Iterate though whole image like before
        //If new color is not in hashset, add it
            //Run algorithm painting pixels of the old color

        public static Bitmap DrawFilledRectangle(int x, int y)//https://stackoverflow.com/questions/12502365/how-to-create-1024x1024-rgb-bitmap-image-of-white
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
            return bmp;
        }
    }

    public struct province
    {
        public Color provColor;
        public axis2d provCoord;
        public int provID;
        public string provName;
        public string culture;
        public string religion;
        public string government;

        public string provHistoryPath;
    }

    public struct provinceHistory
    {
        public string phPath;
        public List<int> provincesInFile;
    }

}

   //ColorPalette pal = loadedImage.Palette;
    //Color.FromArgb(transparencyData[i], col.R, col.G, col.B);