using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Drawing;
using Newtonsoft.Json;

namespace mapCreator
{
	class mapCreator
	{
		//get the location of all province cores, assign color to each
		//Make array of stacks for each province
		//For each stack, go one in all direction until color is taken color reached

		static string PATH = "C:\\Users\\Damian\\Desktop\\ck3autotestzone\\zone8\\";
		static string INPUT = "input\\";
		static string OUTPUT = "output\\";
		static string CONFIG = "config\\";
		//static string TEMP = "temp\\";
		static string originalFile = "testmap2";
		//static string outputFile = "output";
		static Bitmap provincesMap;
		static Dictionary<int, Color> IDdictionary = new Dictionary<int, Color>();
		static HashSet<Color> colorHashSet = new HashSet<Color>(); // Used to efficiently iterate through used colors
		static int provinceIDCounter = 1;
		static List<province> provinces = new List<province>();
		static Dictionary<Color, string> terrainDict = new Dictionary<Color, string>();
		static Dictionary<string, string> terrainGroups = new Dictionary<string, string>();

		//00_landed_titles

		static void Main(string[] args)
		{
			provincesMap = new Bitmap(PATH + INPUT + originalFile + ".png");
			//provincesMap.Save(PATH + "temp\\" + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
			loadColors(); //Load colors from definitions.csv for use in assigning to provinces
			Console.WriteLine("Colors loaded");
			loadTerrains(); //Load terrain csv file for use in assigning terrains
			Console.WriteLine("Terrains loaded");
			assignTerrain(); //Process terrain data
			Console.WriteLine("Terrains processed");
			assignProvinces(); //Create provinces and export province map
			Console.WriteLine("Provinces assigned");
			generateProvinceTerrainFile(); //Export terrain values to file
			Console.WriteLine("Terrains exported");

			if (true) // Automatic color title files
			{
				Bitmap tempMap;
				tempMap = greyToColor.process( PATH + INPUT + "definition.csv", PATH + INPUT + "countiesgrey.png");
				tempMap.Save(PATH + INPUT + "counties.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				Console.WriteLine("counties colored");

				tempMap = greyToColor.process(PATH + INPUT + "definition.csv", PATH + INPUT + "duchiesgrey.png");
				tempMap.Save(PATH + INPUT + "duchies.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				Console.WriteLine("duchies colored");

				tempMap = greyToColor.process(PATH + INPUT + "definition.csv", PATH + INPUT + "kingdomsgrey.png");
				tempMap.Save(PATH + INPUT + "kingdoms.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				Console.WriteLine("kingdoms colored");

				tempMap = greyToColor.process(PATH + INPUT + "definition.csv", PATH + INPUT + "empiresgrey.png");
				tempMap.Save(PATH + INPUT + "empires.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
				Console.WriteLine("empires colored");

				// Assign color values to png files
				// Output config file
				// Output generated png
			}
			loadTitleConfig();      //Load title data from csv files
			processTitles();        //Process the titles internally

			jsonLandedTitlesOutput();
			generateLandedTitles(); //Output the landed titles
		}



		private static void generateLandedTitles() 
		{
			using (FileStream fs = File.Create(PATH + OUTPUT + "00_landed_titles.txt"))
			{
				AddText(fs, "@correct_culture_primary_score = 100\n@better_than_the_alternatives_score = 50\n@always_primary_score = 1000\n\n");
				foreach (empireObject e in empireList)
				{
					List<string> lines = new List<string>();
					lines.Add("e_" + e.name + " = {\n");
					lines.Add("\tcolor = { " + "255 255 255" + " }\n"); //Format this
					lines.Add("\tcolor2 = { " + "255 255 255" + " }\n\n");
					lines.Add("\tcapital = " + "c_" + e.kingdoms[0].duchies[0].counties[0].name + "\n\n");

					foreach (kingdomObject k in e.kingdoms) 
					{
						lines.Add("\tk_" + k.name + " = {\n");
						lines.Add("\t\tcolor = { " + "255 255 255" + " }\n"); //Format this
						lines.Add("\t\tcolor2 = { " + "255 255 255" + " }\n\n");
						lines.Add("\t\tcapital = " + "c_" + k.duchies[0].counties[0].name + "\n\n");

						foreach (duchyObject d in k.duchies) 
						{
							lines.Add("\t\td_" + d.name + " = {\n");
							lines.Add("\t\t\tcolor = { " + "255 255 255" + " }\n"); //Format this
							lines.Add("\t\t\tcolor2 = { " + "255 255 255" + " }\n\n");
							lines.Add("\t\t\tcapital = " + "c_" + d.counties[0].name + "\n\n");

							foreach (countyObject c in d.counties) 
							{
								lines.Add("\t\t\tc_" + c.name + " = {\n");
								lines.Add("\t\t\t\tcolor = { " + "255 255 255" + " }\n"); //Format this
								lines.Add("\t\t\t\tcolor2 = { " + "255 255 255" + " }\n\n");

								foreach (province p in c.provinces) 
								{
									lines.Add("\t\t\t\tb_" + p.provID + " = {\n");
									lines.Add("\t\t\t\t\tprovince = " + p.provID + "\n");
									lines.Add("\t\t\t\t\tcolor = { " + "255 255 255" + " }\n"); //Format this
									lines.Add("\t\t\t\t\tcolor2 = { " + "255 255 255" + " }\n\n");

									lines.Add("\t\t\t\t}\n");
								}
								lines.Add("\t\t\t}\n");
							}
							lines.Add("\t\t}\n");
						}
						lines.Add("\t}\n");
					}
					lines.Add("}\n");
					foreach (string s in lines) {
						AddText(fs, s);
					}
					//Console.WriteLine("Province " + p.provID.ToString() + " with terrain " + p.preferedTerrain);
				}
			}
		}

		private static void jsonLandedTitlesOutput()
		{
			using (FileStream fs = File.Create(PATH + OUTPUT + "json.txt"))
			{
				string s = JsonConvert.SerializeObject(empireList, Formatting.Indented);
				AddText(fs, s);
				//JsonConvert.DeserializeObject<List<empireObject>>(s);
			}
		}

		static Dictionary<Color, empireObject> empires = new Dictionary<Color, empireObject>();
		static Dictionary<Color, kingdomObject> kingdoms = new Dictionary<Color, kingdomObject>();
		static Dictionary<Color, duchyObject> duchies = new Dictionary<Color, duchyObject>();
		static Dictionary<Color, countyObject> counties = new Dictionary<Color, countyObject>();
		private static void loadTitleConfig()
		{
			readTitle(0, INPUT + "definition.csv");
			readTitle(1, INPUT + "definition.csv");
			readTitle(2, INPUT + "definition.csv");
			readTitle(3, INPUT + "definition.csv");

			//readTitle(0, CONFIG + "titles\\empires.csv");
			//readTitle(1, CONFIG + "titles\\kingdoms.csv");
			//readTitle(2, CONFIG + "titles\\duchies.csv");
			//readTitle(3, CONFIG + "titles\\counties.csv");
		}
		//Go over every pixel, read all titles at location. Error if multiple counties 
		private static void readTitle(int select, string file)
		{
			using (var reader = new StreamReader(PATH + file))
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
					switch (select) 
					{
						case 0:
							empires.Add(tempColor, new empireObject() { name = values[0], kingdoms = new List<kingdomObject>() } );
							break;
						case 1:
							kingdoms.Add(tempColor, new kingdomObject() { name = values[0], duchies = new List<duchyObject>() });
							break;
						case 2:
							duchies.Add(tempColor, new duchyObject() { name = values[0], counties = new List<countyObject>() });
							break;
						case 3:
							counties.Add(tempColor, new countyObject() { name = values[0], provinces = new List<province>() });
							break;
					}
				}
			}
		}

		private struct empireObject
		{
			public string name;
			public List<kingdomObject> kingdoms;
		}
		private struct kingdomObject
		{
			public string name;
			public List<duchyObject> duchies;
		}
		private struct duchyObject
		{
			public string name;
			public List<countyObject> counties;
		}
		private struct countyObject
		{
			public string name;
			public List<province> provinces;
		}

		static List<empireObject> empireList = new List<empireObject>();

		private static void processTitles()
		{
			Bitmap empireMap = new Bitmap(PATH + INPUT + "empires" + ".bmp");
			Bitmap kingdomMap = new Bitmap(PATH + INPUT + "kingdoms" + ".bmp");
			Bitmap duchyMap = new Bitmap(PATH + INPUT + "duchies" + ".bmp");
			Bitmap countyMap = new Bitmap(PATH + INPUT + "counties" + ".bmp");

			foreach (province p in provinces)
			{
				//Console.WriteLine("Province " + p.provID + " - color " + p.provColor.ToString() + " - coord (" + p.provCoord.X + "/" + p.provCoord.Y + ")");
				empireObject currentEmpire = empires[empireMap.GetPixel(p.provCoord.X, p.provCoord.Y)];
				kingdomObject currentKingdom = kingdoms[kingdomMap.GetPixel(p.provCoord.X, p.provCoord.Y)];
				duchyObject currentDuchy = duchies[duchyMap.GetPixel(p.provCoord.X, p.provCoord.Y)];
				countyObject currentCounty = counties[countyMap.GetPixel(p.provCoord.X, p.provCoord.Y)];
				int empireIndex;
				int kingdomIndex;
				int duchyIndex;
				int countyIndex;

				if (!empireList.Exists(i => i.name == currentEmpire.name)) //Empire doesn't exist
				{
					empireList.Add(currentEmpire);
				}
				empireIndex = empireList.FindIndex(i => i.name == currentEmpire.name);

				//Kingdom
				if (!empireList[empireIndex].kingdoms.Exists(i => i.name == currentKingdom.name))
				{
					empireList[empireIndex].kingdoms.Add(currentKingdom);
				}
				kingdomIndex = empireList[empireIndex].kingdoms.FindIndex(i => i.name == currentKingdom.name);

				//Duchy
				if (!empireList[empireIndex].kingdoms[kingdomIndex].duchies.Exists(i => i.name == currentDuchy.name))
				{
					empireList[empireIndex].kingdoms[kingdomIndex].duchies.Add(currentDuchy);
				}
				duchyIndex = empireList[empireIndex].kingdoms[kingdomIndex].duchies.FindIndex(i => i.name == currentDuchy.name);

				//County
				if (!empireList[empireIndex].kingdoms[kingdomIndex].duchies[duchyIndex].counties.Exists(i => i.name == currentCounty.name))
				{
					empireList[empireIndex].kingdoms[kingdomIndex].duchies[duchyIndex].counties.Add(currentCounty);
				}
				countyIndex = empireList[empireIndex].kingdoms[kingdomIndex].duchies[duchyIndex].counties.FindIndex(i => i.name == currentCounty.name);

				//Province
				empireList[empireIndex].kingdoms[kingdomIndex].duchies[duchyIndex].counties[countyIndex].provinces.Add(p);
			}
		}

		private static void generateProvinceTerrainFile()
		{
			using (FileStream fs = File.Create(PATH + OUTPUT + "00_province_terrain.txt"))
			{
				AddText(fs, "default=plains\n");
				foreach (province p in provinces) 
				{
					string line = p.provID.ToString() + "=" + p.preferedTerrain + "\n";
					AddText(fs, line);
					//Console.WriteLine("Province " + p.provID.ToString() + " with terrain " + p.preferedTerrain);
				}
			}
		}

		private static void AddText(FileStream fs, string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			fs.Write(info, 0, info.Length);
		}

		private static void loadTerrains()
		{
			using (var reader = new StreamReader(PATH + CONFIG + "terrain.csv"))
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
		private static void assignProvinces()
		{
			List<Queue<stackProvinceObject>> stackList = new List<Queue<stackProvinceObject>>();
			int currentGeneration = 0;

			foreach (province p in provinces) {
				stackList.Add(new Queue<stackProvinceObject>()); //New stack for each province
				stackList[stackList.Count - 1].Enqueue(new stackProvinceObject() { prov = new province() { provColor = p.provColor, provCoord = p.provCoord, provID = p.provID, preferedTerrain = p.preferedTerrain }, generation = 0 }); //Starting pixel for each province
			}

			//Console.WriteLine("Province Stacks " + stackList.Count);

			while (stackList.Count != 0) //Remove not-expanding province as loop goes on
			{
				for (int a = 0; a < stackList.Count; a++) //Every province not full
				{
					if (stackList[a].Count < 1) { //Empty stack
						//Console.WriteLine("debug empty stack 1");
						stackList.RemoveAt(a);
						a--; //Removed from list, go back
						continue;
					}
					while (stackList[a].Peek().generation == currentGeneration) //
					{
						stackProvinceObject currentObject = stackList[a].Dequeue();
						if (!( terrainDict.ContainsKey(provincesMap.GetPixel(currentObject.prov.provCoord.X, currentObject.prov.provCoord.Y))) && currentObject.generation != 0 ) 
						{
							//Only change terrain values
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
			provincesMap.Save(PATH + OUTPUT + "outputfile" + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
		}
		private static void assignTerrain()
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
						provinces.Add(new province { provColor = newColor, provCoord = new axis2d(x, y), provID = provinceIDCounter, preferedTerrain = terrain });
						provinceIDCounter++;
					}
				}
				//Console.WriteLine("x = " + x + "/" + width);
			}
		}

		private static void loadColors()
		{
			using (var reader = new StreamReader(PATH + INPUT + "definition.csv"))
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
		private struct province
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

		private struct stackProvinceObject {
			public province prov;
			public int generation;
		}

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
