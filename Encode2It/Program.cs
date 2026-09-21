using Encode2It;
using Encode2It.Core;

// Version Info
string[] versioninfo = [
    "v1.1",
    "Cable contributes to life!"
];

// Awesome banner
Console.WriteLine("-----------------------------------------------------------------------");
Console.WriteLine("""

ichigoleite
ggggggggggggggg                                                                                                        
ggggggggggggggg  ÆÆÆÆÆÆÆÆ                             ÆÆ           ÆÆÆÆÆ  ÆÆÆ                                          
güÇÇÇÞ6G6Ç6666g  ÆÆÆÆÆÆÆÆ                             ÆÆ          ÆÆÆÆÆÆÆ ÆÆÆ  ÆÆ                                      
g3ÇÇÇÞ6ÞüÇ6666g  ÆÆ                                   ÆÆ         ÆÆÆ  ÆÆÆ ÆÆÆ  ÆÆ                                      
ggggggggggggggg  ÆÆ      ÆÆÆÆÆÆ    ÆÆÆÆ    ÆÆÆÆ    ÆÆÆÆÆ   ÆÆÆÆ  ÆÆÆ  ÆÆÆ ÆÆÆ ÆÆÆÆÆ                                    
ggggggggggggggg  ÆÆÆÆÆÆÆ ÆÆÆÆÆÆÆ  ÆÆÆÆÆÆ  ÆÆÆÆÆÆ  ÆÆÆÆÆÆ  ÆÆÆÆÆÆ      ÆÆÆ ÆÆÆ ÆÆÆÆÆ                                    
                 ÆÆÆÆÆÆÆ ÆÆÆÆ ÆÆ ÆÆÆ  ÆÆ ÆÆÆ ÆÆÆ ÆÆÆ  ÆÆ ÆÆÆ  ÆÆ   ÆÆÆÆÆ  ÆÆÆ  ÆÆ                                      
ggggggGGGGGGGGG  ÆÆ      ÆÆÆ  ÆÆ ÆÆÆ     ÆÆ   ÆÆÆÆÆ   ÆÆ ÆÆÆÆÆÆÆ   ÆÆÆÆ   ÆÆÆ  ÆÆ                                      
gÇ¯¯gg GGGlG6GG  ÆÆ      ÆÆÆ  ÆÆ ÆÆÆ     ÆÆ   ÆÆÆÆÆ   ÆÆ ÆÆÆÆÆÆÆÆ ÆÆÆ     ÆÆÆ  ÆÆ                                      
g6Þ‹GggGGÞ‡‡‹ÞG  ÆÆ      ÆÆÆ  ÆÆ ÆÆÆ  ÆÆ ÆÆ   ÆÆ ÆÆÆ  ÆÆ ÆÆÆ  ÆÆ ÆÆÆ      ÆÆÆ  ÆÆ                                      
gg‡*gggGG6‡3üGG  ÆÆÆÆÆÆÆÆÆÆÆ  ÆÆ  ÆÆÆÆÆÆ ÆÆÆÆÆÆÆ ÆÆÆÆÆÆÆ  ÆÆÆÆÆÆ ÆÆÆÆÆÆÆÆ ÆÆÆ  ÆÆÆÆ                                    
gü`¯Þg GG6663GG  ÆÆÆÆÆÆÆÆÆÆÆ  ÆÆ   ÆÆÆÆ   ÆÆÆÆÆ   ÆÆÆÆÆÆ   ÆÆÆÆ  ÆÆÆÆÆÆÆÆ ÆÆÆ  ÆÆÆÆ                                    
ggggggGGGGGGGGG 

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
