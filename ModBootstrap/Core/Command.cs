using ADV;
using ModUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModdingCore
{
    public abstract class Command
    {
        public string id = "command";
        public static void LoadInitCommands(Dictionary<string, Command> dictionary)
        {
            dictionary["recruit"] = new CommandUnits()
            {
                id = "recruit",
                keyFunc = CommandUnits.Recruit
            };
            dictionary["map"] = new CommandUnits()
            {
                id = "map",
                keyFunc = CommandUnits.Map
            };
            dictionary["reroll"] = new CommandUnits()
            {
                id = "reroll",
                keyFunc = CommandUnits.Reroll
            };
            dictionary["faith"] = new CommandNumber()
            {
                id = "faith",
                floatFunc = CommandNumber.Faith
            };
            dictionary["locus"] = new CommandNumber()
            {
                id = "locus",
                floatFunc = CommandNumber.Locus
            };
        }

        public abstract void Run(List<string> messages);
        public virtual List<string> OnValueChanged(string[] messages)
        {
            return null;
        }

        public int SafeParseInt(string message, int defaultValue)
        {
            if (int.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public float SafeParseFloat(string message, float defaultValue)
        {
            if (float.TryParse(message, out var value))
            {
                return value;
            }
            return defaultValue;
        }

        public class CommandUnits : Command
        {
            public Action<IEnumerable<string>> keyFunc;
            public override void Run(List<string> messages)
            {
                if (Library.Main == null)
                {
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
            }

            public static void Map(IEnumerable<string> keys)
            {
                foreach (string key in keys)
                {
                    LibraryExt.MapCard(key, true);
                }
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
            }
        }

        public class CommandNumber : Command
        {
            public Action<float> floatFunc;
            public override void Run(List<string> messages)
            {
                if (messages.Count == 0)
                {
                    return;
                }

                floatFunc(SafeParseFloat(messages[0], 1));
            }

            public static void Faith(float amount)
            {
                CombatControl.Main.ChangeFate(amount);
            }

            public static void Locus(float amount)
            {
                CombatControl.Main.ChangeMana(amount);
            }
        }
    }
}
