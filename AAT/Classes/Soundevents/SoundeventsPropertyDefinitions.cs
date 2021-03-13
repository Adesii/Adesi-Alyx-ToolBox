using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAT.Soundevents
{
    class SoundeventsPropertyDefinitions
    {
        public static SoundeventsPropertyDefinitions Instance = new SoundeventsPropertyDefinitions();
        public static SortedDictionary<string, Type> typeDictionary = new SortedDictionary<string, Type>();
    }
}
