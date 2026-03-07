using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ModdingCore
{
    public abstract class Command
    {
        public string id = "command";
        public static void LoadInitCommands()
        {
            AddCommand(new CommandUnits()
            {
                id = "recruit",
                keyFunc = CommandUnits.Recruit
            });
            AddCommand(new CommandUnits()
            {
                id = "map",
                keyFunc = CommandUnits.Map
            });
            AddCommand(new CommandUnits()
            {
                id = "reroll",
                keyFunc = CommandUnits.Reroll
            });
            AddCommand(new CommandNumber()
            {
                id = "faith",
                action = CommandNumber.Faith
            });
            AddCommand(new CommandNumber()
            {
                id = "locus",
                action = CommandNumber.Locus
            });
            AddCommand(new CommandNumber()
            {
                id = "core",
                action = CommandNumber.Core
            });
            AddCommand(new CommandCard()
            {
                id = "life",
                action = CommandCard.Life
            });
            AddCommand(new CommandCard()
            {
                id = "damage",
                action = CommandCard.Damage
            });
            AddCommand(new CommandCard()
            {
                id = "destroy",
                action = CommandCard.Destroy
            });
            AddCommand(new CommandGeneric()
            {
                id = "skip",
                action = CommandGeneric.SkipEvent
            });
        }

        public static void AddCommand(Command c) => CommandLine.commands[c.id] = c;

        public abstract void Run(List<string> messages);
        public virtual List<string> OnValueChanged(string[] messages)
        {
            return null;
        }

        public static int SafeParseInt(string message, int defaultValue)
        {
            if (int.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public static float SafeParseFloat(string message, float defaultValue)
        {
            if (float.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public static void Fail(string message)
        {
            (CommandLine.inputField.placeholder as TextMeshPro).SetText("ERROR: " + message);
        }
        public static void Success()
        {
            (CommandLine.inputField.placeholder as TextMeshPro).SetText("Command successfully executed!");
        }

        public class CommandUnits : Command
        {
            public Action<IEnumerable<string>> keyFunc;
            public override void Run(List<string> messages)
            {
                if (Library.Main == null)
                {
                    Fail("Command can only be used in a run or in the codex");
                    return;
                }

                IEnumerable<string> keys = messages.Select(s => LibraryExt.FindBestKey(s)).Where(k => k != null);
                keyFunc(keys);
            }

            public override List<string> OnValueChanged(string[] messages)
            {
                if (messages.Length == 0 || messages[messages.Length - 1] == "" || Library.Main == null)
                {
                    return null;
                }

                List<string> keys = LibraryExt.FindPartialKey(messages[messages.Length - 1]);
                keys = keys.GetRange(0, Math.Min(5, keys.Count));
                return keys;
            }

            public static void Recruit(IEnumerable<string> keys)
            {
                foreach (string key in keys)
                {
                    RecruitPanel.Main.DirectRecruit(key, true);
                }
                Success();
            }

            public static void Map(IEnumerable<string> keys)
            {
                foreach (string key in keys)
                {
                    LibraryExt.MapCard(key, true);
                }
                Success();
            }

            public static void Reroll(IEnumerable<string> keys)
            {
                List<string> validKeys = keys.ToList();
                RecruitPanel.Main.ResetSlot(IgnoreLock: false);
                if (validKeys.Count > 0)
                {
                    RecruitPanel.Main.ResetSlot(IgnoreLock: false);
                    RecruitPanel.Main.RecruitOverride = validKeys;
                    RecruitPanel.Main.NewRecruitProcess(CanSkip: true);
                }
                else
                {
                    ThreadControl.Main.CurrentEvent.SpecialAction("ForceRefresh");
                }
                //RecruitPanel.Main.NewRecruitProcess(CanSkip: true);
                Success();
            }
        }

        public class CommandGeneric : Command
        {
            public Action<List<string>> action;

            public override void Run(List<string> messages)
            {
                action(messages);
            }

            public static void SkipEvent(List<string> messages)
            {
                if (ThreadControl.Main == null)
                {
                    Fail("Run has not started");
                }

                ThreadControl.Main.NextEvent();
            }
        }

        public class CommandNumber : Command
        {
            public Action<float> action;
            public override void Run(List<string> messages)
            {
                if (messages.Count == 0)
                {
                    messages.Add("1");
                }

                action(SafeParseFloat(messages[0], 1));
            }

            public static void Faith(float amount)
            {
                CombatControl.Main.ChangeFate(amount);
                Success();
            }

            public static void Locus(float amount)
            {
                CombatControl.Main.ChangeMana(amount);
                Success();
            }

            public static void Core(float amount)
            {
                CombatControl.Main.SetCoreLife(amount);
                Success();
            }
        }

        public class CommandCard : Command
        {
            public Action<List<string>, Card> action;
            public override void Run(List<string> messages)
            {
                if (UIControl.Main?.SelectingCard == null)
                {
                    Fail("Must be hovering over a card to use command");
                    return;
                }
                action(messages, UIControl.Main.SelectingCard);
            }

            public static void Life(List<string> messages,  Card card)
            {
                if (messages.Count==0)
                {
                    messages.Add("1");
                }

                float amount = SafeParseFloat(messages[0], 1);
                card.SetMaxLife(amount);
                card.SetLife(amount);
                Success();
            }

            public static void Damage(List<string> messages, Card card)
            {
                if (messages.Count == 0)
                {
                    messages.Add("1");
                }

                float amount = SafeParseFloat(messages[0], 1);
                card.SetBaseDamage(amount);
                Success();
            }

            public static void Destroy(List<string> messages, Card card)
            {
                card.Destroy();
            }
        }
    }
}
