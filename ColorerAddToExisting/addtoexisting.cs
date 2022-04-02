using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ColorerAddToExisting
{
    //Console.WriteLine("args1: {0} args2: {1}", value1, value2);
    class colorer
    {
        static int provinceIDCounter = 1;
        const string PATH = "C:\\Users\\Damian\\Desktop\\ck3autotestzone\\zone4\\";
        const string originalFile = "provincesgrey";
        const string endFile = "provincesend";
        const string processingFile = "provincesprocessing";
        //const int TOTALPROVINCES = 9459;
        static Dictionary<int, Color> IDdictionary = new Dictionary<int, Color>(); //Used for getting colors from ID
        static HashSet<Color> colorHashSet = new HashSet<Color>(); // Used to efficiently iterate through used colors
        static Bitmap mapWarning;

        static void Main(string[] args)
        {
            Console.WriteLine("Loading colors");
            loadColors();
            Console.WriteLine("Processing colors");
            checkUniqueColors();
            Console.WriteLine("Processing image");
            loadProvincesGray();

            //

            Console.WriteLine("\n\nFinished");
            
        } 
        static public bool loadProvincesGray()
        {
            Bitmap provincesMap = new Bitmap(PATH + originalFile + ".png");
            mapWarning = new Bitmap(PATH + originalFile + ".png");
            provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < provincesMap.Width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < provincesMap.Height; y = y + 1)
                {
                    Color tempColor = provincesMap.GetPixel(x, y);

                    //if (tempColor == Color.Black) {
                    //    Console.WriteLine("Black");
                    //    continue;
                    //}

                    //provincesMap.Save(PATH + "provincesgrey.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                    //Console.WriteLine("Working x = " + x + " / y = " + y);

                    if (!colorHashSet.Contains(tempColor))//Return false if color already present
                    {
                        provincesMap = fillShape(tempColor, newColor, x, y, provincesMap);

                        if (provinceIDCounter % 100 == 0)
                        {
                            provincesMap.Save(PATH + processingFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                            mapWarning.Save(PATH + "warnings.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                        }

                        provinceIDCounter++;
                        newColor = IDdictionary[provinceIDCounter];
                    }
                    else
                    {

                    }
                }
                Console.WriteLine("\rx = " + x + "/" + provincesMap.Width);
                //Console.WriteLine(" - unique colors = " + provinceIDCounter);
            }

            provincesMap.Save(PATH + endFile + ".png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine("Provinces = " + provinceIDCounter);

            return true;
        }

        public static void fillWarning(Color oldColor, int danger, int x, int y) //https://stackoverflow.com/questions/452480/is-there-an-algorithm-to-determine-contiguous-colored-regions-in-a-grid
        {
            Stack<axis2d> stack = new Stack<axis2d>();
            stack.Push(new axis2d(x, y));
            int shapeSize = 0;
            Color warningColor = Color.FromArgb(255, 255, 0);
            switch (danger)
            {
                case 0:
                    warningColor = Color.FromArgb(0, 255, 0); //Green
                    break;
                case 1:
                    warningColor = Color.FromArgb(0, 0, 255); //Blue
                    break;
                case 2:
                    warningColor = Color.FromArgb(255, 0, 0); //Red
                    break;
            }


            while (stack.Count != 0)
            {
                axis2d cell = stack.Pop();
                if (mapWarning.GetPixel(cell.X, cell.Y) == warningColor)
                {
                    continue;
                }
                mapWarning.SetPixel(cell.X, cell.Y, warningColor);
                shapeSize++;

                bool underX = false;
                bool overX = false;
                bool underY = false;
                bool overY = false;

                if (!(cell.X - 1 < 0))
                {
                    underX = true;
                }
                if (!(cell.X + 1 > mapWarning.Width - 1))
                {
                    overX = true;
                }
                if (!(cell.Y - 1 < 0))
                {
                    underY = true;
                }
                if (!(cell.Y + 1 > mapWarning.Height - 1))
                {
                    overY = true;
                }

                if (underX && underY) // top left
                {
                    if (mapWarning.GetPixel(cell.X - 1, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y - 1));
                    }
                }
                if (underX)
                {// left
                    if (mapWarning.GetPixel(cell.X - 1, cell.Y) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y));
                    }
                }
                if (underX && overY)
                { // bottom left
                    if (mapWarning.GetPixel(cell.X - 1, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X - 1, cell.Y + 1));
                    }
                }

                if (underY) // top
                {
                    if (mapWarning.GetPixel(cell.X, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X, cell.Y - 1));
                    }
                }
                if (overY)//bottom
                {
                    if (mapWarning.GetPixel(cell.X, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X, cell.Y + 1));
                    }
                }

                if (overX && underY) // top right
                {
                    if (mapWarning.GetPixel(cell.X + 1, cell.Y - 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y - 1));
                    }
                }
                if (overX)// right
                {
                    if (mapWarning.GetPixel(cell.X + 1, cell.Y) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y));
                    }
                }
                if (overX && overY)// bottom right
                {
                    if (mapWarning.GetPixel(cell.X + 1, cell.Y + 1) == oldColor)
                    {
                        stack.Push(new axis2d(cell.X + 1, cell.Y + 1));
                    }
                }
            }
        }

        public static Bitmap fillShape(Color oldColor, Color newColor, int x, int y, Bitmap map) //https://stackoverflow.com/questions/452480/is-there-an-algorithm-to-determine-contiguous-colored-regions-in-a-grid
        {
            Stack<axis2d> stack = new Stack<axis2d>();
            stack.Push(new axis2d(x, y));
            int shapeSize = 0;
            while (stack.Count != 0)
            {
                axis2d cell = stack.Pop();
                if (map.GetPixel(cell.X, cell.Y) == newColor)
                {
                    continue;
                }
                map.SetPixel(cell.X, cell.Y, newColor);
                shapeSize++;

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

            if (shapeSize < 300) //Replace with constant
            {
                Bitmap greyMap = new Bitmap(PATH + originalFile + ".png"); //If takes too long, make new solution
                Console.WriteLine("Small province detect at pixel (" + x + ", " + y + ")");
                for (int a = x - 50; a < x + 50; a = a + 1)
                {
                    for (int b = y - 50; b < y + 50; b = b + 1)
                    {
                        if (a < 0)
                        {
                            continue;
                        }
                        if (b < 0)
                        {
                            continue;
                        }
                        if (a >= map.Width - 1)
                        {
                            continue;
                        }
                        if (b >= map.Height - 1)
                        {
                            continue;
                        }
                        Color tempColor = map.GetPixel(a, b);

                        if (tempColor == oldColor)
                        {
                            Console.WriteLine("right compatibile province detect at pixel (" + a + ", " + b + ")");
                            fillWarning(tempColor, 1, a, b);
                            fillWarning(oldColor, 0, x, y);
                            return fillShape(oldColor, newColor, a, b, map);
                        }
                        //if (tempColor == greyMap.GetPixel(a, b) && !(newColor == greyMap.GetPixel(a, b)) && ) //Find province that is unique color and used to be specific grey color and is 
                        //{
                        //    Console.WriteLine("special compatibile province detect at pixel (" + a + ", " + b + ")");
                        //    return fillShape(tempColor, newColor, a, b, map);
                        //}
                        if (tempColor != newColor && colorHashSet.Contains(tempColor) && greyMap.GetPixel(a, b) == oldColor) //Left, find new unique color with old grey color. Replace current color with new unique.
                        {
                            Console.WriteLine("special compatibile province detect at pixel (" + a + ", " + b + ")");
                            //fillWarning(oldColor, 2, a, b);
                            provinceIDCounter--; //Free last used color
                            return fillShape(newColor, tempColor, x, y, map);
                        }
                    }
                }
                //fillWarning(oldColor, 0, x, y);
                Console.WriteLine("No compat found");
            }
            // 2 goals 
            // Small provinces together like lakes and islands
            //Small land into bigger province

            //if size is under 20
            //  search for nearby province of similar color within 300 pixels
            //  if found
            //if new prov same grey
            // color both with new color
            //if new prov unique color
            //color new prov with new color

            //Console.WriteLine("ShapeSize = " + shapeSize);
            return map;
        }

        public static void loadColors()
        {
            using (var reader = new StreamReader(PATH + "definition.csv"))
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
        //Read map, record all colors in definitions.txt
        //For all the others, apply unused colors 
        static public bool checkUniqueColors()
        {
            Bitmap provincesMap = new Bitmap(PATH + endFile + ".png");
            int uniqueProvinceCounter = 0;
            int defaultProvinceCounter = 0;
            //provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < provincesMap.Width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < provincesMap.Height; y = y + 3)
                {
                    Color tempColor = provincesMap.GetPixel(x, y);

                    if (colorHashSet.Contains(tempColor))//Return false if color already present
                    {
                        //provincesMap = fillShape(tempColor, newColor, x, y, provincesMap);
                        //Console.WriteLine("Detected unique color");
                        uniqueProvinceCounter++;
                    }
                    else
                    {
                        defaultProvinceCounter++;
                    }
                }
                Console.Write("\rx = " + x + "/" + provincesMap.Width);
            }
            Console.WriteLine("\rUnique provinces = " + uniqueProvinceCounter);
            Console.WriteLine("Old provinces = " + defaultProvinceCounter);
            Console.WriteLine("Total provinces = " + defaultProvinceCounter + uniqueProvinceCounter);

            //provincesMap.Save(PATH + "ungreyed.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //Console.WriteLine("Provinces = " + provinceIDCounter);

            return true;
        }

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
    }
}
