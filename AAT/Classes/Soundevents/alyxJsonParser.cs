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
        string jsonString;
        public alyxJsonParser()
        {
            jsonString = Properties.Resources.BaseSoundevents;
        }

        public List<Soundevent> GetAlyxSoundeventFromGame()
        {
            List<Soundevent> soundevents = new List<Soundevent>();
            var pakr = new SteamDatabase.ValvePak.Package();
            pakr.Read(Properties.Settings.Default.InstallPath + "/game/hlvr/pak01_dir.vpk");
            foreach (var item in pakr.Entries["vsndevts_c"])
            {
                Soundevent se = null;
                List<SoundeventProperty> propss = new List<SoundeventProperty>();

                pakr.ReadEntry(item, out byte[] b, false);
                var res = new Resource();
                res.Read(new MemoryStream(b));
                //Debug.Print(res.GetBlockByType(BlockType.DATA).ToString());
                KV3File file = ((BinaryKV3)res.DataBlock).GetKV3File();
                foreach (var obb in file.Root)
                {
                    se = new Soundevent(obb.Key);
                    foreach (var inobb in ((KVObject)obb.Value))
                    {
                        SoundeventsPropertyDefinitions.typeDictionary.TryAdd(inobb.Key,inobb.Value.GetType());
                        switch (inobb.Key)
                        {
                            case "base":
                                propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.SoundeventPicker, inobb.Value.ToString()));
                                break;
                            default:
                                propss.Add(new SoundeventProperty(inobb.Key, EventDisplays.StringValue, inobb.Value.ToString()));
                                break;
                        }

                        //Debug.Print(inobb.Value.ToString());
                    }
                    soundevents.Add(se);
                    //Debug.Print("");
                }

                //var s = VKr.Deserialize();
            }
            return soundevents;
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
