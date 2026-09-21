using Encode2It;
using Encode2It.Core;

// Version Info
string[] versioninfo = [
    "v1.0",
    "Welcome to your new guide (encoder)!"
];

// Awesome banner
Console.WriteLine("-----------------------------------------------------------------------");
Console.WriteLine("""

███████╗███╗   ██╗ ██████╗ ██████╗ ██████╗ ███████╗██████╗ ██╗████████╗
██╔════╝████╗  ██║██╔════╝██╔═══██╗██╔══██╗██╔════╝╚════██╗██║╚══██╔══╝
█████╗  ██╔██╗ ██║██║     ██║   ██║██║  ██║█████╗   █████╔╝██║   ██║   
██╔══╝  ██║╚██╗██║██║     ██║   ██║██║  ██║██╔══╝  ██╔═══╝ ██║   ██║   
███████╗██║ ╚████║╚██████╗╚██████╔╝██████╔╝███████╗███████╗██║   ██║   
╚══════╝╚═╝  ╚═══╝ ╚═════╝ ╚═════╝ ╚═════╝ ╚══════╝╚══════╝╚═╝   ╚═╝   

""");
Console.WriteLine($"Version {versioninfo[0]} - {versioninfo[1]}");
Console.WriteLine("Made by ichigoleite");
Console.WriteLine("-----------------------------------------------------------------------");
Console.WriteLine("\n");

// Create config class.
Config config = new();

// Create TimedTasks class.
TimedTasks timedTasks = new(config);

// Start loops.
Task.WaitAll(
    timedTasks.ListingLoop(),
    timedTasks.WeatherLoop()
);

Console.WriteLine("Goodbye.");
