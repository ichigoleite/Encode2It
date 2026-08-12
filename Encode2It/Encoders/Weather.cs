using Encode2It.Core;

namespace Encode2It.Encoders;

public enum WeatherIcons
{
    Sunny = 1,
    MostlySunny = 2,
    PartlySunny = 3,
    IntermittentCloudsDay = 4,
    HazyDay = 5,
    MostlyCloudyDay = 6,
    Cloudy = 7,
    Overcast = 8,
    Fog = 11,
    Showers = 12,
    MostlyCloudyShowersDay = 13,
    PartlyCloudyShowersDay = 14,
    Thunderstorms = 15,
    MostlyCloudyTStormsDay = 16,
    PartlyCloudyTStormsDay = 17,
    Rain = 18,
    Flurries = 19,
    MostlyCloudyFlurriesDay = 20,
    PartlyCloudyFlurriesDay = 21,
    Snow = 22,
    MostlyCloudySnowDay = 23,
    Ice = 24,
    Sleet = 25,
    FreezingRain = 26,
    MixedRnAndSn = 29,
    Hot = 30,
    Cold = 31,
    Windy = 32,
    Clear = 33,
    MostlyClear = 34,
    PartlyClear = 35,
    IntermittentCloudsNight = 36,
    HazyNight = 37,
    MostlyCloudyNight = 38,
    PartlyCloudyShowersNight = 39,
    MostlyCloudyShowersNight = 40,
    PartlyCloudyTStormsNight = 41,
    MostlyCloudyTStormsNight = 42,
    MostlyCloudyFlurriesNight = 43,
    MostlyCloudySnowNight = 44
}

public class WeatherCondition
{
    public string Condition = "CLEAR";
    public WeatherIcons Icon = WeatherIcons.Sunny;
}

public class WeatherInfo
{
    public string HeadendId { get; set; } = "ZAP2IT";
    public string Location { get; set; } = "Kirby";
    public int Temperature { get; set; } = 0;
    public int HighTemp { get; set; } = 0;
    public int LowTemp { get; set; } = 0;
    public WeatherCondition Condition { get; set; } = new();
    public string DaypartTag { get; set; } = "AFT";
    public string DaypartStr { get; set; } = "Afternoon";
    public string WindDirection { get; set; } = "N";
    public double Pressure = 0.00;
    public int WindSpeed = 0;
}

public class CurrentConditions
{
    public WeatherInfo WxInfo { get; set; } = new();
    public string Generate()
    {
        Delimited delimited = new();
        List<string[]> lines = [];

        lines.Add(
            [
                WxInfo.HeadendId,
                WxInfo.Location,
                WxInfo.Temperature.ToString(),
                WxInfo.Condition.Condition,
                WxInfo.WindDirection,
                ((int)WxInfo.Condition.Icon).ToString(),
                WxInfo.HighTemp.ToString(),
                ((int)WxInfo.Pressure).ToString(),
                WxInfo.LowTemp.ToString(),
                WxInfo.Temperature.ToString()
            ]
        );

        delimited.Lines = [.. lines];
        return delimited.Generate();
    }
}

public class ThreeDayForecast
{
    public WeatherInfo[] WxInfo { get; set; } = [new(), new(), new()];
    public string Generate()
    {
        Delimited delimited = new();
        List<string[]> lines = [];

        lines.Add(
            [
                WxInfo[0].HeadendId,
                WxInfo[0].Location,
                WxInfo[0].DaypartTag,
                WxInfo[0].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[0].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[0].Condition.Icon).ToString().PadLeft(2, '0'),
                WxInfo[1].DaypartTag,
                WxInfo[1].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[1].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[1].Condition.Icon).ToString().PadLeft(2, '0'),
                WxInfo[2].DaypartTag,
                WxInfo[2].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[2].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[2].Condition.Icon).ToString().PadLeft(2, '0'),
            ]
        );

        delimited.Lines = [.. lines];
        return delimited.Generate();
    }
}

public class EighteenHourForecast
{
    public WeatherInfo[] WxInfo { get; set; } = [new(), new(), new()];
    public string Generate()
    {
        Delimited delimited = new();
        List<string[]> lines = [];

        lines.Add(
            [
                WxInfo[0].HeadendId,
                WxInfo[0].Location,
                WxInfo[0].DaypartTag,
                WxInfo[0].DaypartStr,
                WxInfo[0].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[0].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[0].Condition.Icon).ToString().PadLeft(2, '0'),
                WxInfo[1].DaypartTag,
                WxInfo[1].DaypartStr,
                WxInfo[1].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[1].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[1].Condition.Icon).ToString().PadLeft(2, '0'),
                WxInfo[2].DaypartTag,
                WxInfo[2].DaypartStr,
                WxInfo[2].HighTemp.ToString().PadLeft(2, '0'),
                WxInfo[2].LowTemp.ToString().PadLeft(2, '0'),
                ((int)WxInfo[2].Condition.Icon).ToString().PadLeft(2, '0'),
            ]
        );

        delimited.Lines = [.. lines];
        return delimited.Generate();
    }
}

public class WeatherDataset
{
    public CurrentConditions currentConditions = new();
    public ThreeDayForecast threeDayForecast = new();
    public EighteenHourForecast eighteenHourForecast = new();
}