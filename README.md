# Securilulz

A simple console app to constantly upload video from your webcam to OneDrive


### Why?

There's no need to pay for commercial products if everything you need comes with your laptop.


### Usage

 * **End-users** (non-coders): 
  * From the [releases](https://github.com/JoshuaKGoldberg/Securilulz/releases) page, download the newest (highest numbered) .zip and extract it onto your computer.
  * Open WebcamRecorder.exe.config (it's just a plain text file) in a text editor (Notepad if you're on Windows), and change the "OutputDirectory" section to match a path on your computer.
 * **Developers**:
  * Open the solution in Visual Studio 2015.
  * Change app.config to point to your desired output directory.
  * Build & run.

If you'd like the option for command line parameters, you can always fork the repository and do it yourself.


### Development requirements

 * [Microsoft Expression Encoder 4](https://www.microsoft.com/en-us/download/details.aspx?id=18974)
 * .NET Framework 4.5.2
 * Visual Studio 2015
