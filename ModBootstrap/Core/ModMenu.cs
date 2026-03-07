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
    public class ModMenu : MonoBehaviour
    {
        public static ModMenu main;

        UIButton_Generic center;
        UIButton_Generic left;
        UIButton_Generic right;
        TextMeshPro textbox;
        TextMeshPro textbox2;
        TextMeshPro textbox3;
        SpriteRenderer modIcon;
        Sprite spriteFallback;
        int index = 0;
        List<HopMod> mods = new List<HopMod>();
        HopMod mod;

        public static void CreateModMenu()
        {
            if (main != null)
            {
                return;
            }

            main = new GameObject("ModMenu").AddComponent<ModMenu>();

        }

        public void Start()
        {
            Transform parent = GameObject.Find("SettingsGroup")?.transform;
            if (parent == null)
            {
                System.Console.WriteLine("Could not find settings. Weird.");
            }
            else
            {
                transform.SetParent(parent, false);
                TextMeshPro textbox = UIFactory.Text(GameObject.FindAnyObjectByType<TitleScreenControl>().transform, new Vector3(0, -8.5f, 10), "Modded\n" + BootstrapMain.version, 18);
                textbox.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 10);
            }
            transform.localPosition = new Vector3(52, -3, 0);
            center = UIFactory.Button(transform, Vector3.zero, 20, 20);
            center.action = CenterClicked;
            modIcon = UIFactory.Box(center.transform, Vector3.zero, Color.gray);
            modIcon.transform.localScale = new Vector3(20, 20, 1);
            left = UIFactory.Button(transform, new Vector3(-13,0,0), 4, 20);
            left.action = LeftClicked;
            SpriteRenderer leftBox = UIFactory.Box(left.transform, Vector3.zero, Color.gray);
            leftBox.transform.localScale = new Vector3(4, 20, 1);
            right = UIFactory.Button(transform, new Vector3(13, 0, 0), 4, 20);
            right.action = RightClicked;
            SpriteRenderer rightBox = UIFactory.Box(right.transform, Vector3.zero, Color.gray);
            rightBox.transform.localScale = new Vector3(4, 20, 1);

            textbox = UIFactory.Text(transform, new Vector3(0, 11, 0), "Mod: Off", 24);
            textbox.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 5);
            textbox.alignment = TextAlignmentOptions.Top;
            textbox2 = UIFactory.Text(transform, new Vector3(-1, -13, 0), "0 of 0", 18);
            //textbox2.color = Color.gray;
            textbox2.alignment = TextAlignmentOptions.TopRight;
            textbox3 = UIFactory.Text(transform, new Vector3(-1, -16, 0), "0 of 0", 18);
            textbox3.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 5);
            spriteFallback = rightBox.sprite;

            

            BootstrapMain.LoadMods();
            mods = BootstrapMain.modDictionary.Values.OrderBy(m => m.Guid).ToList();
            UpdateIcon();
        }

        public void UpdateIcon()
        {
            if (mods.Count == 0)
            {
                return;
            }
            mod = mods[index];
            if (mod.icon == null)
            {
                modIcon.sprite = spriteFallback;
                modIcon.transform.localScale = new Vector3(20, 20, 1);
            }
            else
            {
                modIcon.sprite = mod.icon;
                modIcon.transform.localScale = new Vector3(2000f / mod.icon.rect.width, 2000f / mod.icon.rect.height);
            }
            string on = mod.enabled ? "On" : "Off";
            modIcon.color = mod.enabled ? Color.white : Color.gray;
            textbox.SetText(mod.Title + ": " + on);
            textbox2.SetText("" + (index+1) + " of " + mods.Count);
            textbox3.SetText(mod.Description);
        }

        public void CenterClicked()
        {   
            if (mod == null)
            {
                return;
            }
            if (mod.enabled)
            {
                BootstrapMain.UnloadMod(mod);
            }
            else
            {
                BootstrapMain.LoadMod(mod);
            }
            UpdateIcon();
        }

        public void LeftClicked()
        {
            if (mods.Count == 0)
            {
                return;
            }
            index--;
            if (index == -1)
            {
                index = mods.Count - 1;
            }
            UpdateIcon();
        }

        public void RightClicked()
        {
            if (mods.Count == 0)
            {
                return;
            }
            index++;
            index %= mods.Count;
            UpdateIcon();
            
        }
    }
}
