using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal class CK3Map : MapInterface
    {
        List<ProvinceCK3> provinces = new List<ProvinceCK3>();
        List<ProvinceCK3> getProvinces() 
        {
            Console.WriteLine("No");
            return provinces;
        }
        public Boolean loadProvincesExisting() 
        {
            return false;
        }
        public Boolean loadProvinces()
        {
            return false;
        }
    }
}
