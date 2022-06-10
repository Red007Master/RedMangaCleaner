using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

static class Web
{
    public static void NetworkCheck()
    {
        string openKey = "DataStrOPNTTT";
        string closeKey = "TTTDataStrCLS";
        string password = "m38hnnnZiEWQGYiSHpnu9QHGrtfrwlRvCj4c6Gck";

        string[] targetSites = GetTargetSites();
        string[] readedSitesData = ReadAllSitesData(targetSites);
        string[] encryptedPotentialPayloads = GetAllDataBetweenKeys(openKey, closeKey, readedSitesData);
        string[] decryptedPotentialPayloads = DecryptPotentialPayloads(encryptedPotentialPayloads, password);
        List<WebData> allRetrivedNetworkDatas = DeserializeAllPotentialPayloads(decryptedPotentialPayloads);
        WebData newestNetworkData = GetNewestNetworkData(allRetrivedNetworkDatas);

        NewestNetworkData = newestNetworkData;

        var SC = new SecurityController(); //TODO soft
        WebData ndOutput = GetND();
        string serializedObject = JsonConvert.SerializeObject(ndOutput);
        string encryptedData = SC.Encrypt(password, serializedObject);
        encryptedData = openKey + encryptedData + closeKey;
        File.WriteAllText(@"D:\data.txt", encryptedData);

        Console.WriteLine();
    }

    private static string[] GetTargetSites()
    {
        List<string> result = new List<string>();

        result.Add("https://oztbarj5vwpidxtffiasmp4xoqklh0.blogspot.com/2022/01/blog-post.html");
        result.Add("https://pastebin.com/ZzW4ftNr");

        return result.ToArray();
    }
    private static string[] ReadAllSitesData(string[] targetSites)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < targetSites.Length; i++)
        {
            try
            {
                string buffer = ReadTargetSiteData(targetSites[i]);

                if (buffer != null)
                {
                    result.Add(buffer);
                }
            }
            catch (Exception)
            {
            }
        }

        if (result.Count != 0)
        {
            return result.ToArray();
        }
        else
        {
            return null;
        }
    }
    private static string[] GetAllDataBetweenKeys(string inputOpenKey, string inputCloseKey, string[] inputReadedSitesData)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < inputReadedSitesData.Length; i++)
        {
            string[] buffer = GetDataBetweenKeys(inputOpenKey, inputCloseKey, inputReadedSitesData[i]);

            for (int j = 0; j < buffer.Length; j++)
                result.Add(buffer[j]);
        }

        return result.ToArray();
    }
    private static string[] DecryptPotentialPayloads(string[] inputEncryptedPotentialPayloads, string inputKey)
    {
        List<string> result = new List<string>();
        SecurityController sc = new SecurityController();

        for (int i = 0; i < inputEncryptedPotentialPayloads.Length; i++)
        {
            try
            {
                string buffer = sc.Decrypt(inputKey, inputEncryptedPotentialPayloads[i]);

                if (buffer != null)
                    result.Add(buffer);
            }
            catch (Exception)
            {
            }
        }

        if (result.Count != 0)
            return result.ToArray();
        else
            return null;
    }
    private static List<WebData> DeserializeAllPotentialPayloads(string[] inputDecryptedPotentialPayloads)
    {
        List<WebData> result = new List<WebData>();

        for (int i = 0; i < inputDecryptedPotentialPayloads.Length; i++)
        {
            try
            {
                WebData buffer = JsonConvert.DeserializeObject<WebData>(inputDecryptedPotentialPayloads[i]);

                if (buffer != null)
                    result.Add(buffer);
            }
            catch (Exception)
            { }
        }

        if (result.Count != 0)
            return result;
        else
        {
            return null;
        }
    }
    private static WebData GetNewestNetworkData(List<WebData> inputAllRetrivedNetworkDatas)
    {
        WebData result = new WebData();

        result = inputAllRetrivedNetworkDatas[0];
        for (int i = 0; i < inputAllRetrivedNetworkDatas.Count; i++)
        {
            if (result.NetworkDataId < inputAllRetrivedNetworkDatas[i].NetworkDataId)
                result = inputAllRetrivedNetworkDatas[i];
        }

        return result;
    }

    private static string[] GetDataBetweenKeys(string inputOpenKey, string inputCloseKey, string inputData)
    {
        List<string> result = new List<string>();

        bool openKeyDetected = false;
        bool closeKeyDetected = false;
        int lastOpenKeyStart = 0, lastOpenKeyEnd = 0;
        int lastCloseKeyStart = 0, lastCloseKeyEnd = 0;

        for (int i = 0; i < inputData.Length; i++)
        {
            for (int j = 0; j < inputOpenKey.Length; j++)
            {
                if (inputData[i + j] == inputOpenKey[j])
                {
                    if (j >= inputOpenKey.Length - 1)
                    {
                        lastOpenKeyEnd = i + j + 1;
                        lastOpenKeyStart = (i + j) - inputOpenKey.Length + 1;
                        openKeyDetected = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            for (int j = 0; j < inputCloseKey.Length; j++)
            {
                if (inputData[i + j] == inputCloseKey[j])
                {
                    if (j >= inputCloseKey.Length - 1)
                    {
                        lastCloseKeyEnd = i + j;
                        lastCloseKeyStart = (i + j) - inputCloseKey.Length + 1;
                        closeKeyDetected = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            if (openKeyDetected == true && closeKeyDetected == true)
            {
                openKeyDetected = false;
                closeKeyDetected = false;

                string buffer = "";
                for (int j = lastOpenKeyEnd; j < lastCloseKeyStart; j++)
                {
                    buffer += inputData[j];
                }
                result.Add(buffer);
            }
        }

        return result.ToArray();
    }
    private static string ReadTargetSiteData(string inputSiteUrl)
    {
        using (WebClient webClient = new WebClient())
        {
            string webData = webClient.DownloadString(inputSiteUrl);
            return webData;
        }
    }

    private static WebData GetND()
    {
        var ND = new WebData
        {
            IsDefault = false,
            NetworkDataId = 969,

            LastVersionName = "First",
            LastVersionId = 5,
            LastVersionDownloadUrl = "https://www.youtube.com/watch?v=I_izvAbhExY",
            UpdateMessage = new Dictionary<string, LocalizedString>(),
            UpdateImageUrl = new Dictionary<string, LocalizedString>(),

            SocialLinks = new List<Link>(),
            DonationLinks = new List<Link>(),
            HelpfulLinks = new List<Link>(),

            News = new Dictionary<string, LocalizedList>(),
            NewsId = 69,
        };

        ND.UpdateMessage.Add("ENG", new LocalizedString("ENG", "Engrish text old Update 1-Ass\n2-Pidors\n3-gaySex and more"));
        ND.UpdateMessage.Add("RUS", new LocalizedString("RUS", "Сталин любов геи прочий сброс советьский союз аниме 1-ти пидр\n2-гей\n3-ХТО!?\n4-Я!?"));

        ND.UpdateImageUrl.Add("ENG", new LocalizedString("ENG", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ae/Flag_of_the_United_Kingdom.svg/1280px-Flag_of_the_United_Kingdom.svg.png"));
        ND.UpdateImageUrl.Add("RUS", new LocalizedString("RUS", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Flag_of_the_Soviet_Union.svg/800px-Flag_of_the_Soviet_Union.svg.png"));

        ND.SocialLinks.Add(new Link("Discord", "https://discord.gg/WnKtHMEhYY"));
        ND.SocialLinks.Add(new Link("Email", "RedMangaCleaner@gmail.com"));
        ND.SocialLinks.Add(new Link("Steam", "Red007Master"));

        ND.DonationLinks.Add(new Link("PayPal", "EMPTY"));
        ND.DonationLinks.Add(new Link("Webmoney", "EMPTY"));
        ND.DonationLinks.Add(new Link("CARD?", "EMPTY"));

        ND.HelpfulLinks.Add(new Link("Alturos.Yolo-Github", "https://github.com/AlturosDestinations/Alturos.Yolo"));
        ND.HelpfulLinks.Add(new Link("RedMangaCleaner-YouTube", "EMPTY"));
        ND.HelpfulLinks.Add(new Link("Other", "EMPTY"));

        ND.News.Add("ENG", new LocalizedList("ENG", new List<string> { "New features ass dwdqwdqwdqwdqwdqwdqwdwfgwfwe konec", " state of the art real-time object detection system for C# (Visual Studio). This project has CPU and GPU support, with GPU the detection works much faster. The primary goal of this project is an easy use of yolo, this package is available on nuget and you must only install two packages to start detection. In the background we are use the Windows Yolo", " Pre-Trained Dataset contains the Informations about the recognizable objects. A higher Processing Resolution detects object also if they are smaller but this increases the processing time. The Alturos.YoloV2TinyVocData package is the same as YOLOv2-tiny. You " }));
        ND.News.Add("RUS", new LocalizedList("ENG", new List<string> { "required by yolo_cpp_dll_xxx (process image as byte data detect_mat", "required by yolo_cpp_dll_xxx (POSIX Threads", "required by yolo_cpp_dll_xxx (POSIX Threads" }));

        return ND;
    }

    public static WebData NewestNetworkData { get; set; }
}

class WebData
{
    public bool IsDefault { get; set; }

    public int NetworkDataId { get; set; }

    public string LastVersionName { get; set; }
    public int LastVersionId { get; set; }
    public string LastVersionDownloadUrl { get; set; }
    public Dictionary<string, LocalizedString> UpdateMessage { get; set; }
    public Dictionary<string, LocalizedString> UpdateImageUrl { get; set; }
    public LocalizedString DevNotes { get; set; }

    public List<Link> SocialLinks { get; set; }
    public List<Link> DonationLinks { get; set; }
    public List<Link> HelpfulLinks { get; set; }

    public Dictionary<string, LocalizedList> News { get; set; }
    public int NewsId { get; set; }

    public string AboutContent { get; set; }
    public string KnownBugs { get; set; }
}
