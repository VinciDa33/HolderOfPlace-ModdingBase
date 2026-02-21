using ADV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static T AddSignal<T>(this T skill, params Signal[] signals) where T : Mark_Skill
        {
            foreach(Signal s in signals)
            {
                s.transform.SetParent(skill.transform);
                skill.MainSignals.Add(s.gameObject);
            }
            return skill;
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

        public static T AddTargeting<T>(this T skill, params Targeting[] targetings) where T : Mark_Skill
        {
            foreach(Targeting t in targetings)
            {
                t.transform.SetParent(skill.transform);
                skill.Targetings.Add(t.gameObject);
            }
            return skill;
        }

        public static T NewTargeting<T>(params string[] keys) where T : Targeting
        {
            GameObject obj = new GameObject("signal");
            T targeting = obj.AddComponent<T>();
            obj.name = typeof(T).Name;
            KeyBase keyBase = obj.AddComponent<KeyBase>();
            keyBase.Keys = keys.ToList();
            targeting.KB = keyBase;
            return targeting;
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
    }
}
