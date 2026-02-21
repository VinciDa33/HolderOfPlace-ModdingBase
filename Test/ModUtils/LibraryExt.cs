using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ModUtils
{
    [HarmonyPatch()]
    public static class LibraryExt
    {
        internal static GameObject modAssets;
        internal static List<string> leaderPool = new List<string>();

        public static List<GameObject> animPrefabs = new List<GameObject>();

        public static void DirectRecruit(string key)
        {
            RecruitPanel.Main.DirectRecruit(key, true);
        }

        public static void MapCard(string key, bool hideAutoAttack = true)
        {
            Card card = Library.Main.GetCard(key)?.GetComponent<Card>();
            if (card == null)
            {
                System.Console.WriteLine("[ModUtils] Could not find card");
            }
            System.Console.WriteLine("============");
            System.Console.WriteLine(card.GetName());

            card.KeyMark.GKB();

            Targeting t = card.IniTargeting?.GetComponent<Targeting>();
            if (t != null)
            {
                MapTargetting(t, "|");
            }
            card.IniSkills.Select(g => g.GetComponent<Mark_Skill>()).Where(sk => sk != null && (!hideAutoAttack || sk.name != "AutoAttack")).Do(sk => MapSkill(sk, "-"));
            card.IniStatus.Select(g => g.GetComponent<Mark_Status>()).Where(st => st != null).Do(st => MapStatus(st, "-"));

        }

        public static void MapTargetting(Targeting t, string prefix)
        {
            System.Console.Write(prefix + "[T]" + t.GetType().Name + ": ");
            foreach (string s in t.GKB().Keys)
            {
                System.Console.Write(s + ", ");
            }
            System.Console.WriteLine();
        }

        public static void MapConditions(Condition c, string prefix)
        {
            System.Console.Write(prefix + "[C]" + c.GetType().Name + ": ");
            foreach (string s in c.GKB().Keys)
            {
                System.Console.Write(s + ", ");
            }
            System.Console.WriteLine();
        }

        public static void MapSkill(Mark_Skill skill, string prefix)
        {
            System.Console.Write(prefix + "[SK]" + skill.GetType().Name + "(" + skill.name + "): ");
            foreach(string s in skill.GKB().Keys)
            {
                System.Console.Write(s + ", ");
            }
            System.Console.WriteLine();

            skill.Targetings.Select(t=>t.GetComponent<Targeting>()).Where(t=>t!=null).Do(t => MapTargetting(t,prefix+"|"));
            skill.Conditions.Select(c => c.GetComponent<Condition>()).Where(c => c != null).Do(c => MapConditions(c, prefix + "|"));

            IEnumerable<Signal> signals = skill.MainSignals.Select(m => m.GetComponent<Signal>()).Where(s => s != null);
            foreach (Signal sig in signals)
            {
                MapSignal(sig.GetComponent<Signal>(), prefix + "-");
            }
        }

        public static void MapStatus(Mark_Status status, string prefix)
        {
            System.Console.Write(prefix + "[ST]" + status.GetType().Name + "(" + status.name + "): ");
            foreach (string s in status.GKB().Keys)
            {
                System.Console.Write(s + ", ");
            }
            System.Console.WriteLine();
            if (status is Mark_Trigger trigger)
            {
                IEnumerable<Signal> signals = trigger.MainSignals.Select(m => m.GetComponent<Signal>()).Where(s => s != null);
                foreach (Signal sig in signals)
                {
                    MapSignal(sig.GetComponent<Signal>(), prefix + "-");
                }
            }
            
        }

        public static void MapSignal(Signal signal, string prefix)
        {
            System.Console.Write(prefix + "[SG]");
            if (signal is Signal_AnimEffect animEffect)
            {
                System.Console.Write("AnimEffect(" + (animEffect?.Prefab?.name ?? "Clear") + "):");
            }
            else if (signal is Signal_AnimTrigger animTrigger)
            {
                System.Console.Write("AnimEffect(" + (animTrigger?.AnimKey ?? "Null?") + "):");
            }
            else if (signal is Signal_SoundEvent sound)
            {
                System.Console.Write("SoundFX(" + (sound?.Key ?? "Null?") + "):");
            }
            else
            {
                System.Console.Write(signal.GetType().Name + ": ");
            }
            foreach (string s in signal.GKB().Keys)
            {
                System.Console.Write(s + ", ");
            }
            System.Console.WriteLine();
            List<Medium> mediums = new List<Medium>();
            if (signal is Signal_CreateMedium sigMedium)
            {
                mediums.Add(sigMedium.MediumPrefab.GetComponent<Medium>());
            }
            if (signal is Signal_CreateMediums sigMediums)
            {
                sigMediums.MediumPrefabs.Do(g => mediums.Add(g.GetComponent<Medium>()));
            }
            foreach(Medium medium in mediums)
            {
                System.Console.Write(prefix + "|[M]" + medium.name + ":");
                foreach (string s in medium.GKB().Keys)
                {
                    System.Console.Write(s + ", ");
                }
                System.Console.WriteLine();
                IEnumerable<Signal> signals = medium.Signals.Select(m => m.GetComponent<Signal>()).Where(s => s!=null);
                foreach (Signal sig in signals)
                {
                    MapSignal(sig.GetComponent<Signal>(), prefix + "-");
                }
            }
            if (signal is Signal_AddStatus sigStatus)
            {
                sigStatus.StatusPrefabs?.Do(st => MapStatus(st.GetComponent<Mark_Status>(), prefix + "-"));
            }
            if (signal is Signal_AddSkill sigSkill)
            {
                MapSkill(sigSkill.SkillPrefab?.GetComponent<Mark_Skill>(), prefix + "-");
            }
        }

        public static void ScrapeCards()
        {
            string PrintCardData(string key, Card card)
            {
                string s = key + "::";
                s += card.GetName() + "::";
                s += card.GetRealName() + "::";
                s += card.Life + "::";
                s += card.BaseDamage + "::";
                s += card.Tier + "::";
                foreach (GameObject ini in card.IniSkills)
                {
                    Mark_Skill skill = ini.GetComponent<Mark_Skill>();
                    if (skill != null)
                        s += skill.name + ", ";
                }
                s += "::";
                foreach (GameObject ini in card.IniStatus)
                {
                    Mark_Status skill = ini.GetComponent<Mark_Status>();
                    if (skill != null)
                        s += skill.name + ", ";
                }
                s += "::";
                foreach (GameObject ini in card.IniDefaults)
                {
                    Mark_Skill defSkill = ini.GetComponent<Mark_Skill>();
                    if (defSkill != null)
                        s += defSkill.name + ", ";
                }
                s += "::";
                return s;
            }

            ScrapeCards(PrintCardData);
        }

        public static void ScrapeSignalInfo()
        {
            string PrintSignalData(string key, SignalInfo signal)
            {
                string s = key + "::";
                s += signal.Message;
                return s;
            }

            ScrapeSignalInfo(PrintSignalData);
        }

        public static void ScrapeEvents()
        {
            ADV.Event[] events = ThreadControl.Main.GetComponentsInChildren<ADV.Event>();
            Console.WriteLine("====Scraping Event Data====");
            foreach (ADV.Event e in events)
            {
                if (e == null)
                {
                    continue;
                }
                string s = (e?.Key ?? "Null?") + "::";
                s += e.GetType().FullName + "::";
                if (e is Event_Encounter enc)
                {
                    enc?.Keys.Do((k) => s += k + ", ");
                    s += "::";
                }
                if (e is Event_Random ran)
                {
                    ran?.RandomEvents.Do(e2 => s += (e2?.Key ?? "Null?" + ", "));
                    s += "::";
                }
                if (e is Event_FR_Recruit rec)
                {
                    rec?.Keys.Do((k) => s += k + ",");
                    s += ":";
                }
                if (e is Event_FR_RecruitII rec2)
                {
                        rec2?.Keys.Do((k) => s += k + ", ");
                    s += "::";
                        rec2?.KeysII.Do((k) => s += k + ", ");
                    s += "::";
                }
                Console.WriteLine(s);
            }
            Console.WriteLine("====End of Event Scrape====");

        }

        public static void ScrapeCards(Func<string, Card, string> func)
        {
            Console.WriteLine("====Scraping Card Data====");
            for(int i =0; i<Library.Main.Keys.Count; i++)
            {
                string key = Library.Main.Keys[i];
                Console.WriteLine(func(key, Library.Main.GetCard(key).GetComponent<Card>()));
            }
            Console.WriteLine("====End of Card Scrape====");
        }

        public static void ScrapeSignalInfo(Func<string, SignalInfo, string> func)
        {
            Console.WriteLine("====Scraping Signal Data====");
            for (int i = 0; i < Library.Main.SignalInfoKeys.Count; i++)
            {
                string key = Library.Main.SignalInfoKeys[i];
                Console.WriteLine(func(key, Library.Main.GetSignalInfo(key)));
            }
            Console.WriteLine("====End of Signal Scrape====");
        }

        public static void GatherAnimEffects()
        {
            List<string> animNames = new List<string>();

            foreach(string key in Library.Main.Keys)
            {
                Signal_AnimEffect[] effects = Library.Main.GetCard(key).GetComponentsInChildren<Signal_AnimEffect>();
                foreach(Signal_AnimEffect effect in effects)
                {
                    if (effect?.Prefab != null && !animNames.Contains(effect.Prefab.name))
                    {
                        //System.Console.WriteLine("[Anim] " + effect.Prefab.name + " (" + key + ")");
                        animPrefabs.Add(effect.Prefab);
                        animNames.Add(effect.Prefab.name);
                    }
                }
            }
        }

        public static void GatherAnimTriggers()
        {
            List<string> animNames = new List<string>();

            foreach (string key in Library.Main.Keys)
            {
                Signal_AnimTrigger[] effects = Library.Main.GetCard(key).GetComponentsInChildren<Signal_AnimTrigger>();
                foreach (Signal_AnimTrigger effect in effects)
                {
                    if (effect?.AnimKey != null && !animNames.Contains(effect.AnimKey))
                    {
                        System.Console.WriteLine("[Anim] " + effect.AnimKey + " (" + key + ")");
                        animNames.Add(effect.AnimKey);
                    }
                }
            }
        }

        public static void AddKey(this Library l, string key, GameObject obj)
        {
            l.Keys.Add(key);
            l.CardPrefabs.Add(obj);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Library), nameof(Library.Awake))]
        static void LibraryInit()
        {
            if (modAssets == null)
            {
                modAssets = new GameObject("Mod Assets");
                GameObject.DontDestroyOnLoad(modAssets);
                modAssets.SetActive(false);
                GatherAnimEffects();
                //GatherAnimTriggers();
                ModdedCard.LokiTest();
            }

            ModEvents.InvokeLibraryIni();

            
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Card), nameof(Card.Awake))]
        static bool Prefix()
        {
            return !ModdedCard.freezeAwake;
        }
    }
}
