using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ADV;
using ModUtils;

namespace ModdingCore
{
    [HarmonyPatch]
    static class PatchInEvents
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenControl), "Start")]
        static void AddModMenu()
        {
            System.Console.WriteLine("[Mod] Added Mod Menu");
            ModMenu.CreateModMenu();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ThreadControl), nameof(ThreadControl.LoadEventList), new Type[]
        {
            typeof(EventList)
        })]
        static void ThreadGenerated(ThreadControl __instance)
        {
            ModEvents.InvokeThreadGenerated(__instance.Thread);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Event), nameof(Event.PreActivate))]
        static void EventPreActivated(Event __instance)
        {
            ModEvents.InvokeEventPreActivated(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Event), nameof(Event.Activate))]
        static void EventActivated(Event __instance)
        {
            ModEvents.InvokeEventActivated(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RecruitPanel), nameof(RecruitPanel.DirectRecruit), new Type[]
        {
            typeof(string),
            typeof(bool)
        })]
        static void CardRecruited(RecruitPanel __instance, Card __result)
        {
            ModEvents.InvokeCardRecruited(__result);
        }
        
    }

    [HarmonyPatch(typeof(CodexEntry_EncounterInfo), nameof(CodexEntry_EncounterInfo.GetVisitedCount), new Type[] { })]
    static class TestPatch
    {
        static void Postfix(CodexEntry_EncounterInfo __instance)
        {
            throw new Exception("Test of the error popup - Can you see me?");
        }
    }

    [HarmonyPatch(typeof(Event_FR_RecruitII), nameof(Event_FR_RecruitII.IniRecruit), new Type[]
    {
        typeof(List<string>),
        typeof(List<string>)
    })]
    static class TestPatch2
    {
        static void Postfix(Event_FR_RecruitII __instance)
        {
            if (__instance.FirstRecruit)
            {
                foreach(string key in LibraryExt.leaderPool)
                {
                    RecruitPanel.Main.RecruitOverride.Add(key);
                }
            }
        }
    }
}
