using ADV;
using HarmonyLib;
using ModdingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static ModUtils.MarkFactory;
using static ModUtils.KeyLib;

namespace ModUtils
{
    public enum CardType
    {
        Follower,
        PassiveFollower,
        Aspect,
        Trinket,
        Enemy
    }
    public class ModdedCard : MonoBehaviour
    {
        

        public Card _card;
        public CardInfo _cardInfo;
        public Transform _skillParent;
        public Transform _statusParent;

        public HopMod mod;
        public string dirPath;

        public string[] pools;

        public static bool freezeAwake = false;

        public static ModdedCard CreateNewCard(string path, CardType type)
        {
            if (Library.Main == null)
            {
                System.Console.WriteLine("[ModUtil] Cannot Create a new care until a run or journal is open!");
                ErrorPopup.Open("CreateNewCard Mistiming!", "You cannot create a new card until a run or journal is open!");
                return null;
            }
            freezeAwake = true;
            Card card;
            switch(type)
            {
                default:
                case CardType.Follower:
                    card = Instantiate(Library.Main.GetCard("Militia"), LibraryExt.modAssets.transform).GetComponent<Card>();
                    break;
                case CardType.PassiveFollower:
                    card = Instantiate(Library.Main.GetCard("Obelisk"), LibraryExt.modAssets.transform).GetComponent<Card>();
                    for(int i = card.IniSkills.Count-1; i>=0; i--)
                    {
                        Destroy(card.IniSkills[i]);
                    }
                    card.IniSkills.Clear();
                    break;
                case CardType.Aspect:
                    card = Instantiate(Library.Main.GetCard("Sacrificial"), LibraryExt.modAssets.transform).GetComponent<Card>();
                    for (int i = card.IniSkills.Count - 1; i >= 0; i--)
                    {
                        Destroy(card.IniSkills[i]);
                    }
                    card.IniSkills.Clear();
                    break;
                case CardType.Trinket:
                    card = Instantiate(Library.Main.GetCard("Refresh"), LibraryExt.modAssets.transform).GetComponent<Card>();
                    for (int i = card.IniSkills.Count - 1; i >= 0; i--)
                    {
                        Destroy(card.IniSkills[i]);
                    }
                    card.IniSkills.Clear();
                    for (int i = card.AddConditions.Count-1; i>=0; i--)
                    {
                        Destroy(card.AddConditions[i]);
                    }
                    card.AddConditions.Clear();
                    break;

            }
            card.Info.Portrait = null;
            freezeAwake = false;
            ModdedCard moddedCard = card.AddComponent<ModdedCard>();
            moddedCard.Ini();
            moddedCard.dirPath = path;
            return moddedCard;
        }

        public static ModdedCard CreateNewCard(HopMod mod, CardType type)
        {
            ModdedCard card = CreateNewCard(mod.modPath, type);
            card.mod = mod;
            return card;
        }

        internal void Ini()
        {
            _card = GetComponent<Card>();
            _cardInfo = GetComponent<CardInfo>();
            _skillParent = transform.Find("Skills");
            _statusParent = transform.Find("Status");
        }

        public ModdedCard SetName(string name, string key)
        {
            _cardInfo.RenderName = name;
            _cardInfo.RealName = name;
            _cardInfo.Name = mod.Guid + "." + name;
            gameObject.name = "@" + name;
            _cardInfo.Key = mod.Guid + "." + key;
            return this;
        }

        public ModdedCard SetDesc(string desc)
        {
            _cardInfo.Description = desc;
            return this;
        }

        public ModdedCard SetStats(float baseDamage, float life, float cost)
        {
            _card.MaxLife = life;
            _card.Life = life;
            _card.BaseDamage = baseDamage;
            SetCost((int)cost);
            return this;
        }

        public static int[] costTiers = { 0, 1, 3, 6, 10, 14, 18, 30 };
        public ModdedCard SetCost(int cost)
        {
            cost = Math.Max(0, cost);
            for (int i=0; i<costTiers.Length; i++)
            {
                if (costTiers[i] == cost)
                {
                    _card.Cost = i;
                    return this;
                }
                if (costTiers[i] > cost)
                {
                    int prevTierDiff = cost - costTiers[i - 1];
                    int nextTierDiff = costTiers[i + 1] - cost;
                    if (prevTierDiff <= nextTierDiff)
                    {
                        _card.Cost = i - 1;
                        _card.SetKey("AddCost", prevTierDiff);
                        return this;
                    }
                    else
                    {
                        _card.Cost = i;
                        _card.SetKey("AddCost", -nextTierDiff);
                        return this;
                    }
                }
            }
            _card.Cost = 7;
            return this;
        }

        public ModdedCard SetPortrait(string path)
        {
            return SetPortrait(BootstrapMain.GetSprite(dirPath + "/Images/" + path));
        }

        public ModdedCard SetPortrait(Sprite sprite)
        {
            _cardInfo.Portrait = sprite;
            return this;
        }

        public ModdedCard SetBasePicture(string path)
        {
            return SetBasePicture(BootstrapMain.GetSprite(dirPath + "/Images/" + path));
        }

        public ModdedCard SetBasePicture(Sprite sprite)
        {
            SpriteRenderer render = transform.Find("AnimBase/NewAliveBase/Base").GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            return this;
        }

        public ModdedCard SetBackground(string path)
        {
            return SetBackground(BootstrapMain.GetSprite(dirPath + "/Images/" + path));
        }

        public ModdedCard SetBackground(Sprite sprite)
        {
            SpriteRenderer render = transform.Find("AnimBase/NewAliveBase/Background").GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            return this;
        }

        public ModdedCard SetIdeogram(string path)
        {
            return SetIdeogram(BootstrapMain.GetSprite(dirPath + "/Images/" + path));
        }

        public ModdedCard SetIdeogram(Sprite sprite)
        {
            SpriteRenderer render = transform.Find("AnimBase/NewAliveBase/Ideogram").GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            return this;
        }

        public ModdedCard ChangeKeys(params string[] keys)
        {
            keys.Do(k => _card.ChangeKey(KeyBase.Translate(k,out float v), v));
            return this;
        }

        public ModdedCard SetKeys(params string[] keys)
        {
            keys.Do(k => _card.SetKey(KeyBase.Translate(k, out float v), v));
            return this;
        }

        public ModdedCard SetPools(params string[] poolNames)
        {
            pools = poolNames;
            return this;
        }

        public ModdedCard AddSkills(params Mark_Skill[] skills)
        {
            skills.Do(sk =>
            {
                _card.IniSkills.Add(sk.gameObject);
                sk.transform.SetParent(_skillParent.transform);
            });
            return this;
        }

        public ModdedCard RemoveSkill(int index)
        {
            GameObject obj = _card.IniSkills[index];
            _card.IniSkills.RemoveAt(index);
            Destroy(obj);
            return this;
        }

        public ModdedCard AddStatuses(params Mark_Status[] statuses)
        {
            statuses.Do(st =>
            {
                _card.IniStatus.Add(st.gameObject);
                st.transform.SetParent(_statusParent.transform);
            });
            return this;
        }
    }
}
