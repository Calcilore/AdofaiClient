using System;
using System.Collections.Generic;

namespace Adofai.Misc; 

public static class Logger {
    public static LogLevel LogLevel;
    private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor> {
        { LogLevel.Debug, ConsoleColor.Green  },
        { LogLevel.Info , ConsoleColor.White  },
        { LogLevel.Warn , ConsoleColor.Yellow },
        { LogLevel.Error, ConsoleColor.Red    }
    };

    public static void Log(string log, LogLevel severity) {
        if (severity > LogLevel) return;

        ConsoleColor previousColor = Console.ForegroundColor;
        Console.ForegroundColor = Colors[severity];
        Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [{severity}]: {log}");
        Console.ForegroundColor = previousColor;
    }

    public static void Error(object log)
        => Log(log.ToString(), LogLevel.Error);
    
    public static void Warn(object log)
        => Log(log.ToString(), LogLevel.Warn);
    
    public static void Info(object log)
        => Log(log.ToString(), LogLevel.Info);
    
    public static void Debug(object log)
        => Log(log.ToString(), LogLevel.Debug);
}

public enum LogLevel {
    None,
    Error,
    Warn,
    Info,
    Debug
}
