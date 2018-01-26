using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Datafeeds.Properties
{
    public static class LoggerSetting
    {
        public static void init()
        {
            Stream myFile = File.Open("ApplicationLog.txt", FileMode.Append);
            TextWriterTraceListener listener = new TextWriterTraceListener(myFile);
            Trace.Listeners.Add(new MyListener(myFile));
            Trace.AutoFlush = true;
        }

        class MyListener : TextWriterTraceListener
        {
            public MyListener(Stream file) : base(file)
            {

            }
            ~MyListener() 
            {
                Trace.WriteLine("Application finished.");
            }
        }


    }
}
