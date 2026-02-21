using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            LibraryExt.leaderPool.Clear();
            foreach(ModdedCard c in LibraryExt.modAssets.GetComponentsInChildren<ModdedCard>())
            {
                if (!c.gameObject.activeSelf)
                {
                    continue;
                }
                Library.Main.AddKey(c._cardInfo.RealName, c.gameObject);
                lists.Do(l =>
                {
                    if (c.pools.Contains(l.name))
                    {
                        l.Keys.Add(c._cardInfo.RealName);
                    }
                });
                if (c.pools.Contains("Leader"))
                {
                    LibraryExt.leaderPool.Add(c._cardInfo.RealName);
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
            OnCardGenerated?.Invoke(card);
        }

        public static void InvokeCardRecruited(Card card)
        {
            System.Console.WriteLine("[ModEvent] CardRecruited");
            OnCardRecruited?.Invoke(card);
        }
    }
}
