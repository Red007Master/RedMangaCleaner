using RedsSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

public class Settings
{
    public string FilePath { get; set; }
    public int LoopCounter { get; set; } = 0;

    public SettingsList SettingsList { get; set; }
    private IDictionary<string, ISetting> SettingsDictionary = new Dictionary<string, ISetting>();

    public void ReadSettings()
    {
        if (File.Exists(FilePath))
        {
            P.Logger.Log($"SettingsFile detected, path=[{FilePath}]", LogLevel.Debug, 2);

            string[] fileContentArray = File.ReadAllLines(FilePath);
            string[] cuted1, cuted2;

            for (int i = 0; i < fileContentArray.Length; i++)
            {
                if (fileContentArray[i].Contains("|"))
                {
                    fileContentArray[i] = fileContentArray[i].Replace("[", "");
                    fileContentArray[i] = fileContentArray[i].Replace("]", "");
                    cuted1 = fileContentArray[i].Split('|');
                    cuted2 = fileContentArray[i + 1].Split('=');

                    P.Logger.Log($"Try apply [{cuted2[1]}] to [{cuted1[0]}]", LogLevel.Debug, 3);
                    try
                    {
                        SettingsDictionary[cuted1[0]].SetFromString(cuted2[1]);
                        P.Logger.Log($"[{cuted1[0]}] applyed with value = [{cuted2[1]}]", LogLevel.Debug, 4);
                    }
                    catch (System.Exception ex)
                    { P.Logger.Log($"[{cuted1[0]}] don't applyed with value = [{cuted2[1]}] ex=[{ex}]", LogLevel.Error); }
                }
            }
        }
        else
        {
            if (LoopCounter > 5)
                P.Logger.Log("RecursiveLoop", LogLevel.FatalError);

            LoopCounter++;

            P.Logger.Log($"ReadSettings: Error SettingsFile don't detected, path=[{FilePath}], applying DefaultSettings, WriteSettings: Try", LogLevel.Warning, 1);

            using (TimeLogger tl = new TimeLogger("ApplySettingsList", LogLevel.Information, P.Logger))
            {
                FromDictionaryToClass();
            }

            using (TimeLogger tl = new TimeLogger("WriteSettings", LogLevel.Information, P.Logger))
            {
                WriteSettings();
            }

            P.Logger.Log($"ReadSettings: Error SettingsFile don't detected, path=[{FilePath}], WriteSettings: Success, Initiating Recursive Call of ReadSettings", LogLevel.Debug);

            ReadSettings();
        }
    }
    public void WriteSettings()
    {
        FromClassToDictionary();

        try
        {
            File.Delete(FilePath);
        }
        catch (Exception)
        { } //TODO

        using (StreamWriter sw = new StreamWriter(FilePath, true, System.Text.Encoding.Default))
        {
            for (int i = 0; i < SettingsDictionary.Count; i++)
            {
                var kayValueBuffer = SettingsDictionary.ElementAt(i);
                SettingStr settingStrBuffer = kayValueBuffer.Value.GetSettingStr();

                sw.WriteLine($"[{settingStrBuffer.Name}|{settingStrBuffer.Description}]");
                sw.WriteLine($"{settingStrBuffer.Name}={settingStrBuffer.Value}\n");
            }
        }
    }

    private void GetDefaultSettingsList()
    {
        AddSetting<bool>(true, "UseYoloV5ObjectRecognition", "Use Object Recog For Detecting textboxes");
        AddSetting<bool>(true, "OutputBlackAndWhiteImages", "Output BaW images as result, use 'true' if you work on BaW images");
        AddSetting<bool>(true, "ConductObjectDetectionOnBlackAndWhiteVariants", @"'true' if you use it on BaW images or have model trained on BaW images, if you use default model you can check mode here 'RdmMangaCleaner\Other\About.txt'");
        AddSetting<bool>(true, "ConductTextBoxFillingOnBlackAndWhiteVariants", "Conduct TextBox filling on BaW variants of images (faster)");

        AddSetting<string>("English", "Language", "Language acording to Language.txt file TODO");

        AddSetting<int>(3, "ProcessingBufferSize", @"Amount of backups of your work in 'RdmMangaCleaner\Processing'");

        AddSetting<string>("redsManga001.json", "YoloConfig", "Used by default YoloConfig");
        AddSetting<string>("redsManga001.onnx", "YoloWeights", "Used by default YoloWeights");

        AddSetting<byte>(245, "DefaultFillTargetColorBaW", "Color(Grayscale) used as target for filing (text background), 255=white, 0=black"); //TODO max byte value is 255 add check

        AddSetting<int>(2, "PrecompileRedImageFullsThreadsCount", "Count of threads used to PrecompileRedImageFulls (getting byte[,]s and bitmaps in CGUI)");
        AddSetting<int>(5, "ImagesToBlackAndWhiteThreadsCount", "Count of threads used to Convert images to BaW"); //TODO add perf info
    } //Dev set

    private void AddSetting<Tinput>(Tinput inputValue, string inputName, string inputDescription)
    {
        Setting<Tinput> buffer = new Setting<Tinput>(inputValue, inputName, inputDescription);
        SettingsDictionary.Add(buffer.Name, buffer);
    }

    private void FromClassToDictionary()
    {
        foreach (PropertyInfo propertyInfo in SettingsList.GetType().GetProperties())
        {
            var value = propertyInfo.GetValue(SettingsList);
            string name = propertyInfo.Name;

            SettingsDictionary[name].SetFromObject(value);
        }
    }
    private void FromDictionaryToClass()
    {
        foreach (KeyValuePair<string, ISetting> item in SettingsDictionary)
        {
            ISetting current = item.Value;

            var property = typeof(SettingsList).GetProperty(current.GetName());
            property.SetValue(SettingsList, current.GetValue(), null);
        }
    }

    public void ConsoleOutputSettings()
    {
        foreach (var setting in SettingsDictionary)
        {
            ISetting settingBuffer = setting.Value;
            SettingStr settingStr = settingBuffer.GetSettingStr();

            Console.WriteLine($"Name=[{settingStr.Name}][{settingStr.Value}]");
        }
        Console.WriteLine("\n");
    }

    public Settings(string filePath)
    {
        FilePath = filePath;

        SettingsList = new SettingsList();

        using (TimeLogger tl = new TimeLogger("GetDefaultSettingsList", LogLevel.Information, P.Logger, 1))
        {
            GetDefaultSettingsList();
        }

        using (TimeLogger tl = new TimeLogger("ReadSettings", LogLevel.Information, P.Logger, 1))
        {
            ReadSettings();
        }

        using (TimeLogger tl = new TimeLogger("FromDictionaryToClass", LogLevel.Information, P.Logger, 1))
        {
            FromDictionaryToClass();
        }
    }
}

namespace RedsSettings
{
    public class SettingsList
    {
        public bool UseYoloV5ObjectRecognition { get; internal set; }
        public bool OutputBlackAndWhiteImages { get; internal set; }
        public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; internal set; }
        public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; internal set; }

        public string Language { get; internal set; }

        public int ProcessingBufferSize { get; internal set; }

        public string YoloWeights { get; internal set; }
        public string YoloConfig { get; internal set; }

        public byte DefaultFillTargetColorBaW { get; internal set; }

        public int PrecompileRedImageFullsThreadsCount { get; internal set; }
        public int ImagesToBlackAndWhiteThreadsCount { get; internal set; }
    }

    public interface ISetting
    {
        SettingStr GetSettingStr();
        void SetFromString(string inputStrValue);
        void SetFromObject(object inputObject);
        string GetName();
        object GetValue();
    }

    internal class Setting<TValue> : ISetting
    {
        public TValue Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void SetFromString(string inputStrValue)
        {
            Value = TConverter.ChangeType<TValue>(inputStrValue);
        }
        public void SetFromObject(object inputObject)
        {
            Value = (TValue)inputObject;
        }

        public object GetValue()
        {
            return Value;
        }
        public string GetName()
        {
            return Name;
        }

        public Setting(TValue value, string name, string description)
        {
            Value = value;
            Name = name;
            Description = description;
        }
        public SettingStr GetSettingStr()
        {
            SettingStr result = new SettingStr();

            result.Value = Value.ToString();
            result.Name = this.Name;
            result.Description = this.Description;

            return result;
        }
    }

    public class SettingStr
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public static class TConverter
    {
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(typeof(T), value);
        }

        public static object ChangeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }

        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }
}