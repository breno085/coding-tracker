using System;
using System.Diagnostics;

namespace coding_tracker.Models
{
    public class StopWatch
    {
        DateTime? startTime = null;
        DateTime? endTime = null;
        CodingTracker code = new CodingTracker();
        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            ConsoleKeyInfo keyInfo;
            
            Console.WriteLine("Press 'S' to start the stopwatch, 'E' to stop it, and 'R' to reset. Press 'Q' to quit.");

            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.S)
                {
                    if (!stopwatch.IsRunning)
                    {
                        stopwatch.Start();
                        startTime = DateTime.Now;
                        Console.WriteLine("Stopwatch started.");
                    }
                    else
                    {
                        Console.WriteLine("Stopwatch is already running.");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.E)
                {
                    if (stopwatch.IsRunning)
                    {
                        stopwatch.Stop();
                        endTime = DateTime.Now;
                        Console.WriteLine("Stopwatch stopped.");
                        Console.WriteLine("Elapsed time: {0:hh\\:mm}", stopwatch.Elapsed);
                        Console.WriteLine("Start time: {0}", startTime.HasValue ? startTime.Value.ToString("HH:mm") : "N/A");
                        Console.WriteLine("End time: {0}", endTime.HasValue ? endTime.Value.ToString("HH:mm") : "N/A");

                        if (startTime.HasValue && endTime.HasValue)
                        {
                            code.StartTime = startTime.Value.TimeOfDay;
                            code.EndTime = endTime.Value.TimeOfDay;
                            code.Duration = stopwatch.Elapsed;

                            CodingController insertTimes = new CodingController();

                            insertTimes.Post(code);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Stopwatch is not running.");
                    }
                }
                else if (keyInfo.Key == ConsoleKey.R)
                {
                    stopwatch.Reset();
                    startTime = null;
                    endTime = null;
                    Console.WriteLine("Stopwatch reset.");
                }
            } while (keyInfo.Key != ConsoleKey.Q);

            Console.WriteLine("Application exited.");
        }
    }
}
