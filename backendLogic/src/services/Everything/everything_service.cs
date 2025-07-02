using System;
using System.Diagnostics;

namespace backendLogic.src.services.Everything
{
    public class EverythingService
    {
        public static void Run_EverythingProcess()
        {

            string everythingPath = @".\everything.exe";

            if (!Process.GetProcessesByName("Everything").Any())
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = everythingPath,
                    Arguments = "-startup",
                    CreateNoWindow = true,
                    Verb = "runas",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                try
                {
                    Process.Start(startInfo);
                    Console.WriteLine("Everything launched.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start Everything: {ex.Message}");
                }

            }
            else
            {
                Console.WriteLine("Everything is already running.");
            }
        }
    }
   
}
