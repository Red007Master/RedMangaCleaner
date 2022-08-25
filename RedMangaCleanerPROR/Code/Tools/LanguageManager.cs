using RedsTools.Utility.WPF.LanguageManager.Components;
using RedsTools.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace RedsTools.Utility.WPF.LanguageManager
{
    public class LanguageManager
    {
        public string LanguageCorePath { get; set; }
        public string CurrentLanguageFullName { get; set; }
        private int CurrentLanguageIndex { get; set; }

        public List<Language> Languages = new List<Language>();

        public LanguageManager(string languageCorePath, string currentLanguage)
        {
            LanguageCorePath = languageCorePath;
            Language.LanguageCorePath = languageCorePath;
            CurrentLanguageFullName = currentLanguage;

            Initialization();
            SetLanguageTo(CurrentLanguageFullName);
        }
        public void Initialization()
        {
            string[] languageFiles = Directory.GetFiles(LanguageCorePath);

            if (languageFiles.Length > 0)
            {
                for (int i = 0; i < languageFiles.Length; i++)
                {
                    Languages.Add(new Language(languageFiles[i]));
                }
            }
            else
            {
                Languages.Add(Language.GetDebug());
                Languages[0].Save();
            }
        }

        public void SetLanguageTo(string inputLanguageFullName)
        {
            P.Logger.Log($"Trying SetLanguageTo({inputLanguageFullName})", LogLevel.Information, 1);

            CurrentLanguageFullName = inputLanguageFullName;
            CurrentLanguageIndex = Languages.FindIndex(x => x.FullName == CurrentLanguageFullName);

            if (CurrentLanguageIndex >= 0)
            {
                Languages[CurrentLanguageIndex].Load(LoadSettings.Full);
                P.Logger.Log($"Success language found and set with index=[{CurrentLanguageIndex}]", LogLevel.Information, 2);
            }
            else
            {
                P.Logger.Log($"Language don't found try to set to DEFAULT", LogLevel.Warning, 2);

                int index = Languages.FindIndex(x => x.FullName == Language.GetDebug().FullName);
                if (index >= 0)
                {
                    CurrentLanguageIndex = index;
                    Languages[CurrentLanguageIndex].Load(LoadSettings.Full);
                    P.Logger.Log($"DEFAULT as [{Language.GetDebug().FullName}] found and set on index=[{index}]", LogLevel.Warning, 2);
                }
                else
                {
                    P.Logger.Log($"DEFAULT as [{Language.GetDebug().FullName}] don't found, creating and savind...", LogLevel.Warning, 2);
                    Languages.Add(Language.GetDebug());
                    Languages[Languages.Count - 1].Save();
                    P.Logger.Log($"DEFAULT as [{Language.GetDebug().FullName}] created and saved, starting recursive call...", LogLevel.Warning, 3);

                    SetLanguageTo(Language.GetDebug().FullName);
                }
            }
        }
        public void Localize(object coreClass, object target)
        {
            List<Visual> visuals = WPFTypes.GetChildrens((Visual)target, true);

            for (int i = 0; i < visuals.Count; i++)
            {
                Type controlType = visuals[i].GetType();
                Control control = visuals[i] as Control;

                if (control != null && control.Name.Contains("Localize"))
                {
                    RedsControl redsControl = new RedsControl(control.Name, controlType, coreClass.GetType());
                    ContainsResult containsResult = Languages[CurrentLanguageIndex].Contains(redsControl);
                    if (containsResult.Contains)
                    {
                        PropertyInfo propertyInfo = controlType.GetProperty(containsResult.RedsControl.NameOfLocalizationTargetField);
                        propertyInfo.SetValue(control, containsResult.RedsControl.Text);
                    }
                    else
                    {
                        Languages[CurrentLanguageIndex].Add(redsControl);
                    }
                }
            }
        }
    }

    namespace Components
    {
        public class RedsControl
        {
            private static List<string> Fields { get; set; } = new List<string>();
            private static List<string> ParentTypes { get; set; } = new List<string>();
            private static List<string> Types { get; set; } = new List<string>();

            private int ParentTypeId { get; set; }
            private int TypeId { get; set; }
            private int NameOfLocalizationTargetFieldId { get; set; }

            public string Name { get; set; }
            public string Text { get; set; }
            public string ParentType
            {
                get
                {
                    return ParentTypes[ParentTypeId];
                }

                set
                {
                    ParentTypeId = GetIdAndOrAdd(value, ParentTypes);
                }
            }
            public string Type
            {
                get
                {
                    return Types[TypeId];
                }

                set
                {
                    TypeId = GetIdAndOrAdd(value, Types);
                }
            }
            public string NameOfLocalizationTargetField
            {
                get
                {
                    return Fields[NameOfLocalizationTargetFieldId];
                }

                set
                {
                    NameOfLocalizationTargetFieldId = GetIdAndOrAdd(value, Fields);
                }
            }
            public string WPFName
            {
                get
                {
                    return Name + "_Localize_" + Type;
                }
            }
            public string SaveName
            {
                get
                {
                    return ParentType + "." + Type + "." + Name + "." + NameOfLocalizationTargetField;
                }
            }
            public string CompareString
            {
                get
                {
                    return $"{ParentTypeId}{TypeId}{NameOfLocalizationTargetFieldId}{Name}";
                }
            }

            public int GetIdAndOrAdd(string iName, List<string> iTargetList)
            {
                int index = 0;
                bool isFound = false;

                for (int i = 0; i < iTargetList.Count; i++)
                {
                    if (iTargetList[i] == iName)
                    {
                        isFound = true;
                        index = i;
                        break;
                    }
                }

                if (isFound)
                {
                    return index;
                }
                else
                {
                    iTargetList.Add(iName);
                    return iTargetList.Count - 1;
                }
            }

            public static bool operator ==(RedsControl rc1, RedsControl rc2)
            {
                if (rc1.CompareString == rc2.CompareString)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool operator !=(RedsControl rc1, RedsControl rc2)
            {
                if (rc1.SaveName == rc2.SaveName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public RedsControl(string controlAsString, Type controlType, Type parentType)
            {
                string[] controlNameParts = controlAsString.Split('_');

                Name = controlNameParts[0];
                Text = controlNameParts[0];
                ParentType = parentType.Name;
                Type = controlType.Name;

                NameOfLocalizationTargetField = controlNameParts[2];
            }
            public RedsControl(string saveString)
            {
                string[] splitedToTextAndCore = saveString.Split('=');
                string[] splitedCore = splitedToTextAndCore[0].Split('.');

                Text = splitedToTextAndCore[1];

                ParentType = splitedCore[0];
                Type = splitedCore[1];
                Name = splitedCore[2];
                NameOfLocalizationTargetField = splitedCore[3];
            }
        }
        public class Language
        {
            public static string LanguageCorePath { get; set; }

            public List<RedsControl> RedsControls = new List<RedsControl>();

            public string FullName { get; set; }
            public string KeyName { get; set; }
            public string Credit { get; set; }
            public string FileName { get; set; }

            public void Save()
            {
                using (StreamWriter sw = new StreamWriter(LanguageCorePath + @"\" + FileName, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine("FullName=" + FullName);
                    sw.WriteLine("KeyName=" + KeyName);
                    sw.WriteLine("Credit=" + Credit);

                    if (RedsControls.Count > 0)
                    {
                        for (int i = 0; i < RedsControls.Count; i++)
                        {
                            sw.WriteLine(RedsControls[i].SaveName + "=" + RedsControls[i].Text);
                        }
                    }
                }
            }
            public void Load(LoadSettings loadSettings)
            {
                using (StreamReader sr = new StreamReader(LanguageCorePath + @"\" + FileName, System.Text.Encoding.Default))
                {
                    FullName = sr.ReadLine().Split('=')[1];
                    KeyName = sr.ReadLine().Split('=')[1];
                    Credit = sr.ReadLine().Split('=')[1];

                    if (loadSettings == LoadSettings.Full)
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            RedsControls.Add(new RedsControl(line));
                        }
                    }
                }
            }

            public void Add(RedsControl iRedsControl)
            {
                RedsControls.Add(iRedsControl);

                using (StreamWriter sw = new StreamWriter(LanguageCorePath + @"\" + FileName, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(iRedsControl.SaveName + "=" + iRedsControl.Text);
                }
            }
            public ContainsResult Contains(RedsControl iRedsControl)
            {
                ContainsResult result = new ContainsResult();

                for (int i = 0; i < RedsControls.Count; i++)
                {
                    if (iRedsControl == RedsControls[i])
                    {
                        result.Contains = true;
                        result.Index = i;
                        result.RedsControl = RedsControls[i];
                        break;
                    }
                }

                return result;
            }

            public Language(string languageFilePath)
            {
                FileName = Path.GetFileName(languageFilePath);

                Load(LoadSettings.Partial);
            }
            public Language()
            {

            }

            public static Language GetDebug()
            {
                Language language = new Language();

                language.FullName = "EnglishDebug";
                language.KeyName = "ENGDEB";
                language.Credit = "Debug Language Example File";
                language.FileName = "EnglishDebug.txt";

                return language;
            }
        }

        public class ContainsResult
        {
            public bool Contains { get; set; }
            public int Index { get; set; }
            public RedsControl RedsControl { get; set; }

            public ContainsResult()
            {
                Contains = false;
            }
        }

        public enum LoadSettings
        {
            Full = 0,
            Partial = 1,
        }
    }
}
