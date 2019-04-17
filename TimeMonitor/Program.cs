using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TimeMonitor
{
    internal class MyApplicationContext : ApplicationContext
    {
    }

    public class Program
    {
        private const string LogFormat = "[{0:s}] {1}\r\n";
        private static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TimeMonitor.log");
        private static readonly object SyncRoot = new object();

        [STAThread]
        public static void Main(string[] args)
        {
            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.PowerModeChanged += OnPowerChange;
            SystemEvents.SessionEnded += OnSessionEnded;

            if (args.Contains("-startup"))
            {
                Log("Ran at startup");
            }
            else
            {
                Log("Started at unspecified point");
            }

            Application.Run(new MyApplicationContext());
        }

        private static void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            Log(e.Reason);
        }

        private static void Log(object s)
        {
            lock (SyncRoot)
            {
                File.AppendAllText(LogFilePath, string.Format(LogFormat, DateTime.Now, s));
            }
        }

        private static void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionRemoteControl:
                    break;
                default:
                    Log(e.Reason);
                    break;
            }
        }

        private static void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                case PowerModes.Suspend:
                    Log(e.Mode);
                    break;
            }
        }
    }
}