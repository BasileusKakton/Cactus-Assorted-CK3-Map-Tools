using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator.Logging
{
    internal class LogConsole : LogStratergy
    {
        public void execute(String output)
        {
            Console.WriteLine(output);
        }

        public void execute(String group, String output)
        {
            Console.WriteLine(group + " | " + output);
        }
    }
}
