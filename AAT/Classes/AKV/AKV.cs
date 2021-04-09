using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValveResourceFormat;
using ValveResourceFormat.Serialization.KeyValues;

namespace AAT.AKV
{
    public class AKValue
    {
        public KVType Type { get;set; }
        public object Value { get;set; }

        public AKValue(KVType type, object value)
        {
            Type = type;
            Value = value;
        }

        //Print a value in the correct representation
        public void PrintValue(IndentedTextWriter writer)
        {
            switch (Type)
            {
                case KVType.OBJECT:
                case KVType.ARRAY:
                    if (Value is List<AKValue> k)
                        SerializeArray(writer, k);
                    break;
                case KVType.STRING:
                    writer.Write("\""+((string)Value)?.Trim('\n').Trim()+ "\"");
                    break;
                case KVType.STRING_MULTI:
                    writer.Write("\"\"\"\n");
                    writer.Write((string)Value);
                    writer.Write("\n\"\"\"");
                    break;
                case KVType.BOOLEAN:
                    writer.Write((bool)Value ? "true" : "false");
                    break;
                case KVType.DOUBLE:
                    writer.Write(((float)Value).ToString("#0.000000", CultureInfo.InvariantCulture));
                    break;
                case KVType.INT64:
                    writer.Write(Convert.ToInt64(Value));
                    break;
                case KVType.UINT64:
                    writer.Write(Convert.ToUInt64(Value));
                    break;
                case KVType.NULL:
                    writer.Write("null");
                    break;
                case KVType.BINARY_BLOB:
                    var byteArray = (byte[])Value;
                    var count = 0;

                    writer.WriteLine();
                    writer.WriteLine("#[");
                    writer.Indent++;

                    foreach (var oneByte in byteArray)
                    {
                        writer.Write(oneByte.ToString("X2"));

                        if (++count % 32 == 0)
                        {
                            writer.WriteLine();
                        }
                        else
                        {
                            writer.Write(" ");
                        }
                    }

                    writer.Indent--;

                    if (count % 32 != 0)
                    {
                        writer.WriteLine();
                    }

                    writer.Write("]");
                    break;
                default:
                    throw new InvalidOperationException($"Trying to print unknown type '{Type}'");
            }
        }

        private string EscapeUnescaped(string input, char toEscape)
        {
            if (input.Length == 0)
            {
                return input;
            }

            int index = 1;
            while (true)
            {
                index = input.IndexOf(toEscape, index);

                //Break out of the loop if no more occurrences were found
                if (index == -1)
                {
                    break;
                }

                if (input.ElementAt(index - 1) != '\\')
                {
                    input = input.Insert(index, "\\");
                }

                //Don't read this one again
                index++;
            }

            return input;
        }
        public static void SerializeArray(IndentedTextWriter writer, List<AKValue> k)
        {
            //Need to preserve the order
            writer.WriteLine();
            writer.WriteLine("[");
            writer.Indent++;
            foreach (var item in k)
            {
                item.PrintValue(writer);
                writer.WriteLine(",");
            }
            writer.Indent--;
            writer.Write("]");
        }
    }
}
