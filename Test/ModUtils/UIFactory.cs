using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModdingCore;
using TMPro;
using UnityEngine.UI;

namespace ModUtils
{
    
    public static class UIFactory
    {
        public static UIButton_Generic Button(Transform parent, Vector3 localPosition, int width, int height)
        {
            GameObject obj = new GameObject("Box");
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            obj.transform.localPosition = localPosition;
            UIButton_Generic button = obj.AddComponent<UIButton_Generic>();
            button.CursorRangeX = new Vector2(-width / 2, width / 2);
            button.CursorRangeY = new Vector2(-height / 2, height / 2);
            return button;
        }
        public static SpriteRenderer Box(Transform parent, Vector3 localPosition, Color c)
        {
            GameObject obj = new GameObject("Box");
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            obj.transform.localPosition = localPosition;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = BootstrapMain.GetSprite(BootstrapMain.StreamingAssetPath + "/Box.png");
            sr.color = c;
            return sr;
        }

        public static TextMeshPro Text(Transform parent, Vector3 localPosition, string text, int fontSize = 36)
        {
            GameObject obj = new GameObject("Text");
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            obj.transform.localPosition = localPosition;
            TextMeshPro textbox = obj.AddComponent<TextMeshPro>();
            textbox.SetText(text);
            textbox.fontSizeMax = fontSize;
            textbox.fontSizeMin = fontSize;
            textbox.alignment = TextAlignmentOptions.Top;
            
            return textbox;
        }

        public static TMP_InputField NewInputField(string name, Transform parent, Vector3 position, Vector2 size, string defaultText, float fontRatio = 0.9f)
        {
            //Contianer
            //-TextArea
            //--Text
            //--Placeholder


            float baseFontSize = 30f;
            Vector2 textSizeDelta = new Vector2(30 * size.x / size.y, baseFontSize);

            GameObject container = new GameObject(name, typeof(RectTransform), typeof(SpriteRenderer), typeof(TMP_InputField));
            container.transform.SetParent(parent);
            (container.transform as RectTransform).sizeDelta = size;
            // Textbox image. Console settings (but not masked for round edges)
            //var _sprite = container.GetComponent<SpriteRenderer>();
            //_sprite.color = new Color(0, 0, 0, 0.7f);
            container.SetActive(false);

            // Addendum to create reasonably sized caret
            GameObject textarea = new GameObject("Text Area", new Type[] { typeof(RectTransform), typeof(Image) });
            (textarea.transform as RectTransform).sizeDelta = textSizeDelta;
            textarea.transform.SetParent(container.transform);
            var _image = textarea.GetComponent<Image>();
            _image.color = new Color(0, 0, 0, 0);

            GameObject textContainer = new GameObject("Text", typeof(RectTransform), typeof(TextMeshPro));
            textContainer.transform.SetParent(textarea.transform);
            (textContainer.transform as RectTransform).sizeDelta = textSizeDelta;
            var _text = textContainer.GetComponent<TextMeshPro>();
            _text.color = Color.white;
            _text.fontSize = baseFontSize - 1;
            _text.richText = false;
            _text.overflowMode = TextOverflowModes.Ellipsis; // Change as you like
            _text.alignment = TextAlignmentOptions.Left;

            GameObject placeholderContainer = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshPro));
            placeholderContainer.transform.SetParent(textarea.transform);
            (placeholderContainer.transform as RectTransform).sizeDelta = textSizeDelta;

            var _placeholder = placeholderContainer.GetComponent<TextMeshPro>();
            _placeholder.text = defaultText;
            _placeholder.color = Color.gray;
            _placeholder.fontSize = baseFontSize - 1; //This makes the character:caret ratio 30:3. If you want to change the 
            _placeholder.richText = false;
            _placeholder.overflowMode = TextOverflowModes.Ellipsis; // Change as you like
            _placeholder.alignment = TextAlignmentOptions.Left;

            // Need to set the TextViewport for where text should be visible
            // ..it should be a dedicated TextArea, but we lazy
            var _inputField = container.GetComponent<TMP_InputField>();
            _inputField.textViewport = textarea.transform as RectTransform;
            _inputField.textComponent = _text;
            _inputField.placeholder = _placeholder; // Default to placeholder when no text is inputted
            //_inputField.targetGraphic = _sprite; // Not sure what this is
            // Michael: targetGraphic is from the selectable class. I assume this can be used to get things to be highlighted. (Not used by Input_Fields inherently)

            _inputField.caretWidth = 3; //caretWidth is an integer, sadly :(
                                        //Small trick to spawn the caret
                                        //_inputField.enabled = false;
                                        //_inputField.enabled = true;

            textarea.transform.localScale = ((fontRatio * size.y / baseFontSize) * new Vector3(1, 1, 0)) + new Vector3(0, 0, 1);
            container.transform.localPosition = position;
            container.SetActive(true);

            return _inputField;
        }

    }
}
