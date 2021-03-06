﻿using System;
using System.Drawing;
using System.IO;
using Pastel;

namespace Cpp2IL.Core
{
    public static class Logger
    {
        internal static readonly Color VERB = Color.Gray;
        internal static readonly Color INFO = Color.LightBlue;
        internal static readonly Color WARN = Color.Yellow;
        internal static readonly Color ERROR = Color.DarkRed;
        
        internal static bool DisableColor { private get; set; }

        internal static bool ShowVerbose { private get; set; }
        
        private static bool LastNoNewline;

        public static void VerboseNewline(string message, string source = "Program") => Verbose($"{message}\n", source);

        public static void Verbose(string message, string source = "Program")
        {
            if(ShowVerbose)
                Write("Verb", source, message, VERB);
        }
        
        public static void InfoNewline(string message, string source = "Program") => Info($"{message}\n", source);

        public static void Info(string message, string source = "Program")
        {
            Write("Info", source, message, INFO);
        }
        
        public static void WarnNewline(string message, string source = "Program") => Warn($"{message}\n", source);

        public static void Warn(string message, string source = "Program")
        {
            Write("Warn", source, message, WARN);
        }
        
        public static void ErrorNewline(string message, string source = "Program") => Error($"{message}\n", source);

        public static void Error(string message, string source = "Program")
        {
            Write("Fail", source, message, ERROR);
        }

        internal static void Write(string level, string source, string message, Color color)
        {
            WritePrelude(level, source, color);
            
            LastNoNewline = !message.EndsWith("\n");
            
            if (!DisableColor)
                message = message.Pastel(color);
            
            Console.Write(message);
        }

        private static void WritePrelude(string level, string source, Color color)
        {
            var message = $"[{level}] [{source}] ";
            if (!DisableColor)
                message = message.Pastel(color);
            
            if(!LastNoNewline)
                Console.Write(message);
        }

        public static void CheckColorSupport()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                DisableColor = true;
                WarnNewline("Looks like you're running on a non-windows platform. Disabling ANSI color codes.");
            }
            else if (Directory.Exists(@"Z:\usr\"))
            {
                DisableColor = true;
                WarnNewline("Looks like you're running in wine or proton. Disabling ANSI color codes.");
            } else if (Environment.GetEnvironmentVariable("NO_COLOR") != null)
            {
                DisableColor = true; //Just manually set this, even though Pastel respects the environment variable
                WarnNewline("NO_COLOR set, disabling ANSI color codes as you requested.");
            }
        }
    }
}