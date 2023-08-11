//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class ProvinceCK3
    {
        public String culture { get; set; }
        public String religion { get; set; } //Replace with class
        public String name { get; set; }
        public String duchy { get; set; }
        public String kingdom { get; set; }
        public String empire { get; set; }
        //public String holdingType;
        public Color color { get; set; }
        public int ID { get; set; } //Definition
        public int x { get; set; }
        public int y { get; set; }
        public String terrain { get; set; }
        public ProvinceCK3(){}
        public ProvinceCK3(Color color, int x, int y)
        {
            this.color = color;
            this.x = x;
            this.y = y;
        }
        /*
        //index;color1;color2;color3;name;x;y;culture;religion;duchy;kingdom;empire;
        public String ToCSV()
        {
            uint rgb = (uint) color.ToArgb();
            uint mask = 0xFF000000; // aaaa aaaa rrrr rrrr gggg gggg bbbb bbbb ; We ignore A
            uint r;
            uint g;
            uint b;

            mask = mask >> 8;
            r = rgb & mask;
            r = rgb >> 16;

            mask = mask >> 8;
            g = rgb & mask;
            g = rgb >> 8;

            mask = mask >> 8;
            b = rgb & mask;

            StringBuilder sb = new StringBuilder();
            sb.Append(ID + ";");
            sb.Append(r.ToString() + ";");
            sb.Append(g.ToString() + ";");
            sb.Append(b.ToString() + ";");
            sb.Append(name + ";");
            sb.Append(x + ";");
            sb.Append(y + ";");
            sb.Append(culture + ";");
            sb.Append(religion + ";");
            sb.Append(duchy + ";");
            sb.Append(kingdom + ";");
            sb.Append(empire + ";");

            return sb.ToString();
        }
    */
    }
}
