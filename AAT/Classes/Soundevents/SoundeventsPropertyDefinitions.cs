using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat.Serialization.KeyValues;

namespace AAT.Soundevents
{
    public static class SoundeventsPropertyDefinitions
    {

        private static SortedDictionary<string, EventTypeStruct> m_typeDictionary;
        public static SortedDictionary<string, EventTypeStruct> TypeDictionary
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
        public class EventTypeStruct
        {
            private string m_name;
            private string m_group;
            private KVType m_type;
            private Type m_realtype;
            private int m_arrayDepth;
            private object m_KVValue;

            public KVType Type
            {
                get
                {
                    return m_type;
                }
                set
                {
                    m_type = value;
                }
            }
            public string Name
            {
                get
                {
                    return m_name;
                }
                set
                {
                    m_name = value;
                    Group = value;
                }
            }
            public string Group
            {
                get
                {
                    return m_group;
                }
                set
                {

                    if (!value.Contains("_", StringComparison.CurrentCulture)) m_group = value;
                    else
                        m_group = value.Substring(0, value.IndexOf("_"));
                }
            }

            public int ArrayDepth { get => m_arrayDepth; set => m_arrayDepth = value; }
            public Type Realtype { get => m_realtype; set => m_realtype = value; }
            public object KVValue { get => m_KVValue; set => m_KVValue = value; }
        }
    }
}
