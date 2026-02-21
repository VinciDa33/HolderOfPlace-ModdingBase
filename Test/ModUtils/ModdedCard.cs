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

        public static void LokiTest()
        {
            //The three S's of HoP coding:
            //[S]ignals do things but don't stick around.
            //[S]tatuses stick around but lack nuanced targeting.
            //[S]kills can target but can only act through signals.

            var signalDemonize = NewSignal<Signal_AddStatus>(TARGET_OTHER);
                var demonize = NewMark<Mark_Status_DamageInputMod>("Demonize", "DamageMod[2", "Mod[1", "TriggerCount[1");
                demonize.transform.SetParent(signalDemonize.transform);
                signalDemonize.StatusPrefabs = new List<GameObject> { demonize.gameObject };
            
            var applyDemonize = NewMark<Mark_Skill>("Apply Demonize", PERMANENT, AFTER_ATTACK, TRAIT)
                .AddTargeting(NewTargeting<Targeting_RandomEnemy>())
                .AddSignal(signalDemonize,
                    Signal_AnimColor.NewAnimColor(new Color(0.4f, 0.25f, 0.8f), TARGET_OTHER));
                

            //Todo: need to add animation signals (and find a nice way of accessing all available ones)
            //Or maybe create a status display? (<- sounds tedious to code)
            

            CreateNewCard(BootstrapMain.StreamingAssetPath)
                .SetName("Loki", "Loki")
                .SetDesc("After attacking, apply Demonize to a random enemy")
                .SetStats(1, 2, 0)
                .SetPools("Leader") //Virtual pool. The real pools are Active, Passive, Aspect, Trinket, Moth, ManaAspect
                .SetBasePicture("LokiIcon.png")
                .SetBackground("LokiBackground.png")
                .SetIdeogram("IdeogramLoki.png")
                .SetPortrait("PortraitLoki.png")
                .AddSkills(applyDemonize);
        }

        public static ModdedCard CreateNewCard(string path)
        {
            if (Library.Main == null)
            {
                System.Console.WriteLine("[ModUtil] Cannot Create a new care until a run or journal is open!");
                ErrorPopup.Open("CreateNewCard Mistiming!", "You cannot create a new card until a run or journal is open!");
                return null;
            }
            freezeAwake = true;
            Card card = Instantiate(Library.Main.GetCard("Militia"), LibraryExt.modAssets.transform).GetComponent<Card>();
            freezeAwake = false;
            ModdedCard moddedCard = card.AddComponent<ModdedCard>();
            moddedCard.Ini();
            moddedCard.dirPath = path;
            return moddedCard;
        }

        public static ModdedCard CreateNewCard(HopMod mod)
        {
            ModdedCard card = CreateNewCard(mod.modPath);
            return card;
        }

        internal void Ini()
        {
            _card = GetComponent<Card>();
            _cardInfo = GetComponent<CardInfo>();
            _skillParent = transform.Find("Skills");
            _statusParent = transform.Find("Status");
        }

        public ModdedCard SetName(string renderName, string realName)
        {
            _cardInfo.RenderName = renderName;
            _cardInfo.RealName = realName;
            _cardInfo.Name = realName;
            gameObject.name = "@" + realName;
            return this;
        }

        public ModdedCard SetDesc(string desc)
        {
            _cardInfo.Description = desc;
            return this;
        }

        public ModdedCard SetStats(float baseDamage, float life, float costTier)
        {
            _card.MaxLife = life;
            _card.Life = life;
            _card.BaseDamage = baseDamage;
            _card.Cost = costTier;
            return this;
        }

        public ModdedCard SetPortrait(string path)
        {
            return SetPortrait(BootstrapMain.GetSprite(dirPath + "/" + path));
        }

        public ModdedCard SetPortrait(Sprite sprite)
        {
            _cardInfo.Portrait = sprite;
            return this;
        }

        public ModdedCard SetBasePicture(string path)
        {
            return SetBasePicture(BootstrapMain.GetSprite(dirPath + "/" + path));
        }

        public ModdedCard SetBasePicture(Sprite sprite)
        {
            SpriteRenderer render = transform.Find("AnimBase/NewAliveBase/Base").GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            return this;
        }

        public ModdedCard SetBackground(string path)
        {
            return SetBackground(BootstrapMain.GetSprite(dirPath + "/" + path));
        }

        public ModdedCard SetBackground(Sprite sprite)
        {
            SpriteRenderer render = transform.Find("AnimBase/NewAliveBase/Background").GetComponent<SpriteRenderer>();
            render.sprite = sprite;
            return this;
        }

        public ModdedCard SetIdeogram(string path)
        {
            return SetIdeogram(BootstrapMain.GetSprite(path));
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
