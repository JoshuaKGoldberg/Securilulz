namespace WebcamRecorder
{
    using System;
    using System.Configuration;
    using VideoRecorders;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");

            var settings = new VideoRecorderSettings
            {
                OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"],
                VideoLength = int.Parse(ConfigurationManager.AppSettings["VideoLength"])
            };

            using (var recorder = new WebcamRecorder(settings))
            {
                Console.WriteLine("Initialized.");
                settings.PrintToConsole();
                Console.WriteLine("Type 'exit' or 'quit' to quit.");
                Console.WriteLine();
                recorder.PrintLocationToConsole();

                string command;
                do
                {
                    command = Console.ReadLine().ToLower();
                }
                while (command != "exit" && command != "quit");
            }

            Console.WriteLine("Quitting.");
            Environment.Exit(0);
        }
    }
}
