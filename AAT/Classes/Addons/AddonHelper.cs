﻿using AAT.AKV;
using AAT.Soundevents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ValveResourceFormat.Serialization.KeyValues;

namespace AAT.Addons
{
    public static class AddonHelper
    {
        public const int NUMBEROF0 = 5;
        static string[] GetAddons(string AlyxInstallFolder)
        {
            string path = System.IO.Path.Combine(AlyxInstallFolder, "content", "hlvr_addons");
            List<string> retVal = new List<string>();

            foreach (var dir in new System.IO.DirectoryInfo(path).GetDirectories())
            {
                retVal.Add(dir.Name);
            }
            return retVal.ToArray();
        }
        public static string GetAddonPath(string AlyxInstallFolder, string addonName)
        {
            return System.IO.Path.Combine(AlyxInstallFolder, "content", "hlvr_addons", addonName);
        }
        const string SoundEventHeader = "<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->";

        public static IEnumerable<Soundevent> Deserialize(System.IO.Stream stream)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int bytesRead = 0;
                byte[] buffer = new byte[32768];
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                return Deserialize(System.Text.ASCIIEncoding.ASCII.GetString(ms.ToArray()));
            }
        }
        public static IEnumerable<Soundevent> DeserializeFile(string file)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(file))
            {
                return Deserialize(sr.ReadToEnd());
            }
        }
        public static IEnumerable<Soundevent> Deserialize(string data)  //Change to data instead (no file opening).
        {
            List<Soundevent> events = new List<Soundevent>();

            List<string> Lines = new List<string>();

            bool propertyOpen = false;

            string[] AllLines = data.Replace("\n", string.Empty).Split('\r');

            for (int i = 2; i < AllLines.Length; i++)  //This expects line 1 to be the header (SoundEventHeader defined above), while line 2 is the opening brace ("{").  If it could be different additional logic is needed.
            {
                string line = AllLines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!propertyOpen && line.StartsWith("}"))
                {
                    break;
                }
                else
                {
                    int j = -1;
                    //System.Diagnostics.Debug.WriteLine(line[++j]);
                    while (line[++j] == '\t' || line[j] == ' ') { }

                    if (line[j] == '{')
                    {
                        propertyOpen = true;
                        Lines.Add(line.Substring(j).Trim());
                    }
                    else if (line[j] == '}')
                    {
                        propertyOpen = false;
                        for (int i1 = 0; i1 < Lines.Count; i1++)
                        {
                            string item = Lines[i1];
                            if (item.Contains("=[") || item.Contains("= ["))
                            {
                                Lines[i1]=Lines[i1].Replace('[',' ');
                                Lines.Insert(i1+1, "[");
                                i1++;
                            }
                        }
                        var property = CreateSoundevent(Lines.ToArray());
                        events.Add(property);
                        Lines.Clear();
                    }
                    else
                    {
                        Lines.Add(line.Substring(j).Trim());
                    }
                }
            }
            StringBuilder builder = new StringBuilder();


            return events.ToArray();
        }
        private static Soundevent CreateSoundevent(string[] lines)
        {
            List<string> values = new List<string>();
            bool propertyOpen = false;
            string comment;
            bool valueOpen = false;
            string valueName = null;
            string valueValue;

            string eventName = null;
            Soundevent currentEvent = null;
            List<SoundeventProperty> properties = new();
            foreach (var rawline in lines)
            {
                string line;
                int i = rawline.IndexOf("//");
                if (i > -1)
                {
                    if (i == 0)
                    {
                        //skip comment line.
                        comment = rawline.Substring(2);
                        continue;
                    }
                    else
                    {
                        //strip out comments;
                        line = rawline.Substring(0, i);
                        comment = rawline[i..];
                    }
                }
                else
                {
                    line = rawline;
                }

                if (!propertyOpen)
                {
                    //looking for event name.
                    int j = line.IndexOf('=');
                    if (j > -1)
                    {
                        eventName = line.Substring(0, j).Replace("\"", string.Empty).Trim();
                    }

                }
                //System.Diagnostics.Debug.WriteLine(line);
                if (line.StartsWith("{"))
                {
                    if (string.IsNullOrEmpty(eventName))
                    {
                        //continue;
                        throw new InvalidFileFormatException("Event name for a list of sound event properties was not found.");
                    }
                    propertyOpen = true;
                }
                else if (line.EndsWith("}"))
                {
                    propertyOpen = false;
                    break;

                }
                else if (propertyOpen)
                {
                    if (line.StartsWith('['))
                    {
                        valueOpen = true;
                    }
                    else if (line.StartsWith(']'))
                    {
                        if (string.IsNullOrEmpty(valueName))
                        {
                            throw new InvalidFileFormatException("Invalid file format.");
                        }

                        //TODO: The following line will need change depending on how arrays are handled.

                        List<AKValue> ak = new();
                        for (int i1 = 0; i1 < values.Count; i1++)
                        {
                            System.Diagnostics.Debug.WriteLine(values[i1]);
                            ak.Add(new AKValue(KVType.STRING,values[i1]));
                        }
                        System.Diagnostics.Debug.WriteLine(ak.Count);
                        SoundeventProperty bproperty = new(valueName, EventDisplays.ArrayValue, ak);
                        properties.Add(bproperty);
                        valueOpen = false;
                        values.Clear();
                    }
                    else if (valueOpen)
                    {
                        //Is an array--load list of values.
                        string l = line;
                        if (l.EndsWith(','))
                        {
                            l = l.Substring(0, l.Length - 1);
                        }
                        values.Add(l.Replace("\"", string.Empty));
                    }
                    else
                    {
                        //either name = value or name =
                        //                            [
                        //                            ]
                        int k = line.IndexOf('=');
                        if (k > -1)
                        {
                            valueName = line.Substring(0, k).Trim();

                            while (++k < line.Length && line[k] < ' ') { }
                            if (k < line.Length)
                            {
                                valueValue = line.Substring(k + 1).Trim();
                                if (!string.IsNullOrEmpty(valueValue))
                                {
                                    if (valueValue.StartsWith("\"") && valueValue.EndsWith("\""))
                                    {
                                        valueValue = valueValue[1..^1].Replace("\n",string.Empty);
                                    }
                                    var aproperty = new SoundeventProperty(valueName, valueValue);
                                    properties.Add(aproperty);
                                }
                            }
                        }
                        else
                        {
                           throw new InvalidFileFormatException("File format is invalid. Current Line: "+ line);
                        }
                    }
                }
            }
            if (eventName == null)
            {
                throw new InvalidFileFormatException("Event name for a list of sound event properties was not found.");
            }
            currentEvent = new Soundevent(eventName);
            foreach (var prop in properties)
            {
                currentEvent.AddProperty(prop);
            }

            return currentEvent;
        }
        public static string Serialize(IEnumerable<Soundevent> soundevents)
        {
            StringBuilder sb = new();
            sb.Append(SoundEventHeader +"\r\n");
            sb.Append("{" + "\r\n");
            foreach (var soundevent in soundevents)
            {
                sb.Append("\t"+soundevent.EventName+ " = " + "\r\n");
                sb.Append("\t{" + "\r\n");
                foreach (var property in soundevent.Properties)
                {
                    if (property.Value == null || string.IsNullOrWhiteSpace(property.Value.ToString())) continue;
                    sb.Append("\t\t");
                    sb.Append(property.TypeName);
                    sb.Append(" = ");

                    //TODO: The following lines will need change depending on how arrays are handled.
                    if (property.DisAs == EventDisplays.ArrayValue || property.DisAs == EventDisplays.FilePicker)
                    {
                        var w = new ValveResourceFormat.IndentedTextWriter();
                        var kvO = property.Value as List<AKValue>;
                        w.NewLine = "\r\n\t\t";
                        AKV.AKValue.SerializeArray(w, kvO);
                        sb.Append(w.ToString()+"\r\n");
                    }
                    else
                    {
                        if (property.DisAs == EventDisplays.StringValue)
                        {
                            sb.Append("\"");
                        }
                        switch (property.DisAs)
                        {
                            case EventDisplays.FloatValue:
                                if (property.Value != null)
                                {
                                    sb.Append(float.Parse(property.Value.ToString()).ToString("0.00##").Replace(",", "."));
                                }
                                else
                                {
                                    sb.Append(property.Value);
                                }
                                break;
                            case EventDisplays.FilePicker:
                                var w = new ValveResourceFormat.IndentedTextWriter();
                                w.NewLine = "\n\t\t";
                                w.Indent++;
                                sb.Append("\n\t\t[\n\t\t\t");
                                if (property.Value as List<AKV.AKValue> != null)
                                    foreach (var item in property.Value as List<AKV.AKValue>)
                                    {
                                        w.WriteLine(item.Value.ToString());
                                    }
                                else System.Diagnostics.Debug.WriteLine(property.Value);
                                sb.Append(w.ToString());
                                sb.Append("]");
                                break;
                            case EventDisplays.SoundeventPicker:
                            case EventDisplays.ArrayValue:
                            case EventDisplays.EventTypePicker:
                            case EventDisplays.TypePicker:
                                sb.Append('"'+property.Value.ToString().Trim('\"')+ '"');
                                break;
                            case EventDisplays.StringValue:
                                sb.Append(property.Value.ToString().Trim('\"'));
                                break;
                            default:
                                sb.Append(property.Value.ToString().Trim('\"'));
                                break;
                        }
                        System.Diagnostics.Debug.WriteLine("property: "+property.Value.ToString()+"   "+property.DisAs + "   " + property.TypeName);
                        if (property.DisAs == EventDisplays.StringValue)
                        {
                            sb.Append("\"" + "\r\n");
                        }
                        else
                        {
                            sb.Append("\r\n");
                        }
                    }
                }

                sb.Append("\t}" + "\r\n");
            }
            sb.Append("}");
            return sb.ToString();
        }

    }
}
