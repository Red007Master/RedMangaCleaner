﻿using Newtonsoft.Json;
using RedsCleaningProjects.Core;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class PRORLauncher
{
    public string PRORPath { get; set; }
    public string StartArgumentsPath { get; set; }
    public StartArguments Arguments { get; set; }

    public PRORLauncher(string pRORPath, string startArgumentsPath, StartArguments startArguments)
    {
        PRORPath = pRORPath;
        StartArgumentsPath = startArgumentsPath;
        Arguments = startArguments;
    }

    public void Start()
    {
        string serialized = JsonConvert.SerializeObject(Arguments);
        File.WriteAllText(StartArgumentsPath, serialized);

        Thread.Sleep(100);

        ProcessStartInfo processStartInfo = new ProcessStartInfo(PRORPath);
        processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        Process.Start(processStartInfo);
    }
}