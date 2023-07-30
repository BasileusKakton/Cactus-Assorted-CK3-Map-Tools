using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class GameState
    {
        //Make this a singleton with provinces, maps, 
        static GameState instance;
        private List<ProvinceCK3> provinces;
        private String cultures;

        public static GameState GetGameState()
        {
            if (instance == null)
            {
                instance = new GameState();
            }
            return instance;
        }


    }
}
