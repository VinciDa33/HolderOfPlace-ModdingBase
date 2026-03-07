using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static ModUtils.KeyLib;

namespace ModUtils
{
    public static class MarkFactory
    {
        public static T NewMark<T>(string name, params string[] keys) where T : Mark
        {
            GameObject obj = new GameObject(name);
            T mark = obj.AddComponent<T>();
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            mark.KB = keyBase;
            if (mark is Mark_Skill skill)
            {
                skill.Targetings = new List<GameObject>();
                skill.Conditions = new List<GameObject>();
                skill.MainSignals = new List<GameObject>();
            }
            return mark;
        }
        public static T NewSignal<T>(params string[] keys) where T : Signal
        {
            GameObject obj = new GameObject("signal");
            T signal = obj.AddComponent<T>();
            obj.name = typeof(T).Name;
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            signal.KB = keyBase;
            return signal;
        }
        public static Signal NewSignalInfo(this Signal s, string message, string copyFrom = "")
        {
            SignalInfo info = s.gameObject.AddComponent<SignalInfo>();
            info.CopyFrom = copyFrom;
            info.Message = message;
            info.TargetText = "";
            info.SourceText = "";
            return s;
        }
        public static T NewTargeting<T>(params string[] keys) where T : Targeting
        {
            GameObject obj = new GameObject("targeting");
            T targeting = obj.AddComponent<T>();
            obj.name = typeof(T).Name;
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            targeting.KB = keyBase;
            return targeting;
        }
        public static Signal_CreateMedium NewMedium<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMedium>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.MediumPrefab = medium.gameObject;
            return signal;
        }
        
        public static Signal_CreateMedium_Inverse NewMediumInverse<T>(string name, out T medium, params string[] keys) where T : Medium
        {
            var signal = NewSignal<Signal_CreateMedium_Inverse>(keys);
            signal.name = name;
            medium = signal.gameObject.AddComponent<T>();
            KeyBase keybase2 = signal.gameObject.AddComponent<KeyBase>();
            medium.KB = keybase2;
            keybase2.Keys = new List<string>();
            medium.Signals = new List<GameObject>();
            if (medium is Medium_Explosion explosion)
            {
                explosion.SubSignals = new List<GameObject>();
            }
            signal.MediumPrefab = medium.gameObject;
            return signal;
        }

        public static T EditMark<T>(this T mark, Action<T> action) where T : Mark
        {
            action(mark);
            return mark;
        }

        public static T EditSignal<T>(this T signal, Action<T> action) where T : Signal
        {
            action(signal);
            return signal;
        }
        public static Signal_CreateMedium EditMedium<T>(this Signal_CreateMedium signal, Action<T> action) where T:Medium
        {
            T medium = signal.GetComponent<T>();
            if (medium != null)
            {
                action(medium);
            }
            return signal;
        }

        public static void AddKeys(this Medium medium, params string[] keys)
        {
            KeyBase kb = medium.GKB();
            foreach(string key in keys)
            {
                kb.Keys.Add(key);
            }
        }

        public static void AddTo_Generic(Transform parent, List<GameObject> mainList, IEnumerable<GameObject> newObj)
        {
            foreach (GameObject g in newObj)
            {
                g.transform.SetParent(parent);
                mainList.Add(g.gameObject);
            }
        }
        public static Mark_Skill AddSignal(this Mark_Skill skill, params Signal[] signals)
        {
            AddTo_Generic(skill.transform, skill.MainSignals, signals.Select(s => s.gameObject));
            return skill;
        }
        public static Mark_Trigger AddSignal(this Mark_Trigger trigger, params Signal[] signals)
        {
            if (trigger.MainSignals == null)
            {
                trigger.MainSignals = new List<GameObject>();
            }
            AddTo_Generic(trigger.transform, trigger.MainSignals, signals.Select(s => s.gameObject));
            return trigger;
        }
        public static Medium AddSignal(this Medium medium, params Signal[] signals)
        {
            AddTo_Generic(medium.transform, medium.Signals, signals.Select(s => s.gameObject));
            return medium;
        }
        public static Medium_Explosion AddSignal_Explosion(this Medium_Explosion medium, bool addToSubSignalsOnly, params Signal[] signals)
        {
            AddTo_Generic(medium.transform, medium.SubSignals, signals.Select(s => s.gameObject));
            if (!addToSubSignalsOnly)
            {
                AddTo_Generic(medium.transform, medium.Signals, signals.Select(s => s.gameObject));
            }
            return medium;
        }
        public static Mark_Skill AddTargeting(this Mark_Skill skill, params Targeting[] targetings)
        {
            AddTo_Generic(skill.transform, skill.Targetings, targetings.Select(t => t.gameObject));
            return skill;
        }

        public static Signal_Invoke NewInvoke(string skillKey, params string[] keys)
        {
            var invoke = NewSignal<Signal_Invoke>(keys);
            invoke.SkillKey = "Auto";
            return invoke;
        }

        public static Signal_AddSkill NewAddSkill(Mark_Skill skill, params string[] keys)
        {
            var addSkill = NewSignal<Signal_AddSkill>(keys);
            addSkill.SkillPrefab = skill.gameObject;
            skill.transform.SetParent(addSkill.transform);
            return addSkill;
        }

        public static Signal_AddStatus NewAddStatus(Mark_Status status, params string[] keys)
        {
            var addStatus = NewSignal<Signal_AddStatus>(keys);
            addStatus.StatusPrefabs = new List<GameObject> { status.gameObject };
            status.transform.SetParent(addStatus.transform);
            return addStatus;
        }

        public static Signal_AnimEffect NewAnimEffect(string animKey, params string[] keys)
        {
            Signal_AnimEffect anim = NewSignal<Signal_AnimEffect>(keys);
            GameObject animPrefab = LibraryExt.animPrefabs.FirstOrDefault(a => a.name.ToLower() == animKey?.ToLower());
            anim.Prefab = animPrefab;
            return anim;
        }

        public static Signal_SoundEvent NewSound(string soundKey, params string[] keys)
        {
            Signal_SoundEvent sound = NewSignal<Signal_SoundEvent>(keys);
            sound.Key = soundKey;
            return sound;
        }

        public static T InstantiateEditName<T> (this T original, string name) where T:MonoBehaviour
        {
            T newObj = GameObject.Instantiate<T>(original);
            newObj.name = name;
            return newObj;
        }

        public static Mark_Trigger_Signal NewTriggerSignal(string triggerKey, params string[] keys)
        {
            var triggerSignal = NewMark<Mark_Trigger_Signal>(triggerKey,keys);
            triggerSignal.TriggerKey = triggerKey;
            triggerSignal.MainSignals = new List<GameObject>();
            return triggerSignal;
        }

        public static Signal_ColorEffect NewColorEffect(Color color, params string[] keys)
        {
            Signal_ColorEffect signal = NewSignal<Signal_ColorEffect>(keys);
            signal.color = color;
            return signal;
        }

        public static T AddMarkInfo<T>(this T mark, string name, string desc) where T:Mark
        {
            MarkInfo markInfo = mark.AddComponent<MarkInfo>();
            markInfo.Name = name;
            markInfo.Description = desc;
            return mark;
        }

        public static T AddAspectTrigger<T>(this T skill) where T : Mark_Skill
        {
            skill.AddSignal(NewMedium<Medium_Instant>("AspectTrigger", out var instant, TARGET_OTHER));
            /*
            var createMedium = NewSignal<Signal_CreateMedium>(TARGET_OTHER);
            Medium_Instant medium1 = createMedium.AddComponent<Medium_Instant>();
            medium1.KB = medium1.AddComponent<KeyBase>();
            medium1.GKB().Keys = new List<string>();
            createMedium.MediumPrefab = medium1.gameObject;
            */
            var mediumExplosion = NewMediumInverse<Medium_Explosion>("SendToAllFriendly", out var explosion, TARGET_OTHER);
            /*
            var mediumExplosion = NewSignal<Signal_CreateMedium_Inverse>(TARGET_OTHER);
            Medium_Explosion explosion = mediumExplosion.AddComponent<Medium_Explosion>();
            explosion.KB = explosion.AddComponent<KeyBase>();
            explosion.GKB().Keys = new List<string>();
            mediumExplosion.MediumPrefab = explosion.gameObject;
            */
            explosion.AddKeys("TargetAllFriendly[1", "TargetDeath[1", "TargetUntargeted[1", "IgnoreSource[0");

            /*
            explosion.SetKey("TargetAllFriendly", 1);
            explosion.SetKey("TargetDeath", 1);
            explosion.SetKey("TargetUntargeted", 1);
            explosion.SetKey("IgnoreSource", 0);
            */
            explosion.AddSignal_Explosion(false, NewSignal<Signal>(TARGET_OTHER, "OnAspect[1", "OnTraitChange[1"));
            /*
            explosion.Signals = new List<GameObject> { NewSignal<Signal>(TARGET_OTHER, "OnAspect[1", "OnTraitChange[1").gameObject };
            explosion.SubSignals = explosion.Signals;
            explosion.Signals[0].transform.SetParent(explosion.transform);
            */
            instant.AddSignal(mediumExplosion,
                NewSignal<Signal>(TARGET_OTHER, "OnAdditionalTrait[1")
                );
            /*
            instant.Signals = new List<GameObject> { mediumExplosion.gameObject,
                NewSignal<Signal>(TARGET_OTHER, "OnAdditionalTrait[1").gameObject};
            instant.Signals[0].transform.SetParent(instant.transform);
            instant.Signals[1].transform.SetParent(instant.transform);
            */

            //skill.AddSignal(createMedium);
            return skill;
        }

        public static T AddTrinketTrigger<T>(this T skill) where T : Mark_Skill
        {
            skill.AddSignal(NewMedium<Medium_Instant>("Trinket Trigger", out var medInstant, TARGET_OTHER));

            medInstant.AddSignal(NewMediumInverse<Medium_Explosion>("Trinket Trigger II", out var explosion, TARGET_OTHER),
                NewSignal<Signal>(TARGET_OTHER,"OnTrinket[1"));

            explosion.AddKeys("TargetAllFriendly[1", "TargetDeath[1", "TargetUntargeted[1", "IgnoreSource[0");
            explosion.AddSignal_Explosion(false, NewSignal<Signal>(TARGET_OTHER, "OnTrinket[1"));
            return skill;
        }
    }
}
