using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameObject = UnityEngine.GameObject;
using UnityEngine.Events;

namespace ModUtils
{
    public class ModEvents
    {
        public delegate void CardHandler(Card card);
        public delegate void CardCardHandler(Card oldCard, Card newCard);

        public static event UnityAction OnRunStarted;
        //public static event UnityAction OnRunFinished;

        public static event UnityAction OnLibraryIni;

        //public static event UnityAction OnThreadPreGenerate;
        public static event UnityAction<List<Event>> OnThreadGenerated;

        public static event UnityAction<Event> OnEventPreActivated;
        public static event UnityAction<Event> OnEventActivated;

        public static event CardHandler OnCardGenerated;
        public static event CardHandler OnCardRecruited;
        //public static event CardHandler OnCardRemoved;
        //public static event CardCardHandler OnCardReplaced;
        //public static event CardCardHandler OnTrinketAttached;

        public static void InvokeRunStarted()
        {
            System.Console.WriteLine("[ModEvent] RunStarted");
            OnRunStarted?.Invoke();
        }

        public static void InvokeLibraryIni()
        {
            System.Console.WriteLine("[ModEvent] LibraryIni");
            OnLibraryIni?.Invoke();
        }

        public static void InvokeThreadGenerated(List<Event> events)
        {
            System.Console.WriteLine("[ModEvent] ThreadGenerated");
            OnThreadGenerated?.Invoke(events);

            KeyList[] lists = ThreadControl.Main.GetComponentsInChildren<KeyList>();
            LibraryExt.ClearAllPools();
            //Need to clean out hollowPool of sandbat himself.
            foreach(ModdedCard c in LibraryExt.modAssets.GetComponentsInChildren<ModdedCard>())
            {
                if (!c.gameObject.activeSelf || (c.mod != null && !c.mod.enabled))
                {
                    continue;
                }
                Library.Main.AddKey(c._cardInfo.Key, c.gameObject);
                lists.Do(l =>
                {
                    if (c.pools.Contains(l.Key))
                    {
                        l.Keys.Add(c._cardInfo.Key);
                    }
                });

                void TryAddToVirtualPool(string pool)
                {
                    if (c.pools.Contains(pool))
                    {
                        LibraryExt.virtPools[pool].Add(c._cardInfo.Key);
                    }
                }

                TryAddToVirtualPool("Origin");
                TryAddToVirtualPool("Sea");
                TryAddToVirtualPool("Hollow");
                TryAddToVirtualPool("Trinket_Early");
                TryAddToVirtualPool("Trinket_Middle");
                TryAddToVirtualPool("Trinket_Late");

            }
            Event @event = ThreadControl.Main.FindEvent("Recruit_Sea_Real");
            if (@event is Event_FR_RecruitII seaEvent)
            {
                System.Console.WriteLine("[CORE] Adding To Sea Pool");
                foreach (string key in LibraryExt.virtPools["Sea"])
                {
                    seaEvent.Keys.Add(key);
                } 
            }
            @event = ThreadControl.Main.FindEvent("Recruit_II_Trinket");
            if (@event is Event_FR_RecruitII trinketEarly)
            {
                System.Console.WriteLine("[CORE] Adding To Early Trinkets");
                foreach (string key in LibraryExt.virtPools["Trinket_Early"])
                {
                    trinketEarly.AddKeys.Add(key + "[2");
                }
            }
            @event = ThreadControl.Main.FindEvent("Recruit_III_Trinket");
            if (@event is Event_FR_RecruitII trinketMiddle)
            {
                System.Console.WriteLine("[CORE] Adding To Mid Trinkets");
                foreach (string key in LibraryExt.virtPools["Trinket_Middle"])
                {
                    trinketMiddle.AddKeys.Add(key + "[2");
                }
            }
            @event = ThreadControl.Main.FindEvent("Recruit_FirstEndless");
            if (@event is Event_FR_RecruitII trinketEnd)
            {
                System.Console.WriteLine("[CORE] Adding To Endless Trinkets");
                foreach (string key in LibraryExt.virtPools["Trinket_Late"])
                {
                    trinketEnd.KeysII.Add(key);
                }
            }
            @event = ThreadControl.Main.FindEvent("@Recruit_Endless");
            if (@event is Event_FR_RecruitII trinketEnd2)
            {
                System.Console.WriteLine("[CORE] Adding To Endless Trinkets II");
                foreach (string key in LibraryExt.virtPools["Trinket_Late"])
                {
                    trinketEnd2.KeysII.Add(key);
                }
            }

            GameObject sandbat = Library.Main.GetCard("Sandbat");
            if (sandbat != null)
            {
                System.Console.WriteLine("[CORE] Adding to Hollow Pool");
                Signal_ReplaceRecruitOverride signal = sandbat.GetComponentInChildren<Signal_ReplaceRecruitOverride>();
                for(int i=signal.Keys.Count-1; i>=7; i--)
                {
                    signal.Keys.RemoveAt(i);
                }
                foreach (string key in LibraryExt.virtPools["Hollow"])
                {
                    signal.Keys.Add(key);
                }
            }
        }

        public static void InvokeEventPreActivated(Event @event)
        {
            System.Console.WriteLine("[ModEvent] EventPreactivated");
            OnEventPreActivated?.Invoke(@event);
        }

        public static void InvokeEventActivated(Event @event)
        {
            System.Console.WriteLine("[ModEvent] EventActivated");
            OnEventActivated?.Invoke(@event);
        }

        public static void InvokeCardGenerated(Card card)
        {
            System.Console.WriteLine("[ModEvent] CardGenerated");
            card.name = card.name.Replace("(Clone)", "");
            OnCardGenerated?.Invoke(card);
        }

        public static void InvokeCardRecruited(Card card)
        {
            System.Console.WriteLine("[ModEvent] CardRecruited");
            OnCardRecruited?.Invoke(card);
        }
    }
}
