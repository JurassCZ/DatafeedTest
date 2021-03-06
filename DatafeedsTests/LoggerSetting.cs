﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Datafeeds.Tests
{
    public static class LoggerSetting
    {
        static bool isInitialized;
        public static void init()
        {
            if (isInitialized) return;

            Stream myFile = File.Open("TestsLog.txt", FileMode.Create);
            Trace.Listeners.Add(new CustomListener(myFile));
            Trace.AutoFlush = true;

            Trace.Listeners.Add(new ConsoleTraceListener());

            isInitialized = true;
        }

        class CustomListener : TextWriterTraceListener
        {
            public CustomListener(Stream file) : base(file)
            {

            }
        }


    }
}
