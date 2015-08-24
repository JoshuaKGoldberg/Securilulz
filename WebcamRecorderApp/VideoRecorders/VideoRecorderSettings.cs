namespace WebcamRecorder.VideoRecorders
{
    using System;

    public class VideoRecorderSettings : IVideoRecorderSettings
    {
        /// <summary>
        /// The output directory to place files in.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// How long each video should be.
        /// </summary>
        public int VideoLength { get; set; }

        /// <summary>
        /// Prints a summary of the settings to the console.
        /// </summary>
        public void PrintToConsole()
        {
            Console.WriteLine("OutputDirectory: {0}", this.OutputDirectory);
            Console.WriteLine("VideoLength: {0}", this.VideoLength);
        }
    }
}
