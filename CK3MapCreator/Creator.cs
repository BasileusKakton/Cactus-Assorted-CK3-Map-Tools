using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class Creator
    {
        public GameState state;
        public Creator(Boolean AskFileLocation) 
        {
            if (AskFileLocation)
            {
                Console.WriteLine("Custom File locations are not implemented yet");
            }
            state = GameState.GetGameState();
        }

        public void createCulture()
        {
            Console.WriteLine("Not done");
        }

        public void createAll()
        {
            Console.WriteLine("Not done");
        }

        public void generateProvincesFromScratch()
        {
            MapFromScratch generator = new MapFromScratch();
            generator.doThings();
        }

        public void setBasePath(String path)
        {
            FileLoader fileLoader = FileLoader.getFileLoader();
            fileLoader.setBasePath(path);
        }

        public void useBaseInternalPath()
        {
            FileLoader fileLoader = FileLoader.getFileLoader();
            fileLoader.useDefaultPath();
        }
    }
}
