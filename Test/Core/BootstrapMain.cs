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
        public static string StreamingAssetPath;
        public static string modPath;
        public static Harmony mainHarmony;
        public static List<HopMod> mods = new List<HopMod>();


        public static bool explorerLoaded = false;


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
        public static void Start()
        {
            if (mainHarmony == null)
            {
                //CreateModSelector();
                StreamingAssetPath = Application.streamingAssetsPath;
                modPath = StreamingAssetPath + "/Mods";
                StartHarmony();
                ModMenu.CreateModMenu();
            }
            OpenUnityExplorer();
        }

        public static void OpenUnityExplorer()
        {
            if (explorerLoaded)
            {
                return;
            }
            UnityExplorer.ExplorerStandalone.CreateInstance();
            explorerLoaded = true;
        }

        public static void StartHarmony()
        {
            if (mainHarmony != null)
            {
                return;
            }
            Console.LoadConsole();
            mainHarmony = new Harmony("mhcdc9.hop");
            System.Console.WriteLine("[MOD] Harmony Online!");
            mainHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void LoadMods()
        {
            if (!Directory.Exists(modPath))
            {
                return;
            }
            foreach(string path in Directory.GetDirectories(modPath))
            {
                System.Console.WriteLine(path);
                List<Assembly> assemblies = new List<Assembly>();
                Directory.GetFiles(path).Where(p => p.Substring(p.Length-4) == ".dll").Do(p => assemblies.Add(Assembly.LoadFrom(p)));
                
                foreach(Assembly assembly in assemblies)
                {
                    System.Console.WriteLine("Assembly: " + path);
                    Type type = assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(typeof(HopMod)));
                    if (type != null)
                    {
                        HopMod mod = (HopMod)type.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] {path});
                        mods.Add(mod);
                        break;
                    }
                }
            }
        }

        public static void CreateModSelector()
        {

        }
    }
}
