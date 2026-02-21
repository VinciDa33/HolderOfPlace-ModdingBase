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

        TMP_InputField inputField;

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
            }
            transform.localPosition = new Vector3(52, -26, 0);
            center = UIFactory.Button(transform, Vector3.zero, 20, 20);
            center.action = CenterClicked;
            SpriteRenderer centerBox = UIFactory.Box(center.transform, Vector3.zero, Color.gray);
            centerBox.transform.localScale = new Vector3(20, 20, 1);
            left = UIFactory.Button(transform, new Vector3(-13,0,0), 4, 20);
            left.action = LeftClicked;
            SpriteRenderer leftBox = UIFactory.Box(left.transform, Vector3.zero, Color.gray);
            leftBox.transform.localScale = new Vector3(4, 20, 1);
            right = UIFactory.Button(transform, new Vector3(13, 0, 0), 4, 20);
            right.action = RightClicked;
            SpriteRenderer rightBox = UIFactory.Box(right.transform, Vector3.zero, Color.gray);
            rightBox.transform.localScale = new Vector3(4, 20, 1);

            textbox = UIFactory.Text(transform, new Vector3(0, 10, 0), "Mod: Off", 24);


            inputField = UIFactory.NewInputField("Test Input", transform, new Vector3(0, -10, 0), new Vector2(50, 6), "Try typing something!");
            inputField.transform.localScale = new Vector3(5, 5, 1);
        }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CenterClicked();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                LeftClicked();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RightClicked();
            }
        }

        public void CenterClicked()
        {
            System.Console.WriteLine("You clicked on a mod. Good Job!");
            inputField.ActivateInputField();
            inputField.Select();
            if (textbox.text == "Mod: Off")
            {
                textbox.SetText("Mod: On");
            }
            else
            {
                textbox.SetText("Mod: Off");
            }
        }

        public void LeftClicked()
        {
            System.Console.WriteLine("You clicked on the left button. Good Job!");
        }

        public void RightClicked()
        {
            System.Console.WriteLine("You clicked on the right button. Good Job!");
        }
    }
}
