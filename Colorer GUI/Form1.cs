using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Colorer_GUI
{
	public partial class Form1 : Form
	{
		static int provinceIDCounter = 1;
		string PATH = "";
		const string originalFile = "provinces";
		string endFile = "";
		string processingFile = "provincesprocessing";
		//const int TOTALPROVINCES = 9459;
		static Dictionary<int, Color> IDdictionary = new Dictionary<int, Color>(); //Used for getting colors from ID
		static HashSet<Color> colorHashSet = new HashSet<Color>(); // Used to efficiently iterate through used colors
		static Bitmap mapWarning;
		static Bitmap greyMap;
        bool warningFileMode = false;
        bool debugMode = false;
        bool existingMode = false;
        bool nonContiguousMode = false;
        int provinceDetectRadius = 0;
        int provinceSmallLimit = 0;
        Bitmap provincesMap;
        Thread T;

        public Form1()
		{
			InitializeComponent();
		}
        //common/province_terrain
		private void btnStart_Click(object sender, EventArgs e)
		{
            if (!File.Exists(PATH + originalFile + ".png")) {
                txtLog.AppendText("Aborting - file at \"" + PATH + originalFile + ".png" + "\" does not exist");
                return;
            }
            provincesMap = new Bitmap(PATH + originalFile + ".png");

            //Init form
            btnStart.Enabled = false;
            btnConfig.Enabled = false;
            btnAbort.Enabled = true;
            progressBar1.Maximum = provincesMap.Width; //

            //Threading
            T = new Thread(new ThreadStart(calculationFunction));
            T.Start();
		}

        //

        //Create dictionary of all ID's and colors
        //Create dictionary of all colors and ID's
        //Create hashset of used ID's 
        //Do inital passthrough to read used colors to ID's, then add to used ID's hashset
        //On normal pass, increment from ID 1. If the color dictionary ID is in ID hashset, increment and check again

        //Dictionary<int, Color> IDToColorDict;
        Dictionary<Color, int> ColorToIDDict;
        HashSet<int> usedIDs = new HashSet<int>();

        private bool checkPreExistingColors()
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
                if (x % 25 == 0) 
                {
                    this.Invoke(new MethodInvoker(delegate () { //This makes the GUI thread come here, instead of calling thread
                        progressBar1.PerformStep(); //obama
                    }));
                    if (debugMode)
                    {
                        txtLog.AppendText("\rx = " + x + "/" + provincesMap.Width + "\n");
                        txtLog.AppendText(Environment.NewLine);
                    }
                }
            }
                txtLog.AppendText("Unique provinces before processing = " + usedIDs.Count + "/" + foundColor.Count);
                txtLog.AppendText(Environment.NewLine);
            if (foundColor.Count == usedIDs.Count) {
                txtLog.AppendText("Aborting - No new provinces detected");
                txtLog.AppendText(Environment.NewLine);
                return false;
            }

            return true;
        }
        private void calculationFunction()
        {
            txtLog.AppendText("Loading colors");
            txtLog.AppendText(Environment.NewLine);
            loadColors();

            if (existingMode) 
            {
                txtLog.AppendText("Searching for pre-existing colors");
                txtLog.AppendText(Environment.NewLine);
                if (!checkPreExistingColors()) 
                {
                    releaseGUI();
                    return;
                }
                this.Invoke(new MethodInvoker(delegate () { //This makes the GUI thread come here, instead of calling thread
                    progressBar1.Value = 0; //obama
                }));
            }

            txtLog.AppendText("Processing image");
            txtLog.AppendText(Environment.NewLine);
            loadProvincesGray();
            //checkUniqueColors();

            txtLog.AppendText("\n\nFinished");
            txtLog.AppendText(Environment.NewLine);
            releaseGUI();
            return;
        }

        private bool loadProvincesGray()
        {
            //provincesMap = new Bitmap(PATH + originalFile + ".png");
            if (warningFileMode) {
                mapWarning = new Bitmap(PATH + originalFile + ".png");
            }
            greyMap = new Bitmap(PATH + originalFile + ".png");
            provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            int width = provincesMap.Width;
            int height = provincesMap.Height;

            while (usedIDs.Contains(provinceIDCounter)) //If ID was already used, go to next ID
            {
                provinceIDCounter++;
            }
            Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < height; y = y + 1)
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
                            if (warningFileMode && debugMode) {
                                mapWarning.Save(PATH + "warnings.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            if (debugMode) {
                                provincesMap.Save(PATH + processingFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                        }

                        provinceIDCounter++;
                        while (usedIDs.Contains(provinceIDCounter)) //If ID was already used, go to next ID
                        {
                            provinceIDCounter++;
                        }
                        newColor = IDdictionary[provinceIDCounter];
                    }
                    else
                    {

                    }
                }
                if (x % 25 == 0) {
                    this.Invoke(new MethodInvoker(delegate () { //This makes the GUI thread come here, instead of calling thread
                        progressBar1.PerformStep(); //obama
                    }));
                    if (debugMode)
                    {
                        txtLog.AppendText("\rx = " + x + "/" + provincesMap.Width);
                        txtLog.AppendText(Environment.NewLine);
                        //Console.WriteLine(" - unique colors = " + provinceIDCounter);
                    }
                }
            }

            //provincesMap.Save(PATH + endFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            provincesMap.Save(PATH + endFile + ".png", System.Drawing.Imaging.ImageFormat.Png);
            if (warningFileMode) {
                mapWarning.Save(PATH + "warnings.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
            
            txtLog.AppendText("Last used ID = " + provinceIDCounter);
            txtLog.AppendText(Environment.NewLine);

            return true;
        }

        private void fillWarning(Color oldColor, int danger, int x, int y) //https://stackoverflow.com/questions/452480/is-there-an-algorithm-to-determine-contiguous-colored-regions-in-a-grid
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

        private Bitmap fillShape(Color oldColor, Color newColor, int x, int y, Bitmap map) //https://stackoverflow.com/questions/452480/is-there-an-algorithm-to-determine-contiguous-colored-regions-in-a-grid
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

            if (shapeSize < provinceSmallLimit && nonContiguousMode) //Replace with constant
            {
                //Bitmap greyMap = new Bitmap(PATH + originalFile + ".png"); //If takes too long, make new solution
                if (debugMode) {
                    txtLog.AppendText("Small province detect at pixel (" + x + ", " + y + ")\n");
                    txtLog.AppendText(Environment.NewLine);
                }
                for (int a = x - provinceDetectRadius/2; a < x + provinceDetectRadius / 2; a = a + 1)
                {
                    for (int b = y - provinceDetectRadius / 2; b < y + provinceDetectRadius / 2; b = b + 1)
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
                            if (debugMode) {
                                txtLog.AppendText("right compatibile province detect at pixel (" + a + ", " + b + ")\n");
                                txtLog.AppendText(Environment.NewLine);
                            }
                            if (warningFileMode) {
                                fillWarning(tempColor, 1, a, b);
                                fillWarning(oldColor, 0, x, y);
                            }
                            return fillShape(oldColor, newColor, a, b, map);
                        }
                        //if (tempColor == greyMap.GetPixel(a, b) && !(newColor == greyMap.GetPixel(a, b)) && ) //Find province that is unique color and used to be specific grey color and is 
                        //{
                        //    Console.WriteLine("special compatibile province detect at pixel (" + a + ", " + b + ")");
                        //    return fillShape(tempColor, newColor, a, b, map);
                        //}
                        if (tempColor != newColor && colorHashSet.Contains(tempColor) && greyMap.GetPixel(a, b) == oldColor) //Left, find new unique color with old grey color. Replace current color with new unique.
                        {
                            if (debugMode) {
                                txtLog.AppendText("special compatibile province detect at pixel (" + a + ", " + b + ")\n");
                                txtLog.AppendText(Environment.NewLine);
                            }
                            if (warningFileMode) {
                                //fillWarning(oldColor, 2, a, b);
                            }

                            provinceIDCounter--; //Free last used color
                            return fillShape(newColor, tempColor, x, y, map);
                        }
                    }
                }
                if (warningFileMode) {
                    //fillWarning(oldColor, 0, x, y);
                }
                if (debugMode) {
                    txtLog.AppendText("No compat found\n");
                    txtLog.AppendText(Environment.NewLine);
                }
            }
            //Console.WriteLine("ShapeSize = " + shapeSize);
            return map;
        }

        private void loadColors()
        {
            ColorToIDDict = new Dictionary<Color, int>();
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
                    ColorToIDDict.Add(tempColor, tempID);
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

        private bool checkUniqueColors()
        {
            Bitmap map = new Bitmap(PATH + endFile + ".png");
            int uniqueProvinceCounter = 0;
            int defaultProvinceCounter = 0;
            HashSet<Color> foundColor = new HashSet<Color>();
            //provincesMap.Save(PATH + originalFile + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //Color newColor = IDdictionary[provinceIDCounter];

            for (int x = 0; x < map.Width; x = x + 1) //This allows us to assume we will always get the leftmost point of a province
            {
                for (int y = 0; y < map.Height; y = y + 3)
                {
                    Color tempColor = map.GetPixel(x, y);

                    if (foundColor.Add(tempColor))//Return false if color already present
                    {
                        if (colorHashSet.Contains(tempColor))
                        {
                            uniqueProvinceCounter++;
                        }
                        else
                        {
                            defaultProvinceCounter++;
                        }
                        //provincesMap = fillShape(tempColor, newColor, x, y, provincesMap);
                        //Console.WriteLine("Detected unique color");
                    }
                    else
                    {

                    }
                }
                txtLog.AppendText("\rx = " + x + "/" + map.Width + "\n");
            }
            txtLog.AppendText("\rUnique provinces = " + uniqueProvinceCounter + "\n");
            txtLog.AppendText("Old provinces = " + defaultProvinceCounter + "\n");
            txtLog.AppendText("Total provinces = " + (defaultProvinceCounter + uniqueProvinceCounter + "\n"));

            //provincesMap.Save(PATH + "ungreyed.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //Console.WriteLine("Provinces = " + provinceIDCounter);

            return true;
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

        private void releaseGUI()
        {
            btnStart.Enabled = true;
            btnConfig.Enabled = true;
            btnAbort.Enabled = false;
        }
		private void btnAbort_Click(object sender, EventArgs e)
		{
            txtLog.AppendText("Debug - aborted");
            txtLog.AppendText(Environment.NewLine);
            T.Abort();
            releaseGUI();
        }

		private void btnConfig_Click(object sender, EventArgs e)
		{
            config frm = new config(this);
            frm.ShowDialog();
        }

        public void applyConfig(string path, string outName, bool warningFile, bool debug, bool existing, bool nonContiguous, int provDetectRadius, int provSmallDef)
        {
            PATH = path;
            endFile = outName;
            warningFileMode = warningFile;
            debugMode = debug;
            existingMode = existing;
            nonContiguousMode = nonContiguous;
            provinceDetectRadius = provDetectRadius;
            provinceSmallLimit = provSmallDef;
            btnStart.Enabled = true;
        }
	}
}
