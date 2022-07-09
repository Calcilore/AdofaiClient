﻿using System;
using System.Collections.Generic;
using Adofai.Misc;
using CommandLine;

namespace Adofai;

class Options {
    [Value(0, MetaName = "File", HelpText = "File to be loaded")] 
    public string FileName { get; set; }
    
    [Option('a', "auto", HelpText = "Turn on Autoplay")]
    public bool Auto { get; set; }

    [Option('l', "loglevel", HelpText = "Sets the logging level of the game", Default = LogLevel.Info)]
    public LogLevel LogLevel { get; set; }
    
    [Option('o', "offset", HelpText = "Sets the music offset in game", Default = 0f)]
    public float Offset { get; set; }
}

public static class Program {
    public static string FilePath;
    public static bool Auto;
    public static float OffsetOption;
    
    [STAThread]
    static void Main(string[] args) {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }
    
    static void RunOptions(Options opts) {
        FilePath = opts.FileName;
        Auto = opts.Auto;
        OffsetOption = opts.Offset / 1000f;
        Logger.LogLevel = opts.LogLevel;
        
        Logger.Info("Loading Game...");
        using (var game = new MainGame())
            game.Run();
    }
    static void HandleParseError(IEnumerable<Error> errs) {
        
    }
}
