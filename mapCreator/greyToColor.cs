using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace mapCreator
{
	static public class greyToColor
	{

        static Dictionary<int, Color> IDdictionary; //Used for getting colors from ID
        static HashSet<Color> colorHashSet;// Used to efficiently iterate through used colors
        static int provinceIDCounter;
        static Bitmap map;
        static string definitionPath;

        static public Bitmap process(string definitionsPath, string mapPath)
        {
            map = new Bitmap(mapPath);
            definitionPath = definitionsPath;
            IDdictionary = new Dictionary<int, Color>();
            colorHashSet = new HashSet<Color>();
            provinceIDCounter = 1;

            loadColors();
            Console.WriteLine("Colors loaded");
            loadProvincesGray();

            return map;
        }

        static private bool loadProvincesGray()
        {
            //provincesMap = new Bitmap(PATH + originalFile + ".png");
            //provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            int width = map.Width;
            int height = map.Height;

            //while (usedIDs.Contains(provinceIDCounter)) //If ID was already used, go to next ID
            //{
            //    provinceIDCounter++;
            //}
            Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < height; y = y + 1)
                {
                    Color tempColor = map.GetPixel(x, y);
                    if (tempColor == Color.FromArgb(0,0,0,0) || tempColor == Color.Black) 
                    {
                        //Console.WriteLine("Empty");
                        continue;
                    }

                    //provincesMap.Save(PATH + "provincesgrey.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                    //Console.WriteLine("Working x = " + x + " / y = " + y);

                    if (!colorHashSet.Contains(tempColor))//Return false if color already present
                    {
                        map = fillShape(tempColor, newColor, x, y, map);

                        provinceIDCounter++;
                        //while (usedIDs.Contains(provinceIDCounter)) //If ID was already used, go to next ID
                        //{
                        //    provinceIDCounter++;
                        //}
                        newColor = IDdictionary[provinceIDCounter];
                    }
                }
                if (x % 50 == 0) {
                    Console.WriteLine("X = " + x);
                }
            }

            //provincesMap.Save(PATH + endFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //map.Save(PATH + endFile + ".png", System.Drawing.Imaging.ImageFormat.Png);

            return true;
        }

        private static Bitmap fillShape(Color oldColor, Color newColor, int x, int y, Bitmap map) //https://stackoverflow.com/questions/452480/is-there-an-algorithm-to-determine-contiguous-colored-regions-in-a-grid
        {
            Stack<axis2d> stack = new Stack<axis2d>();
            stack.Push(new axis2d(x, y));
            while (stack.Count != 0)
            {
                axis2d cell = stack.Pop();
                if (map.GetPixel(cell.X, cell.Y) == newColor)
                {
                    continue;
                }
                map.SetPixel(cell.X, cell.Y, newColor);

                bool underX = false;
                bool overX = false;
                bool underY = false;
                bool overY = false;

                if (!(cell.X - 1 < 0))
                {
                    underX = true;
                }
                if (!(cell.X + 1 > map.Width - 1))
                {
                    overX = true;
                }
                if (!(cell.Y - 1 < 0))
                {
                    underY = true;
                }
                if (!(cell.Y + 1 > map.Height - 1))
                {
                    overY = true;
                }

                if (underX && underY) // top left
                {
                    if (map.GetPixel(cell.X - 1, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y - 1));
                    }
                }
                if (underX)
                {// left
                    if (map.GetPixel(cell.X - 1, cell.Y) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y));
                    }
                }
                if (underX && overY)
                { // bottom left
                    if (map.GetPixel(cell.X - 1, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y + 1));
                    }
                }

                if (underY) // top
                {
                    if (map.GetPixel(cell.X, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X, cell.Y - 1));
                    }
                }
                if (overY)//bottom
                {
                    if (map.GetPixel(cell.X, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X, cell.Y + 1));
                    }
                }

                if (overX && underY) // top right
                {
                    if (map.GetPixel(cell.X + 1, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y - 1));
                    }
                }
                if (overX)// right
                {
                    if (map.GetPixel(cell.X + 1, cell.Y) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y));
                    }
                }
                if (overX && overY)// bottom right
                {
                    if (map.GetPixel(cell.X + 1, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y + 1));
                    }
                }
            }
            //Console.WriteLine("ShapeSize = " + shapeSize);
            return map;
        }

        private static void loadColors()
        {
            //ColorToIDDict = new Dictionary<Color, int>();
            using (var reader = new StreamReader(definitionPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (line.Trim().Length == 0)//Prevent crash for next if statement, skip empty line
                    {
                        continue;
                    }
                    if (line.Trim().ElementAt(0) == '#')//Ignore commented out lines, skip
                    {
                        continue;
                    }

                    Color tempColor = Color.FromArgb(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                    int tempID = int.Parse(values[0]);
                    IDdictionary.Add(tempID, tempColor);
                    //ColorToIDDict.Add(tempColor, tempID);
                    colorHashSet.Add(tempColor);
                }
            }
            //for(int v = 0; v < colorHashSet.Count; v++ ){ //Remove hidden greys. How did they get here?
            //    if (colorHashSet.ElementAt(v).R == colorHashSet.ElementAt(v).B && colorHashSet.ElementAt(v).R == colorHashSet.ElementAt(v).G && colorHashSet.ElementAt(v).B == colorHashSet.ElementAt(v).G) {
            //        colorHashSet.Remove(colorHashSet.ElementAt(v));
            //        Console.WriteLine("Grey color found: " + colorHashSet.ElementAt(v).ToString());
            //    }
            //}
        }

        /*static Dictionary<Color, int> ColorToIDDict;
        static HashSet<int> usedIDs = new HashSet<int>();
        static private bool checkPreExistingColors()
        {
            //Bitmap map = new Bitmap(PATH + endFile + ".png");
            //int uniqueProvinceCounter = 0;
            //int defaultProvinceCounter = 0;
            HashSet<Color> foundColor = new HashSet<Color>();
            //provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < provincesMap.Width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < provincesMap.Height; y = y + 3)
                {
                    Color tempColor = provincesMap.GetPixel(x, y);

                    if (foundColor.Add(tempColor)) //Triggers first time for each color encountered only
                    {
                        if (ColorToIDDict.ContainsKey(tempColor)) //Unique color spotted
                        {
                            usedIDs.Add(ColorToIDDict[tempColor]); //Add it to usedID's list
                        }
                    }
                    else
                    {

                    }
                }
            }
            if (foundColor.Count == usedIDs.Count)
            {
                return false;
            }

            return true;
        }*/
        private struct axis2d
        {
            public axis2d(int x, int y)
            {
                X = x;
                Y = y;
            }
            public int X { get; }
            public int Y { get; }
        }
    }
}
