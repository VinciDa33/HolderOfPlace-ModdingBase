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
    public class CommandLine : MonoBehaviour
    {
        public static CommandLine main;
        public static GameObject inputFieldHolder;
        public static TMP_InputField inputField;
        public static TextMeshPro autoComplete;

        public static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public static int blocking = 0;

        public bool LineActive => inputFieldHolder != null && inputFieldHolder.activeInHierarchy;

        public List<string> autocompletes = new List<string>();

        public static void CreateCommandLineHolder()
        {
           Command.LoadInitCommands(commands);

            GameObject obj = new GameObject("CommandLineHolder");
            main = obj.AddComponent<CommandLine>();
            DontDestroyOnLoad(obj);
            main.CreateInputField();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Tilde))
            {
                ToggleInputField();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (autocompletes != null && autocompletes.Count > 0)
                {
                    string[] currentText = inputField.textComponent.text.Split(' ');
                    currentText[currentText.Length - 1] = autocompletes[0];
                    string text = string.Join(" ", currentText);
                    inputField.SetTextWithoutNotify(text);
                    inputField.MoveToEndOfLine(false, false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (autocompletes != null && autocompletes.Count > 1)
                {
                    string s = autocompletes[0];
                    autocompletes.RemoveAt(0);
                    autocompletes.Add(s);
                    ProduceAutocomplete();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (autocompletes != null && autocompletes.Count > 1)
                {
                    string s = autocompletes[autocompletes.Count-1];
                    autocompletes.RemoveAt(autocompletes.Count - 1);
                    autocompletes.Insert(0,s);
                    ProduceAutocomplete();
                }
            }
        }

        public void ToggleInputField()
        {
            System.Console.WriteLine("Input Field Toggled");
            if (inputField == null)
            {
                CreateInputField();
            }

            if (LineActive)
            {
                DisableInputField();
            }
            else
            {
                blocking++;
                BootstrapMain.inputBlock++;
                Transform t = FindAnyObjectByType<CodexControl>()?.transform
                    ?? FindAnyObjectByType<CombatControl>()?.transform
                    ?? FindAnyObjectByType<TitleScreenControl>()?.transform
                    ?? FindAnyObjectByType<BackgroundControl>()?.transform;
                inputFieldHolder.transform.SetParent(t);
                inputFieldHolder.transform.position = new Vector3(0, 0, -8);
                inputFieldHolder.SetActive(true);

                autocompletes.Clear();
                ProduceAutocomplete();

                inputField.ActivateInputField();
                inputField.Select();
                inputField.SetTextWithoutNotify("");
            }
        }

        public void DisableInputField()
        {
            if (inputFieldHolder != null)
            {
                
                inputFieldHolder.SetActive(false);
                inputFieldHolder.transform.SetParent(transform);
            }
            BootstrapMain.inputBlock -= blocking;
            blocking = 0;
        }

        public void CreateInputField()
        {
            inputFieldHolder = new GameObject("InputFieldHolder");
            inputFieldHolder.SetActive(false);
            SpriteRenderer sr = UIFactory.Box(inputFieldHolder.transform, Vector3.zero, new Color(0, 0, 0, 0.5f));
            sr.transform.localScale = new Vector3(250, 150, 1);
            inputField = UIFactory.NewInputField("InputField", inputFieldHolder.transform, Vector3.zero, new Vector2(35, 6), "Type something!");
            inputField.transform.localScale = new Vector3(4, 4, 1);
            inputField.onSubmit.AddListener(IssueCommand);
            inputField.onDeselect.AddListener(Deselect);
            inputField.onValueChanged.AddListener(OnValueChanged);

            autoComplete = UIFactory.Text(inputFieldHolder.transform, new Vector3(0, -5, 0), "", 20);
            autoComplete.rectTransform.sizeDelta = new Vector2(125, 5);
            autoComplete.alignment = TextAlignmentOptions.TopLeft;
        }

        public void OnValueChanged(string s)
        {
            string[] parts = s.Split(' ');
            if (parts.Length == 1)
            {
                autocompletes = commands.Keys.Where(k => k.StartsWith(parts[0])).ToList();
            }
            else if (parts.Length > 1)
            {
                if (commands.ContainsKey(parts[0].ToLower()))
                {
                    autocompletes = commands[parts[0].ToLower()].OnValueChanged(parts.Skip(1).ToArray());
                    
                }
            }
            ProduceAutocomplete();
        }

        public void ProduceAutocomplete()
        {
            if (autocompletes == null)
            {
                autocompletes = new List<string>();
            }
            if (autocompletes.Count == 0)
            {
                autoComplete.SetText("");
                return;
            }

            string s = "";
            for(int i=0; i<autocompletes.Count-1; i++)
            {
                s += autocompletes[i] + "\n";
            }
            s += autocompletes[autocompletes.Count - 1];
            autoComplete.SetText(s);
        }

        public void Deselect(string s)
        {
            if (LineActive)
            {
                inputField.ActivateInputField();
                inputField.Select();
            }
            else
            {
                DisableInputField();
            }
        }

        public void IssueCommand(string command)
        {
            inputField.SetTextWithoutNotify("");
            autocompletes.Clear();
            ProduceAutocomplete();
            System.Console.WriteLine("[Command] " + command);
            string[] parts = command.ToLower().Split(' ');

            if (parts.Length == 0 || !commands.ContainsKey(parts[0].ToLower()))
            {
                return;
            }

            commands[parts[0].ToLower()].Run(parts.Skip(1).ToList());

            inputField.ActivateInputField();
            inputField.Select();

            /*
            if (parts.Length > 1 && parts[0] == "recruit" && RecruitPanel.Main != null)
            {
                string key = null;
                if (parts[1].Length > 0 && parts[1][0] == '@')
                {
                    for(int i=0; i<Library.Main.CardPrefabs.Count; i++)
                    {
                        Card card = Library.Main.CardPrefabs[i].GetComponent<Card>();

                        if (card.Info.RealName.ToLower().Replace(" ", "") == parts[1].Substring(1).ToLower())
                        {
                            key = Library.Main.Keys[i];
                            break;
                        }
                    }
                }
                else
                {
                    key = Library.Main.Keys.FirstOrDefault(s => s.ToLower() == parts[1]);
                }
                if (key != null)
                {
                    RecruitPanel.Main.DirectRecruit(key, true);
                }
            }
            else if (parts.Length > 1 && parts[0] == "map" && Library.Main!= null)
            {
                string key = null;
                if (parts[1].Length > 0 && parts[1][0] == '@')
                {
                    for (int i = 0; i < Library.Main.CardPrefabs.Count; i++)
                    {
                        Card card = Library.Main.CardPrefabs[i].GetComponent<Card>();

                        if (card.Info.RealName.ToLower().Replace(" ", "") == parts[1].Substring(1).ToLower())
                        {
                            key = Library.Main.Keys[i];
                            break;
                        }
                    }
                }
                else
                {
                    key = Library.Main.Keys.FirstOrDefault(s => s.ToLower() == parts[1]);
                }
                if (key != null)
                {
                    LibraryExt.MapCard(key, true);
                }
            }
            else if (parts[0] == "pray" && CombatControl.Main != null)
            {
                float amount = 1;
                if (parts.Length > 1 && !float.TryParse(parts[1], out amount))
                {
                    amount = 1;
                }
                CombatControl.Main.ChangeFate(amount);
            }
            else if (parts[0] == "reroll" && RecruitPanel.Main != null)
            {
                List<string> validKeys = new List<string>();
                for (int i = 1; i < parts.Length; i++)
                {
                    System.Console.WriteLine("word: " + parts[i]);
                    string key = Library.Main.Keys.FirstOrDefault(s => s.ToLower() == parts[i]);
                    if (key != null)
                    {
                        validKeys.Add(key);
                        System.Console.WriteLine("Key Added: " + key);
                    }
                }
                RecruitPanel.Main.ResetSlot(IgnoreLock: false);
                if (validKeys.Count > 0)
                {
                    RecruitPanel.Main.RecruitOverride = validKeys;
                    System.Console.WriteLine("Recruit Overrided: " + RecruitPanel.Main.RecruitOverride.Count);
                }
                RecruitPanel.Main.NewRecruitProcess(CanSkip: false);
            }
            else if (parts[0] == "life" && UIControl.Main?.SelectingCard != null && parts.Length > 1)
            {
                if (!float.TryParse(parts[1], out float amount))
                {
                    return;
                }
                if (amount <= 0)
                {
                    amount = 1;
                }
                UIControl.Main.SelectingCard.SetLife(amount);
                UIControl.Main.SelectingCard.OriLife = amount;
                UIControl.Main.SelectingCard.Life = amount;
            }
            else if (parts[0] == "damage" && UIControl.Main?.SelectingCard != null && parts.Length > 1)
            {
                if (!float.TryParse(parts[1], out float amount))
                {
                    return;
                }
                if (amount < 0)
                {
                    amount = 0;
                }
                UIControl.Main.SelectingCard.SetBaseDamage(amount);
                UIControl.Main.SelectingCard.OriDamage = amount;
                UIControl.Main.SelectingCard.BaseDamage = amount;
            }
            
            inputField.ActivateInputField();
            inputField.Select();
            */
        }
            
    }
}
