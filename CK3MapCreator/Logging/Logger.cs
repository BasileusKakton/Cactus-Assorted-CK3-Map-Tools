using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator.Logging
{
    internal class Logger //This is the stratergy context
    {
        //Output as one file with different groups
        //Output groups as different files
        //Output to console
        private static Logger logger;

        public static Logger getLogger()
        {
            if (logger == null)
            {
                logger = new Logger();
            }
            return logger;
        }

        private LogStratergy stratergy = new LogConsole(); //Stratergy Context

        public void setStrategy(string userStrat)
        {
            if (userStrat.Equals("file"))
            {
                logger.stratergy = new LogFile();
            }
            /*else if (userStrat.Equals("ManyFiles"))
            {
                logger.stratergy = new LogConsole();
            }*/
            else if (userStrat.Equals("console"))
            {
                logger.stratergy = new LogConsole();
            }
            else
            {
                //Default to Console
                logger.stratergy = new LogConsole();
            }
        }

        public void Log(String output)
        {
            logger.stratergy.execute(output);
        }

        public void Log(String output, String group)
        {
            logger.stratergy.execute(output, group);
        }
    }
}
