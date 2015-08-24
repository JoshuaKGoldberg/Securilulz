namespace WebcamRecorder.VideoRecorders
{
    using Microsoft.Expression.Encoder;
    using Microsoft.Expression.Encoder.Devices;
    using Microsoft.Expression.Encoder.Live;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    public class WebcamRecorder : IVideoRecorder
    {
        /// <summary>
        /// Settings dictating file size and output directory.
        /// </summary>
        private VideoRecorderSettings Settings { get; set; }

        /// <summary>
        /// Constantly running job to receive audio and video from a webcam.
        /// </summary>
        private LiveJob Job { get; set; }

        /// <summary>
        /// The source for audio and video being piped to the <see cref="Job"/>.
        /// </summary>
        private LiveDeviceSource DeviceSource { get; set; }

        /// <summary>
        /// <see cref="Timer"/> that restarts the job every <see cref="videoInterval"/> milliseconds.
        /// </summary>
        private Timer JobTimer { get; set; }

        /// <summary>
        /// <see cref="Timer"/> that saves the data as an image every <see cref="taskDelay"/> milliseconds.
        /// </summary>
        private Timer SaveTimer { get; set; }

        /// <summary>
        /// The name of the currently edited video.
        /// </summary>
        private string VideoName { get; set; }

        public WebcamRecorder(VideoRecorderSettings settings)
        {
            this.Settings = settings;

            this.Initialize();
            RunTaskOnInterval(this.RestartCapture, this.Settings.VideoLength);
        }

        /// <summary>
        /// Disposes of the <see cref="WebcamRecorder"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.Job.IsCapturing)
            {
                this.Job.StopEncoding();
            }

            this.Job.Dispose();
            this.DeviceSource.Dispose();
            this.JobTimer.Dispose();
            this.SaveTimer.Dispose();
        }

        /// <summary>
        /// Prints the location of the video file to the console.
        /// </summary>
        public void PrintLocationToConsole()
        {
            Console.WriteLine("Outputting to {0}...", this.VideoName);
        }

        /// <summary>
        /// Initializes the window component and the <see cref="LiveJob"/> with its stream.
        /// </summary>
        private void Initialize()
        {
            if (this.Job != null && this.Job.IsCapturing)
            {
                this.Job.StopEncoding();
            }
            
            this.InitializeJob();
            this.InitializeFileExport();

            this.Job.StartEncoding();
        }

        /// <summary>
        /// Starts the <see cref="LifeJob"/> with the first found webcam.
        /// </summary>
        private void InitializeJob()
        {
            // Starts a new job for the preview window
            this.Job = new LiveJob();

            // Create a new device source (the first found video and audio devices)
            DeviceSource = Job.AddDeviceSource(
                EncoderDevices.FindDevices(EncoderDeviceType.Video)[0],
                EncoderDevices.FindDevices(EncoderDeviceType.Audio)[0]);

            // Activate the newly created device source
            this.Job.ActivateSource(this.DeviceSource);

            // Apply a typical preset for smooth encoding & playback
            this.Job.ApplyPreset(LivePresets.VC1256kDSL16x9);
        }

        /// <summary>
        /// Sets the job to output to a file named by the current timestamp.
        /// </summary>
        private void InitializeFileExport()
        {
            if (!String.IsNullOrWhiteSpace(this.VideoName))
            {
                Console.WriteLine("Finished outputting to {0}", this.VideoName);
            }
            
            var now = DateTime.Now;
            var paths = new[] { "yyyy", "MM", "dd" }
                .Select(path => String.Format("{0:" + path + "}", now));

            EnsureDirectoryStructureExists(this.Settings.OutputDirectory, paths);

            this.VideoName = String.Format(
                "{0}/{1:yyyy}/{1:MM}/{1:dd}/{1:hh-mm-ss}.wmv",
                this.Settings.OutputDirectory,
                DateTime.Now);

            this.Job.PublishFormats.Add(
                new FileArchivePublishFormat
                {
                    OutputFileName = this.VideoName
                });
        }

        /// <summary>
        /// Ensures a base directory and list of subpaths exists for a file to be placed inside.
        /// </summary>
        /// <param name="outputDirectory">The base directory the subpath exists within.</param>
        /// <param name="paths">A path within the <see cref="outputDirectory"/>, split into an <see cref="IEnumerable"/>.</param>
        private static void EnsureDirectoryStructureExists(string outputDirectory, IEnumerable<string> paths = null)
        {
            var currentPath = outputDirectory;

            System.IO.Directory.CreateDirectory(currentPath);

            if (paths != null)
            {
                foreach (var path in paths)
                {
                    currentPath = (currentPath + @"\" + path).Trim('\\');
                    System.IO.Directory.CreateDirectory(currentPath);
                }
            }
        }

        /// <summary>
        /// Stops and starts capture from the webcam, so a new file is created.
        /// </summary>
        private void RestartCapture()
        {
            lock (this)
            {
                if (!this.Job.IsCapturing)
                {
                    return;
                }

                this.Job.StopEncoding();

                this.Job.PublishFormats.Clear();
                this.InitializeFileExport();

                this.Job.StartEncoding();

                this.PrintLocationToConsole();
            }
        }

        /// <summary>
        /// Runs an <see cref="Action"/> every <see cref="videoInterval"/> milliseconds.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="videoInterval">How often to execute the action, in milliseconds.</param>
        private static Timer RunTaskOnInterval(Action action, int videoInterval)
        {
            var timer = new Timer(videoInterval);

            timer.Elapsed += (a, b) => action();
            timer.Enabled = true;

            return timer;
        }
    }
}
