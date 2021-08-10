using System;
using Pastel;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using System.IO;

namespace VoxMicrophone
{
    public class MainProgram
    {
        public static bool DeviceSelected = false;
        public static int PlayDelay = 0;
        public static string SelectedVoice = "vox";

        public static List<string> ValidVoices = new List<string>
        {
            "vox",
            "fvox",
            "hgrunt"
        };

        static void Main(string[] args)
        {
            Console.Title = "VoxMicrophone";
            Console.WriteLine("VoxMicrophone ".Pastel(Color.Orange) + "by AestheticalZ".Pastel(Color.LightYellow));
            Console.WriteLine("Use the " + "help".Pastel(Color.Yellow) + " command to get a list of available commands.");
            Console.WriteLine("Use the " + "setvoice".Pastel(Color.Yellow) + " command to change the voice.");

            VoxSpeaker voxSpeaker = new VoxSpeaker();
            WaveOut waveOut = new WaveOut();

            EnterCommand:
            Console.Write("\n-> ".Pastel(Color.CornflowerBlue));

            string[] command = Console.ReadLine().Trim().ToLower().Split(' ');
            Console.WriteLine();
            switch(command[0])
            {
                case "say":
                    if (command.Length == 1)
                    {
                        PrintError("Not enough arguments.");
                        goto EnterCommand;
                    }

                    if (DeviceSelected == false)
                    {
                        PrintError("You have not selected any device!");
                        goto EnterCommand;
                    }
                    List<string> sentence = command.ToList(); sentence.RemoveAt(0);

                    PrintMessage("Processing sentence...");
                    if(voxSpeaker.ProcessText(sentence, SelectedVoice) == 1)
                    {
                        goto EnterCommand;
                    }
                    voxSpeaker.PlayQueue(ref waveOut);

                    goto EnterCommand;

                case "devices":
                    Console.WriteLine(string.Concat("Do note that due to limitations with the Windows API,\n",
                                      "names are limited to 32 characters.\n").Pastel(Color.Red));

                    Console.WriteLine("------------AVAILABLE DEVICES------------".Pastel(Color.Aqua));
                    for (int n = -1; n < WaveOut.DeviceCount; n++)
                    {
                        var caps = WaveOut.GetCapabilities(n);
                        Console.WriteLine($"Device {n}: ".Pastel(Color.Aquamarine) + caps.ProductName);
                    }
                    Console.WriteLine("-----------------------------------------".Pastel(Color.Aqua));
                    goto EnterCommand;

                case "setvoice":
                    if (command.Length == 1)
                    {
                        PrintError("Not enough arguments.");
                        goto EnterCommand;
                    }

                    if(!ValidVoices.Contains(command[1]))
                    {
                        PrintError($"Voice \"{command[1]}\" does not exist.");
                        goto EnterCommand;
                    }
                    SelectedVoice = command[1];
                    PrintMessage($"Voice is now set to {command[1]}.");

                    goto EnterCommand;

                case "setdevice":
                    if (command.Length == 1)
                    {
                        PrintError("Not enough arguments.");
                        goto EnterCommand;
                    }

                    if (int.TryParse(command[1], out int deviceNum))
                    {
                        if(deviceNum > WaveOut.DeviceCount)
                        {
                            PrintError("This device number exceeds the amount of available devices.");
                            goto EnterCommand;
                        }
                        waveOut.DeviceNumber = deviceNum;
                        PrintMessage($"Selected device is now \"{WaveOut.GetCapabilities(deviceNum).ProductName}\".");
                        DeviceSelected = true;
                    }
                    else PrintError("Please provide a correct device number.");
                    goto EnterCommand;

                case "setdelay":
                    if(command.Length == 1)
                    {
                        PrintError("Not enough arguments.");
                        goto EnterCommand;
                    }

                    if (int.TryParse(command[1], out int delay))
                    {
                        PlayDelay = delay;
                        PrintMessage($"Delay is now {delay}.");
                    }
                    else PrintError("Please provide a correct delay.");
                    goto EnterCommand;

                case "clear":
                    Console.Clear();
                    Console.WriteLine("VoxMicrophone ".Pastel(Color.Orange)
                        + "by AestheticalZ".Pastel(Color.LightYellow));
                    Console.WriteLine("Use the " + "help".Pastel(Color.Yellow) + " command to get a list of available commands.");
                    Console.WriteLine("Use the " + "setvoice".Pastel(Color.Yellow) + " command to change the voice.");

                    goto EnterCommand;

                case "help":
                    PrintHelp();
                    goto EnterCommand;

                default:
                    PrintError("Unknown command.");
                    goto EnterCommand;
            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("------------AVAILABLE COMMANDS------------".Pastel(Color.Aqua));

            Console.Write("say <sentence> : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Says a sentence to the selected device.");

            Console.Write("devices : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Lists available devices and their number IDs.");

            Console.Write("setdevice <number> : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Sets a device ID as the currently selected device.");

            Console.Write("setdelay <seconds> : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Sets a delay before the sentence plays.");

            Console.Write("clear : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Clears the console output.");

            Console.Write("setvoice <voice> : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("Sets the voice. Voice types are vox, fvox and hgrunt.");

            Console.Write("help : ".Pastel(Color.CornflowerBlue));
            Console.WriteLine("This command.");
            Console.WriteLine("------------------------------------------".Pastel(Color.Aqua));
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine("[MESSAGE] ".Pastel(Color.CadetBlue) + message);
        }

        public static void PrintError(string message)
        {
            Console.WriteLine("[ERROR] ".Pastel(Color.IndianRed) + message);
        }
    }
}
