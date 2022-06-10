using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedsTools
{
    namespace Files
    {
        public static class Files
        {
            public static string[] CollectFileArrayFromDir(string targetDir, string filter, bool callout)
            {
                string[] finalArrayFiles = new string[0];
                string[] buffer1 = new string[1];
                string[] buffer2 = new string[0];

                buffer1[0] = targetDir;

                DateTime start = DateTime.Now;
                if (callout)
                    Console.WriteLine($"Start collecting files array, time = [{start}]");

                while (true)
                {
                    foreach (var dir in buffer1)
                    {
                        try
                        {
                            finalArrayFiles = finalArrayFiles.Concat(Directory.GetFiles(dir)).ToArray();
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            buffer2 = buffer2.Concat(Directory.GetDirectories(dir)).ToArray();
                        }
                        catch (Exception)
                        { }

                    }

                    if (buffer2.Length == 0)
                    {
                        break;
                    }

                    buffer1 = buffer2.ToArray();
                    buffer2 = new string[0];
                }

                DateTime end = DateTime.Now;
                if (callout)
                    Console.WriteLine($"End collecting files array, time = [{end}], time spent = [{end - start}]");

                if (filter != "")
                {
                    List<string> listResult = new List<string>();

                    start = DateTime.Now;
                    if (callout)
                        Console.WriteLine($"Start filtering files array, time = [{start}]");


                    for (int i = 0; i < finalArrayFiles.Length; i++)
                    {
                        string[] split = finalArrayFiles[i].Split('.');

                        if (split[split.Length - 1] == filter)
                        {
                            listResult.Add(finalArrayFiles[i]);
                        }
                    }

                    end = DateTime.Now;
                    if (callout)
                        Console.WriteLine($"End filtering files array, time = [{end}], time spent = [{end - start}]");

                    return listResult.ToArray();
                }

                return finalArrayFiles;
            }

            public static class Write
            {
                static public void WriteInTxt(string targetFile, string line, bool rewriteOff)
                {
                    using (StreamWriter sw = new StreamWriter(targetFile, rewriteOff, System.Text.Encoding.Default))
                    {
                        sw.WriteLine($"{line}");
                    }
                }

                static public void WriteInTxt(string targetFile, string[] lines)
                {
                    using (StreamWriter sw = new StreamWriter(targetFile, true, System.Text.Encoding.Default))
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            sw.WriteLine($"{lines[i]}");
                        }
                    }
                }
            }

            public static class Read
            {
                static public string[] GetTXTFileContent(string filePath)
                {
                    List<string> result = new List<string>();
                    string line;

                    using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            result.Add(line);
                        }
                    }

                    return result.ToArray();
                }
            }
        }
    }
}

