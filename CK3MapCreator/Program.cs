using CK3MapCreator.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class Program
    {
        static Creator mapCreator = new Creator(false);

        static void Main(string[] args)
        {

            mapCreator = new Creator(false);
            //Step 1: Ask the User for files types
            //We will assume they are all at predefined locations
            mapCreator.setBasePath("C:\\Users\\Damian and Kerstin\\Desktop\\MapCreator\\Cactus-Assorted-CK3-Map-Tools\\Test\\");
            mapCreator.useBaseInternalPath();

            //Step 2: Generate provinces
            //mapCreator.generateProvincesFromScratch();

            while (processInput()); 
        }
        private static bool processInput()
        {
            Console.Write(">>>");
            String input = Console.ReadLine();
            if (input == null)
            {
                return true; //Continue for empty line
            }
            String[] args = input.Split(' ');

            if (args.Length == 1)
            {
                if (args[0].Equals("exit") || args[0].Equals("quit"))
                {
                    Environment.Exit(0);
                    return false;
                }
                else if (args[0].Equals("help"))
                {
                    Console.WriteLine("\n(exit or quit)");
                    Console.WriteLine("basepath path                -Change the basepath to the desired path");
                    Console.WriteLine("generate                     -Generates provinces from scratch - needs a few files setup first");
                    Console.WriteLine("logmode (file or console)    -Makes the log out to console or a log file at the root of the basepath");
                    Console.WriteLine("export                       -Exports current game state for future restore");
                    Console.WriteLine("import                       -Imports game state from file");
                    Console.WriteLine("provinces                    -How many provinces are loaded");
                    //Console.WriteLine("generate - generates provinces from scratch - needs a few files setup first");
                }
                else if (args[0].Equals("generate"))
                {
                    mapCreator.generateProvincesFromScratch();
                }
                else if (args[0].Equals("export"))
                {
                    mapCreator.export();
                }
                else if (args[0].Equals("import"))
                {
                    mapCreator.import();
                }
                else if (args[0].Equals("provinces"))
                {
                    mapCreator.provinces();
                }
                else
                {
                    Console.WriteLine("There are no commands of this type with no args");
                }
            } 
            else if (args.Length == 2)
            {
                if (args[0].Equals("basepath"))
                {
                    mapCreator.setBasePath(args[1]);
                }
                else if (args[0].Equals("logmode"))
                {
                    Logger.getLogger().setStrategy(args[1]); //TODO - Check for valid input, fix files appending at start
                }
                else if (args[0].Equals("basepath"))
                {
                    mapCreator.setBasePath(args[1]);
                }
                else
                {
                    Console.WriteLine("There are no commands of this type with 1 arg");
                }
            }
            else
            {
                Console.WriteLine("There is no command with this many args");
            }

            
            return true; //Continue for bad input
        }
    }
}
