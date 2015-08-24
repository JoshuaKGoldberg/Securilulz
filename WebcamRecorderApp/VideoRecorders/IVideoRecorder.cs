namespace WebcamRecorder.VideoRecorders
{
    using System;

    public interface IVideoRecorder : IDisposable {
        void PrintLocationToConsole();
    }
}
