using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ValveKeyValue;
using SteamDatabase;
using System.IO;
using ValveResourceFormat;
using ValveResourceFormat.Serialization.KeyValues;
using ValveResourceFormat.ResourceTypes;
using KVObject = ValveResourceFormat.Serialization.KeyValues.KVObject;

namespace AAT.Soundevents
{
    class alyxJsonParser
    {
        public alyxJsonParser()
        {
        }

        public static List<Soundevent> allsoundeventsfromGame;
        public static List<string> AllEventTypes = new();

        public static Task GetAlyxSoundeventFromGame()
        {
            return Task.Run(() =>
            {


                List<Soundevent> soundevents = new List<Soundevent>();
                var pakr = new SteamDatabase.ValvePak.Package();
                pakr.Read(Properties.Settings.Default.InstallPath + "/game/hlvr/pak01_dir.vpk");

                Dictionary<string, SoundeventsPropertyDefinitions.EventTypeStruct> TypeDictionaryTemp = new();

                foreach (var item in pakr.Entries["vsndevts_c"])
                {


                    pakr.ReadEntry(item, out byte[] b, false);
                    var res = new Resource();
                    res.Read(new MemoryStream(b));
                    //Debug.Print(res.GetBlockByType(BlockType.DATA).ToString());
                    KV3File file = ((BinaryKV3)res.DataBlock).GetKV3File();
                    foreach (var obb in file.Root)
                    {
                        List<SoundeventProperty> propss = new();
                        Soundevent se = new(obb.Key);
                        foreach (var inobb in ((KVObject)obb.Value).Properties)
                        {
                            TypeDictionaryTemp[inobb.Key] = new SoundeventsPropertyDefinitions.EventTypeStruct() { Name = inobb.Key, Type = inobb.Value.Type, Realtype = inobb.Value.GetType(), KVValue = inobb.Value };

                            switch (inobb.Key)
                            {
                                case "base":
                                    propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.SoundeventPicker, inobb.Value.ToString()));
                                    break;
                                case "vsnd_files":
                                    propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.FilePicker, inobb.Value.ToString()));
                                    break;
                                case "type":
                                    if (!AllEventTypes.Contains(inobb.Value.Value.ToString()))
                                        AllEventTypes.Add(inobb.Value.Value.ToString());
                                    propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.TypePicker, inobb.Value));
                                    break;
                                default:
                                    propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.StringValue, inobb.Value));
                                    break;
                            }

                            //Debug.Print(inobb.Value.ToString());
                        }
                        se.Properties = propss;
                        soundevents.Add(se);
                        //Debug.Print("");
                    }

                    //var s = VKr.Deserialize();

                }
                SoundeventsPropertyDefinitions.TypeDictionary.Clear();
                foreach (var item in TypeDictionaryTemp)
                {
                    SoundeventsPropertyDefinitions.TypeDictionary.Add(item.Key, item.Value);
                }
                AllEventTypes.Sort();
                allsoundeventsfromGame = soundevents;
                Debug.WriteLine("finished loading all sounds");
            });
        }
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

}
