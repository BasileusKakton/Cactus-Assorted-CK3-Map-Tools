using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK3MapCreator
{
    internal interface MapInterface
    {
        List<ProvinceCK3> getProvinces();
        Boolean loadProvincesExisting();
        Boolean loadProvinces();
    }
}
