using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace VoxMicrophone
{
    public class VoxSpeaker
    {
        public List<string> WaveQueue = new List<string>();

        public int ProcessText(List<string> text, string selectedVoice)
        {
            foreach (string token in text)
            {
                if (!File.Exists($"{selectedVoice}\\{token}.wav"))
                {
                    MainProgram.PrintError($"Word \"{token}.wav\" does not exist.");
                    WaveQueue.Clear();
                    return 1;
                }
                else WaveQueue.Add($"{selectedVoice}\\{token}.wav");
            }
            return 0;
        }

        public void PlayQueue(ref WaveOut waveOut)
        {
            byte[] buffer = new byte[1024];
            MemoryStream stream = new MemoryStream();

            WaveFileWriter writer = null;
            WaveFileReader reader = null;

            foreach (string word in WaveQueue)
            {
                try
                {
                    reader = new WaveFileReader(word);
                    if (writer == null) writer = new WaveFileWriter(stream, reader.WaveFormat);

                    if (!reader.WaveFormat.Equals(writer.WaveFormat))
                    {
                        MainProgram.PrintError("One of the WAVE files has a different format.");
                        WaveQueue.Clear();
                        return;
                    }

                    int read = 0;
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, read);
                    }
                }
                catch(Exception ex)
                {
                    MainProgram.PrintError(ex.Message);
                }
            }

            writer.Flush();
            reader.Dispose();

            if (MainProgram.PlayDelay != 0)
            {
                MainProgram.PrintMessage($"Waiting {MainProgram.PlayDelay} second/s before playing...");
                Thread.Sleep(TimeSpan.FromSeconds(MainProgram.PlayDelay));
            }
            MainProgram.PrintMessage("Playing...");

            stream.Seek(0, SeekOrigin.Begin);
            using (WaveFileReader newReader = new WaveFileReader(stream))
            {
                waveOut.Init(newReader);
                waveOut.Play();
                while (waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    Thread.Sleep(500);
                }
            }
            writer.Dispose();
            stream.Dispose();

            WaveQueue.Clear();
        }
    }
}
