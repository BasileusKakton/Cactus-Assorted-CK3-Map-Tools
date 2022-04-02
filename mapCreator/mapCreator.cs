using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Drawing;

namespace mapCreator
{
	class mapCreator
	{
		//get the location of all province cores, assign color to each
		//Make array of stacks for each province
		//For each stack, go one in all direction until color is taken color reached

		static string PATH = "C:\\Users\\Damian\\Desktop\\ck3autotestzone\\zone7\\";
		static string originalFile = "testmap2";
		static string outputFile = "output";
		static Bitmap provincesMap;
		static Dictionary<int, Color> IDdictionary = new Dictionary<int, Color>();
		static HashSet<Color> colorHashSet = new HashSet<Color>(); // Used to efficiently iterate through used colors
		static int provinceIDCounter = 1;
		static List<province> provinces = new List<province>();
		static Dictionary<Color, string> terrainDict = new Dictionary<Color, string>();
		static Dictionary<string, string> terrainGroups = new Dictionary<string, string>();

		static void Main(string[] args)
		{
			provincesMap = new Bitmap(PATH + originalFile + ".png");
			provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
			loadColors();
			assignTerrains();
			assignLocation();
			spreadProvinces();
			generateProvinceTerrainFile();
		}

		static public void generateProvinceTerrainFile()
		{
			using (FileStream fs = File.Create(PATH + "00_province_terrain.txt"))
			{
				AddText(fs, "default=plains\n");
				foreach (province p in provinces) 
				{
					string line = p.provID.ToString() + "=" + p.preferedTerrain + "\n";
					AddText(fs, line);
					Console.WriteLine("Province " + p.provID.ToString() + " with terrain " + p.preferedTerrain);
				}
			}
		}

		private static void AddText(FileStream fs, string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			fs.Write(info, 0, info.Length);
		}

		static public void assignTerrains()
		{
			using (var reader = new StreamReader(PATH + "terrain.csv"))
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
					terrainDict.Add(tempColor, values[0]);
					terrainGroups.Add(values[0], values[4]);
				}
			}
		}
		static public void spreadProvinces()
		{
			List<Queue<stackProvinceObject>> stackList = new List<Queue<stackProvinceObject>>();
			int currentGeneration = 0;

			foreach (province p in provinces) {
				stackList.Add(new Queue<stackProvinceObject>()); //New stack for each province
				stackList[stackList.Count - 1].Enqueue(new stackProvinceObject() { prov = new province() { provColor = p.provColor, provCoord = p.provCoord, provID = p.provID, preferedTerrain = p.preferedTerrain }, generation = 0 }); //Starting pixel for each province
			}

			Console.WriteLine("Province Stacks " + stackList.Count);

			while (stackList.Count != 0) //Remove not-expanding province as loop goes on
			{
				for (int a = 0; a < stackList.Count; a++) //Every province not full
				{
					if (stackList[a].Count < 1) { //Empty stack
						Console.WriteLine("debug empty stack 1");
						stackList.RemoveAt(a);
						a--; //Removed from list, go back
						continue;
					}
					while (stackList[a].Peek().generation == currentGeneration) //
					{
						stackProvinceObject currentObject = stackList[a].Dequeue();
						if (!( terrainDict.ContainsKey(provincesMap.GetPixel(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y))) && currentObject.generation != 0 )//If not special green, skip 
						{
							//Console.WriteLine("province not green");
						}
						else
						{
							provincesMap.SetPixel(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y, currentObject.prov.provColor);
							//Console.WriteLine("pixel x:" + currentObject.coords.X + " - y:" + currentObject.coords.Y + " - color: " + currentObject.color.ToString());

							bool underX = false;
							bool overX = false;
							bool underY = false;
							bool overY = false;

							if (!(currentObject.prov.provCoord.X - 1 < 0))
							{
								underX = true;
							}
							if (!(currentObject.prov.provCoord.X + 1 > provincesMap.Width - 1))
							{
								overX = true;
							}
							if (!(currentObject.prov.provCoord.Y - 1 < 0))
							{
								underY = true;
							}
							if (!(currentObject.prov.provCoord.Y + 1 > provincesMap.Height - 1))
							{
								overY = true;
							}

							var rand = new Random();
							string terrain;
							if (underX && underY) // top left
							{
								if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.provCoord.X - 1, currentObject.prov.provCoord.Y - 1), out terrain)) {
									if (terrainGroups[terrain] == terrainGroups[currentObject.prov.preferedTerrain])
									{
										stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X - 1, currentObject.prov.provCoord.Y - 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
									}
								}
								if (rand.Next(2) == 1)
								{
									stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X - 1, currentObject.prov.provCoord.Y - 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
								}
							}
							if (underX)// left
							{
								stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X - 1, currentObject.prov.provCoord.Y), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });

								if (rand.Next(5) == 1)
								{
									if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.provCoord.X - 2, currentObject.prov.provCoord.Y), out terrain))
									{
										if (terrainGroups[terrain] == terrainGroups[currentObject.prov.preferedTerrain])
										{
											stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X - 2, currentObject.prov.provCoord.Y), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
										}
									}
								}
							}
							if (underX && overY)// bottom left
							{
								if (rand.Next(2) == 1) 
								{
									stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X - 1, currentObject.prov.provCoord.Y + 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
								}
							}

							if (underY) // top
							{
								stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y - 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });

								if (rand.Next(5) == 1)
								{
									if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y - 2), out terrain))
									{
										if (terrainGroups[terrain] == terrainGroups[currentObject.prov.preferedTerrain])
										{
											stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y - 2), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
										}
									}
								}
							}
							if (overY)//bottom
							{
								stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y + 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });

								if (rand.Next(5) == 1)
								{
									if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y + 2), out terrain))
									{
										if (terrainGroups[terrain] == terrainGroups[currentObject.prov.preferedTerrain])
										{
											stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y + 2), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1});
										}
									}
								}
							}

							if (overX && underY) // top right
							{
								if (rand.Next(2) == 1)
								{
									stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X + 1, currentObject.prov.provCoord.Y - 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
								}
							}
							if (overX)// right
							{
								stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X + 1, currentObject.prov.provCoord.Y), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });

								if (rand.Next(5) == 1)
								{
									if (terrainDict.TryGetValue(provincesMap.GetPixel(currentObject.prov.provCoord.X + 2, currentObject.prov.provCoord.Y), out terrain))
									{
										if (terrainGroups[terrain] == terrainGroups[currentObject.prov.preferedTerrain])
										{
											stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X + 2, currentObject.prov.provCoord.Y), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
										}
									}
								}
							}
							if (overX && overY)// bottom right
							{
								if (rand.Next(2) == 1)
								{
									stackList[a].Enqueue(new stackProvinceObject() { prov = new province() { provColor = currentObject.prov.provColor, provCoord = new axis2d(currentObject.prov.provCoord.X + 1, currentObject.prov.provCoord.Y + 1), provID = currentObject.prov.provID, preferedTerrain = currentObject.prov.preferedTerrain }, generation = currentObject.generation + 1 });
								}
							}
						}
						if (stackList[a].Count == 0)
						{ //Empty stack
							Console.WriteLine("debug empty stack 2");
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
			provincesMap.Save(PATH + "spreadfile" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
		}
		static public void assignLocation()
		{
			int width = provincesMap.Width;
			int height = provincesMap.Height;

			Color newColor;

			for (int x = 0; x < width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
			{
				for (int y = 0; y < height; y = y + 1)
				{
					Color tempColor = provincesMap.GetPixel(x, y);

					if (tempColor == Color.FromArgb(255,0,0)) 
					{
						newColor = IDdictionary[provinceIDCounter];
						Console.WriteLine("Red found at x:" + x + " - y:" + y);
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
						provinces.Add(new province { provColor = newColor, provCoord = new axis2d(x, y), provID = provinceIDCounter, preferedTerrain = terrain });
						provinceIDCounter++;
					}
				}
				Console.WriteLine("x = " + x + "/" + width);
			}
		}

		static public void loadColors()
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
		public struct province
		{
			public Color provColor;
			public axis2d provCoord;
			public int provID;
			public string preferedTerrain;
			//public string provName;
			//public string culture;
			//public string religion;
			//public string government;

			//public string provHistoryPath;
		}

		public struct stackProvinceObject {
			public province prov;
			public int generation;
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
