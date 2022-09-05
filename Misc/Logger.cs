using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;

namespace Adofai.Misc; 

public static class Logger {
    public static LogLevel LogLevel { get; set; } = LogLevel.Debug;
    private static FileStream logFile;
    private static StreamWriter streamWriter;
    private static Task writeTask = Task.CompletedTask;
    private static string typeText;
    private static string execDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

    private static readonly Dictionary<LogLevel, ConsoleColor> Colors = new Dictionary<LogLevel, ConsoleColor> {
        { LogLevel.Debug, ConsoleColor.Green  },
        { LogLevel.Info , ConsoleColor.White  },
        { LogLevel.Warn , ConsoleColor.Yellow },
        { LogLevel.Error, ConsoleColor.Red    }
    };
    
    public static void Log(object logObj, LogLevel level) {
        if (LogLevel < level) { return; }

        string log = $"[{DateTime.Now.ToLongTimeString()}] [{level}]: {logObj}\n";
        
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = Colors[level];
        Console.Write(log);
        Console.ForegroundColor = originalColor;
            
        typeText += log;

        if (!writeTask.IsCompleted) { return; }
        writeTask = streamWriter!.WriteAsync(typeText);
        typeText = "";
    }

    public static void WaitFlush() {
        writeTask.Wait();

        streamWriter!.Write(typeText);
        typeText = "";
    }

    public static void Init(LogLevel logLevel) {
        LogLevel = logLevel;

        string logPath = Path.Join(execDirectory, "Logs");

        if (!Directory.Exists(logPath)) 
            Directory.CreateDirectory(logPath);
        
        // Compress existing latest.log
        if (File.Exists(logPath + "/latest.log")) {
            using FileStream originalFileStream = File.Open(logPath + "/latest.log", FileMode.Open);
            string gzFileLoc = new StreamReader(originalFileStream).ReadLine() ?? string.Empty;

            try {
                gzFileLoc = logPath + gzFileLoc[gzFileLoc.LastIndexOf('/')..] + ".gz";
            }
            catch (Exception) { // in case it cant find date of latest.log, make name have random value
                gzFileLoc = logPath + "/Unknown-" + 
                            (int)Math.Abs(new Random().Next()*1000000*Math.PI) + ".log.gz";
            }

            originalFileStream.Seek(0, SeekOrigin.Begin);

            using FileStream compressedFileStream = File.Create(gzFileLoc);
            using GZipStream compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
            originalFileStream.CopyTo(compressor);
        }

        string logFileName = $"{DateTime.Now:yyyy-MM-dd}-";

        int i = 1;
        while (File.Exists($"{logPath}/{logFileName}i.log.gz")) { i++; }  // Get a unique number for the name

        logFileName += i + ".log";
            
        logFile = File.OpenWrite(logPath + "/latest.log");
        streamWriter = new StreamWriter(logFile);
        streamWriter.AutoFlush = true;
        typeText = "";
        Info($"Logging to: Logs/{logFileName}");
    }
    
    public static void Error(object log) => Log(log, LogLevel.Error);
    public static void Warn(object log) => Log(log, LogLevel.Warn);
    public static void Info(object log) => Log(log, LogLevel.Info);
    public static void Debug(object log) => Log(log, LogLevel.Debug);
}

public enum LogLevel {
    None,
    Error,
    Warn,
    Info,
    Debug
}
