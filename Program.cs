using System;
using System.Collections.Generic;
using Adofai.Misc;
using CommandLine;
using CommandLine.Text;

namespace Adofai;

class Options {
    [Value(0, MetaName = "File", HelpText = "File to be loaded", Required = true)] 
    public string FileName { get; set; }
    
    [Option('a', "auto", HelpText = "Turn on Autoplay")]
    public bool Auto { get; set; }

    [Option('l', "loglevel", HelpText = "Sets the logging level of the game", Default = LogLevel.Info)]
    public LogLevel LogLevel { get; set; }
}

public static class Program {
    public static string FilePath;
    public static bool Auto;
    
    [STAThread]
    static void Main(string[] args) {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }
    
    static void RunOptions(Options opts) {
        FilePath = opts.FileName;
        Auto = opts.Auto;
        Logger.LogLevel = opts.LogLevel;
        
        Logger.Info("Loading Game...");
        using (var game = new MainGame())
            game.Run();
    }
    static void HandleParseError(IEnumerable<Error> errs) {
        
    }
}
