using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ADV;
using ModUtils;

namespace ModdingCore
{
    public class ErrorPopup : MonoBehaviour
    {
        public static ErrorPopup popup;
        public static String errorTemplate = "ERROR! Press [TAB] to dismiss...\n\n<size=16>{0}</size>";

        public SpriteRenderer image;
        public TextMeshPro textbox;
        public static void Open(string logString, string stackTrace)
        {
            CreatePopup();
            String s = String.Format(errorTemplate, logString + "\n" + stackTrace);
            popup.textbox.SetText(s);
        }

        public static void CreatePopup()
        {
            if (popup != null)
            {
                return;
            }

            GameObject obj = new GameObject("Error Popup");
            popup = obj.AddComponent<ErrorPopup>();


            GameObject parent = FindAnyObjectByType<CodexControl>()?.gameObject 
                ?? FindAnyObjectByType<CombatControl>()?.gameObject
                ?? FindAnyObjectByType<BackgroundControl>()?.gameObject;

            if (parent != null)
            {
                popup.transform.SetParent(parent.transform, false);
                popup.transform.position = new Vector3(130, 10, 0);
                Debug.Log("[MOD] Parent Control Found!");
            }
            else
            {
                Debug.Log("[MOD] Parent Control Not Found");
            }

            popup.image = UIFactory.Box(popup.transform, Vector3.zero, new Color(0.5f, 0, 0, 0.9f));
            popup.image.transform.localScale = new Vector3(200, 100, 1);

            GameObject textObject = new GameObject("Textbox");
            popup.textbox = textObject.AddComponent<TextMeshPro>();
            textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 60);
            textObject.transform.SetParent(obj.transform);

            Debug.Log("[MOD] New Popup Created!");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Destroy(popup.gameObject);
            }
        }
    }
}
