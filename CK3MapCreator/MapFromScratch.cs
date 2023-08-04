using CK3MapCreator.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    //Needs initial map file, definition.csv, terrain.csv, 00_province_terrain, black heightmap
    internal class MapFromScratch
    {
        Dictionary<int, Color> IDtoColor = new Dictionary<int, Color>();
        HashSet<Color> KnownColors = new HashSet<Color>(); // Used to efficiently iterate through used colors
        Logger logger = Logger.getLogger();
        FileLoader file = FileLoader.getFileLoader();
        List<ProvinceCK3> provinces = new List<ProvinceCK3>(); 

        public void doThings()
        {
            //provincesMap = new Bitmap(PATH + INPUT + originalFile + ".png");
            provincesMap = new Bitmap(file.getFilePath("mapv1.png"));
            //provincesMap.Save(PATH + "temp\\" + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            loadColors(); //Load colors from definitions.csv for use in assigning to provinces
            logger.Log("Colors loaded");

            loadTerrains(); //Load terrain csv file for use in assigning terrains
            logger.Log("Terrains loaded");

            assignTerrain(); //Process terrain data
            logger.Log("Terrains processed");

            assignProvinces(); //Create provinces and export province map
            logger.Log("Provinces assigned");

            generateProvinceTerrainFile(); //Export terrain values to file
            logger.Log("Terrains exported");

            generateHeightmap();
            logger.Log("Heightmap generated");
        }

        //Reads through the definition file and assosiates province ID's to their colors
        private void loadColors()
        {
            using (var reader = new StreamReader(file.getFilePath("definition.csv")))
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

                    Color newColor = Color.FromArgb(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                    int tempID = int.Parse(values[0]);
                    IDtoColor.Add(tempID, newColor);
                    KnownColors.Add(newColor);
                }
            }
            //for(int v = 0; v < colorHashSet.Count; v++ ){ //Remove hidden greys. How did they get here?
            //    if (colorHashSet.ElementAt(v).R == colorHashSet.ElementAt(v).B && colorHashSet.ElementAt(v).R == colorHashSet.ElementAt(v).G && colorHashSet.ElementAt(v).B == colorHashSet.ElementAt(v).G) {
            //        colorHashSet.Remove(colorHashSet.ElementAt(v));
            //        Console.WriteLine("Grey color found: " + colorHashSet.ElementAt(v).ToString());
            //    }
            //}
        }
        Dictionary<Color, string> terrainDict = new Dictionary<Color, string>();
        Dictionary<string, string> terrainGroups = new Dictionary<string, string>();
        //Reads through the terrain config file and assosiates terrain colors to its name, and name to its group
        private void loadTerrains()
        {
            using (var reader = new StreamReader(file.getFilePath("terrain.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }
                    if (line.Trim().ElementAt(0) == '#')//Ignore commented out lines, skip
                    {
                        continue;
                    }

                    Color tempColor = Color.FromArgb(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                    terrainDict.Add(tempColor, values[0]);
                    terrainGroups.Add(values[0], values[4]);
                }
            }
        }
        //Keep an internal reference to all provinces of this culture

        Bitmap provincesMap;
        int provinceIDCounter = 1;

        // Look through the whole province map, if the color of this location is red(should be one pixel) then get the province's color from the ID to color list,
        // Set the color to the province color, try changing up and down and left colors?, Add newly created provinces to list
        private void assignTerrain()
        {
            int width = provincesMap.Width;
            int height = provincesMap.Height;

            Color newColor;

            for (int x = 0; x < width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < height; y = y + 1)
                {
                    Color tempColor = provincesMap.GetPixel(x, y);

                    if (tempColor == Color.FromArgb(255, 0, 0))
                    {
                        newColor = IDtoColor[provinceIDCounter];
                        //Console.WriteLine("Red found at x:" + x + " - y:" + y);
                        provincesMap.SetPixel(x, y, newColor);

                        string terrain;
                        try
                        {
                            Color downColor = provincesMap.GetPixel(x, y - 1);
                            //Console.WriteLine("Color = " + downColor.ToString());
                            //Console.WriteLine("Terrain = " + terrainDict[downColor]);
                            //Console.WriteLine("Group = " + terrainGroups[terrainDict[downColor]]);
                            terrain = terrainDict[downColor];
                        }
                        catch
                        {
                            try
                            {
                                Color upColor = provincesMap.GetPixel(x, y + 1);
                                terrain = terrainDict[upColor];
                            }
                            catch
                            {
                                Color leftColor = provincesMap.GetPixel(x - 1, y);
                                terrain = terrainDict[leftColor];
                            }
                        }
                        provinces.Add(new ProvinceCK3 { color = newColor, x = x, y = y, ID = provinceIDCounter,  terrain = terrain });
                        provinceIDCounter++;
                    }
                }
                //Console.WriteLine("x = " + x + "/" + width);
            }
        }

        // Create an entry in the stack for every province, move in all directions 
        private void assignProvinces()
        {
            List<Queue<stackProvinceObject>> stackList = new List<Queue<stackProvinceObject>>();
            int currentGeneration = 0;

            foreach (ProvinceCK3 p in provinces)
            {
                stackList.Add(new Queue<stackProvinceObject>()); //New stack for each province
                stackList[stackList.Count - 1].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = p.color, x = p.x, y = p.y, ID = p.ID, terrain = p.terrain}, generation = 0 }); //Starting pixel for each province
            }

            //Console.WriteLine("Province Stacks " + stackList.Count);

            while (stackList.Count != 0) //Remove not-expanding province as loop goes on
            {
                for (int a = 0; a < stackList.Count; a++) //Every province not full
                {
                    if (stackList[a].Count < 1)
                    { //Empty stack
                      //Console.WriteLine("debug empty stack 1");
                        stackList.RemoveAt(a);
                        a--; //Removed from list, go back
                        continue;
                    }
                    while (stackList[a].Peek().generation == currentGeneration) //
                    {
                        stackProvinceObject currentObject = stackList[a].Dequeue();
                        if (!(terrainDict.ContainsKey(provincesMap.GetPixel(currentObject.prov.x, currentObject.prov.y))) && currentObject.generation != 0)
                        {
                            //Only change terrain values
                        }
                        else
                        {
                            provincesMap.SetPixel(currentObject.prov.x, currentObject.prov.y, currentObject.prov.color);
                            //Console.WriteLine("pixel x:" + currentObject.coords.X + " - y:" + currentObject.coords.Y + " - color: " + currentObject.color.ToString());

                            bool underX = false;
                            bool overX = false;
                            bool underY = false;
                            bool overY = false;

                            if (!(currentObject.prov.x - 1 < 0))
                            {
                                underX = true;
                            }
                            if (!(currentObject.prov.x + 1 > provincesMap.Width - 1))
                            {
                                overX = true;
                            }
                            if (!(currentObject.prov.y - 1 < 0))
                            {
                                underY = true;
                            }
                            if (!(currentObject.prov.y + 1 > provincesMap.Height - 1))
                            {
                                overY = true;
                            }

                            var rand = new Random();
                            string terrain;
                            if (underX && underY) // top left
                            {
                                if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.x - 1, currentObject.prov.y - 1), out terrain))
                                {
                                    if (terrainGroups[terrain] == terrainGroups[currentObject.prov.terrain])
                                    {
                                        stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x - 1, y = currentObject.prov.y - 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                    }
                                }
                                if (rand.Next(2) == 1)
                                {
                                    stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x - 1, y = currentObject.prov.y - 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                }
                            }
                            if (underX)// left
                            {
                                stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x - 1, y = currentObject.prov.y, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });

                                if (rand.Next(5) == 1)
                                {
                                    if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.x - 2, currentObject.prov.y), out terrain))
                                    {
                                        if (terrainGroups[terrain] == terrainGroups[currentObject.prov.terrain])
                                        {
                                            stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x - 2, y = currentObject.prov.y, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                        }
                                    }
                                }
                            }
                            if (underX && overY)// bottom left
                            {
                                if (rand.Next(2) == 1)
                                {
                                    stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x - 1, y = currentObject.prov.y + 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                }
                            }

                            if (underY) // top
                            {
                                stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x, y = currentObject.prov.y - 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });

                                if (rand.Next(5) == 1)
                                {
                                    if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.x, currentObject.prov.y - 2), out terrain))
                                    {
                                        if (terrainGroups[terrain] == terrainGroups[currentObject.prov.terrain])
                                        {
                                            stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x, y = currentObject.prov.y - 2, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                        }
                                    }
                                }
                            }
                            if (overY)//bottom
                            {
                                stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x, y = currentObject.prov.y + 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });

                                if (rand.Next(5) == 1)
                                {
                                    if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.x, currentObject.prov.y + 2), out terrain))
                                    {
                                        if (terrainGroups[terrain] == terrainGroups[currentObject.prov.terrain])
                                        {
                                            stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x, y = currentObject.prov.y + 2, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                        }
                                    }
                                }
                            }

                            if (overX && underY) // top right
                            {
                                if (rand.Next(2) == 1)
                                {
                                    stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x + 1, y = currentObject.prov.y - 1, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                }
                            }
                            if (overX)// right
                            {
                                stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, x = currentObject.prov.x + 1, y = currentObject.prov.y, ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });

                                if (rand.Next(5) == 1)
                                {
                                    if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.x + 2, currentObject.prov.y), out terrain))
                                    {
                                        if (terrainGroups[terrain] == terrainGroups[currentObject.prov.terrain])
                                        {
                                            stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, provCoord = new axis2d(currentObject.prov.provCoord.X + 2, currentObject.prov.provCoord.Y), ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                        }
                                    }
                                }
                            }
                            if (overX && overY)// bottom right
                            {
                                if (rand.Next(2) == 1)
                                {
                                    stackList[a].Enqueue(new stackProvinceObject() { prov = new ProvinceCK3() { color = currentObject.prov.color, provCoord = new axis2d(currentObject.prov.provCoord.X + 1, currentObject.prov.provCoord.Y + 1), ID = currentObject.prov.ID, terrain = currentObject.prov.terrain }, generation = currentObject.generation + 1 });
                                }
                            }
                        }
                        if (stackList[a].Count == 0)
                        { //Empty stack
                          //Console.WriteLine("debug empty stack 2");
                            stackList.RemoveAt(a);
                            a--; //Removed from list, go back
                            break;
                        }
                    }
                }
                Console.WriteLine("Generation " + currentGeneration + " complete");
                //provincesMap.Save(PATH + "generationFile" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp); //Output a file everytime, debug only
                currentGeneration++;
            }
            provincesMap.Save(file.getFilePath("outputfile.bmp"), System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private struct stackProvinceObject
        {
            public ProvinceCK3 prov;
            public int generation;
        }

        private void generateProvinceTerrainFile()
        {
            using (FileStream fs = File.Create(file.getFilePath("00_province_terrain.txt")))
            {
                AddText(fs, "default=plains\n");
                foreach (ProvinceCK3 p in provinces)
                {
                    string line = p.ID.ToString() + "=" + p.terrain + "\n";
                    AddText(fs, line);
                    //Console.WriteLine("Province " + p.ID.ToString() + " with terrain " + p.terrain);
                }
            }
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        private void generateHeightmap()
        {
            Bitmap heightMap = new Bitmap(file.getFilePath("heightmapBlank.png")); //Non-empty heightmap
            Bitmap terrainMap = new Bitmap(file.getFilePath("testmap2.png")); //Non-empty heightmap 
                                                                              //Console.WriteLine(heightMap.GetPixel(0,0).ToString());
                                                                              //heightMap.SetResolution(terrainMap.Width, terrainMap.Height);
            for (int x = 0; x < terrainMap.Width; x = x + 1)
            {
                for (int y = 0; y < terrainMap.Height; y = y + 1)
                {
                    if (terrainDict.ContainsKey(terrainMap.GetPixel(x, y)))
                    {
                        heightMap.SetPixel(x, y, Color.FromArgb(255, 20, 20, 20));
                    }
                }
            }
            heightMap.Save(file.getFilePath("heightmap.png"), System.Drawing.Imaging.ImageFormat.Png);

            heightMap.Dispose();
            terrainMap.Dispose();
        }
    }
}
