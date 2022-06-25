using System;
using System.Collections.Generic;
using CommandLine;

namespace Adofai;

class Options {
    [Value(0, MetaName = "File", HelpText = "File to be loaded", Required = true)] 
    public string FileName { get; set; }
    
    [Option('a', "auto", HelpText = "Turn on Autoplay")]
    public bool Auto { get; set; }
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
        
        using (var game = new MainGame())
            game.Run();
    }
    static void HandleParseError(IEnumerable<Error> errs) {
        foreach (Error err in errs) {
            if (err.Tag == ErrorType.MissingValueOptionError) {
                Console.WriteLine("Usage: ");
            }
        }
    }
}
