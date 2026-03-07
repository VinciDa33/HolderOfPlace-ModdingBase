using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ModUtils;
using TMPro;
using UnityEngine;

namespace ModdingCore
{
    public class BootstrapMain
    {
        public static string version = "v0.1.0";
        public static string StreamingAssetPath;
        public static string modPath;
        public static string corePath;
        public static Harmony mainHarmony;

        public static bool modsLoaded = false;
        public static Dictionary<string, HopMod> modDictionary = new Dictionary<string, HopMod>();
        public static List<string> activeMods = new List<string>();

        public static int inputBlock = 0;


        //Config Variables
        public static bool consoleOnStartup = true;

        public static Sprite GetSprite(string path)
        {
            Sprite s = GetSprite(GetTex(path), new Vector2(0.5f, 0.5f));
            if (s != null)
            {
                s.name = path;
            }
            return s;
        }

        public static Texture2D GetTex(string path)
        {
            string fullPath = path;
            if (!File.Exists(fullPath))
            {
                System.Console.WriteLine("[Mod] Could not find file at path: " + path);
                return null;
            }
            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D t = new Texture2D(2, 2);
            Traverse.Create(typeof(ImageConversion)).Method("LoadImage", new Type[] {typeof(Texture2D), typeof(byte[]), typeof(bool)}).GetValue(t, bytes, false);
            return t;
        }
        
        public static Sprite GetSprite(Texture2D tex, Vector2 pivot)
        {
            if (tex == null)
            {
                return null;
            }
               
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot);
        }
        //AnimBase/NewAliveBase
        //Base
        //Background
        //Ideogram

        //Skills

        //Status

        public static bool IsInputAllowed()
        {
            return (inputBlock == 0);
        }

        public static void Start()
        {
            if (mainHarmony == null)
            {
                //CreateModSelector();
                StreamingAssetPath = Application.streamingAssetsPath;
                modPath = StreamingAssetPath + "/Mods";
                corePath = StreamingAssetPath + "/Core";
                if (consoleOnStartup)
                {
                    Console.LoadConsole();
                }
                StartHarmony();
                LibraryExt.StartVirtualPools();
                ModMenu.CreateModMenu();
                CommandLine.CreateCommandLineHolder();

                //LoadMods();
            }
        }

        public static void OpenUnityExplorer()
        {
            //To-do: Update DLLEditor so I can remove this method.
        }

        public static void StartHarmony()
        {
            if (mainHarmony != null)
            {
                return;
            }
            mainHarmony = new Harmony("mhcdc9.hop");
            System.Console.WriteLine("[MOD] Harmony Online!");
            mainHarmony.PatchAll(typeof(BootstrapMain).Assembly);
        }

        public static void LoadMods()
        {
            //1. Load all the assemblies from every possible place
            //2. Find the HopMod of each group and create an instance of that
            //3. Run OnEnabled for already enabled mods
            if (modsLoaded)
            {
                return;
            }
            Directory.CreateDirectory(modPath);
            Directory.CreateDirectory(corePath);
            Dictionary<string, List<Assembly>> assemblyDictionary = new Dictionary<string, List<Assembly>>();
            //1
            foreach (string path in Directory.GetDirectories(modPath))
            {
                System.Console.WriteLine(path);
                List<Assembly> assemblies = new List<Assembly>();
                Directory.GetFiles(path).Where(p => p.Substring(p.Length - 4) == ".dll").Do(p => assemblies.Add(Assembly.LoadFrom(p)));
                if (assemblies.Count > 0)
                {
                    assemblyDictionary[path] = assemblies;
                }
            }
            //2
            foreach(string path in assemblyDictionary.Keys)
            {
                foreach(Assembly assembly in assemblyDictionary[path])
                {
                    Type type = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(HopMod)));
                    if (type != null)
                    {
                        HopMod mod = (HopMod)type.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] {path});
                        modDictionary[mod.Guid] = mod;
                        break;
                    }
                }
            }
            //3
            string lastActiveModsPath = corePath + "/" + "lastActiveMods.txt";
            if (!File.Exists(lastActiveModsPath))
            {
                File.WriteAllText(lastActiveModsPath,"");
            }
            foreach(string guid in File.ReadLines(lastActiveModsPath))
            {
                if (guid != null && modDictionary.ContainsKey(guid))
                {
                    LoadMod(modDictionary[guid]);
                }
            }
            SaveLastActive();
            modsLoaded = true;
        }

        public static List<HopMod> loadStack = new List<HopMod>();
        public static void LoadMod(HopMod mod)
        {
            if (mod.enabled)
            {
                return;
            }
            if (loadStack.Contains(mod))
            {
                throw new Exception("Infinite Loop Detected! Involves the mod: " + mod.Title);
            }
            loadStack.Add(mod);
            foreach(string dependency in mod.Depends)
            {
                if (!modDictionary.ContainsKey(dependency))
                {
                    throw new Exception("Could not find dependency with guid " + dependency);
                }
                LoadMod(modDictionary[dependency]);
            }
            loadStack.Remove(mod);
            mod.OnEnable();
            activeMods.Add(mod.Guid);
            System.Console.WriteLine("[Mod] Loaded [" + mod.Title + "]");
            if (modsLoaded)
            {
                SaveLastActive();
            }
        }

        public static void UnloadMod(HopMod mod)
        {
            if (!mod.enabled)
            {
                return;
            }
            mod.OnDisable();
            activeMods.Remove(mod.Guid);
            System.Console.WriteLine("[Mod] Unloaded [" + mod.Title + "]");
            SaveLastActive();
        }

        public static void SaveLastActive()
        {
            string lastActiveModsPath = corePath + "/" + "lastActiveMods.txt";
            string contents = "";
            foreach(string s in activeMods)
            {
                contents += s + "\n";
            }
            File.WriteAllText(lastActiveModsPath, contents);
            System.Console.WriteLine("[Mod] Last Active Mods Saved!");
        }
    }
}
