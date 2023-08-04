using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Creator mapCreator = new Creator(false);
            //Step 1: Ask the User for files types
            //We will assume they are all at predefined locations
            mapCreator.setBasePath("C:\\Users\\Damian and Kerstin\\Desktop\\CKModding\\mapMaker");

            //Step 2: Generate provinces
            mapCreator.generateProvinces();
        }
    }
}
