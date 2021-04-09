using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AAT.Soundevents;
using System.IO;
using HLACaptionCompiler;
using AAT.CloseCaptions;
using ValveResourceFormat.Serialization.KeyValues;
using AAT.AKV;

namespace AAT.Addons
{
    public class ResourceManifest : Interfaces.IWriteable
    {
        public static string Header = "<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->";
        public Addon ownAddon;
        public List<SoundeventFile> Files = new();

        public Dictionary<uint, Soundevent> AddonSoundeventDictionary = new();
        public Dictionary<string, LanguageCaption> AddonCaptionDictionary = new();

        public ResourceManifest(Addon owner)
        {
            ownAddon = owner;
            FillFileList();
        }

        public void FillFileList()
        {
            string soundeventsPath = Path.Combine(ownAddon.Path, "soundevents");
            if (!Directory.Exists(soundeventsPath)) Directory.CreateDirectory(soundeventsPath);
            foreach (var item in Directory.GetFiles(soundeventsPath, "*",SearchOption.AllDirectories))
            {
                //System.Diagnostics.Debug.WriteLine(Path.GetRelativePath(soundeventsPath, item));
                SoundeventFile f = new(Path.GetRelativePath(soundeventsPath, item)[..(Path.GetRelativePath(soundeventsPath, item).LastIndexOf('_'))], item);
                Files.Add(f);
                if (!ownAddon.AddonSpecificAddonFiles.ContainsKey(f.FileName)) ownAddon.AddonSpecificAddonFiles[f.FileName] = f;
                else ownAddon.AddonSpecificAddonFiles[f.FileName].Soundevents.Concat(f.Soundevents);
            }

        }
        public void SaveFile()
        {
            
            foreach (var EventFile in Files)
            {
                EventFile.SaveFileToEventsFolder(Path.Combine(ownAddon.Path,"soundevents"));
            }
            string manifestPath = Path.Combine(ownAddon.Path, "resourcemanifests");
            if (!Directory.Exists(manifestPath))
            {
                Directory.CreateDirectory(manifestPath);
            }
            File.WriteAllText(Path.Combine(manifestPath, ownAddon.AddonName + "_addon_resources.vrman"),ResourceManifestText(Files),Encoding.UTF8);

        }


        public string ResourceManifestText(List<SoundeventFile> data)
        {
            //TODO: dont hardcode it lul
            string TemplateBegin = 
                @"<!-- kv3 encoding:text:version{e21c7f3c-8a33-41c5-9977-a76d3a32aa0d} format:generic:version{7412167c-06e9-4698-aff2-e63eb59037e7} -->
{
    resourceManifest =
	[
		[";
            List<string> filePaths = new();
            foreach (var item in data)
            {
                filePaths.Add("\n\t\t\"soundevents\\"+item.FileName+ ".vsndevts\"");
            }
            string TemplateEnd = @"		],
	]
}";

            return TemplateBegin+string.Join(",",filePaths.ToArray())+ "\n"+TemplateEnd;
        }
    }
}
