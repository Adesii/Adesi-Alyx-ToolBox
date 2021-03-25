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

        private static SortedDictionary<string, Type> m_typeDictionary;
        public static SortedDictionary<string, Type> TypeDictionary
        {
            get
            {
                if (m_typeDictionary == null) m_typeDictionary = new();

                return m_typeDictionary;
            }
            set
            {
                m_typeDictionary = value;
            }
        }
    }
}
