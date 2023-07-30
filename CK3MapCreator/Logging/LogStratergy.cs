using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator.Logging
{
    internal interface LogStratergy
    {
        void execute(String output);
        void execute(String output, String group);
    }
}
