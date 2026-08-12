using Encode2It.Encoders;
using Encode2It.Core;
using Encode2It.Schemas.Inputs.Weather;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Encode2It.Inputs;

public class WeatherInputs
{
    private readonly Logger Log = new("Inputs - Weather");

    // From https://open-meteo.com/en/docs
    private readonly Dictionary<int, WeatherCondition> OpenMeteo2WxNight = new()
    {

        {
            0, new() {Icon = WeatherIcons.Clear, Condition = "CLEAR"}
        },
        {
            1, new () { Icon = WeatherIcons.MostlyClear, Condition = "MOSTLY CLEAR"}
        },
        {
            2, new () { Icon = WeatherIcons.PartlyClear, Condition = "PARTLY CLEAR"}
        },
        {
            3, new () { Icon = WeatherIcons.IntermittentCloudsNight, Condition = "INTERMITTENT CLOUDS"}
        },
        {
            45, new () { Icon = WeatherIcons.Fog, Condition = "FOG"}
        },
        {
            48, new () { Icon = WeatherIcons.Fog, Condition = "FOG"}
        },
        {
            51, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            53, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            55, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            56, new () { Icon = WeatherIcons.Flurries, Condition = "FLURRIES"}
        },
        {
            57, new () { Icon = WeatherIcons.Flurries, Condition = "FLURRIES"}
        },
        {
            61, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            63, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            65, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            66, new () { Icon = WeatherIcons.FreezingRain, Condition = "FREEZING RAIN"}
        },
        {
            67, new () { Icon = WeatherIcons.FreezingRain, Condition = "FREEZING RAIN"}
        },
        {
            71, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            73, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            75, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            77, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            80, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            81, new () { Icon = WeatherIcons.PartlyCloudyShowersNight, Condition = "SHOWERS"}
        },
        {
            82, new () { Icon = WeatherIcons.MostlyCloudyShowersNight, Condition = "SHOWERS"}
        },
        {
            85, new () { Icon = WeatherIcons.Snow, Condition = "SNOW SHOWERS"}
        },
        {
            86, new () { Icon = WeatherIcons.MostlyCloudySnowNight, Condition = "SNOW SHOWERS"}
        },
        {
            95, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
        {
            96, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
        {
            99, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
    };

    private readonly Dictionary<int, WeatherCondition> OpenMeteo2WxDay = new()
    {

        {
            0, new() {Icon = WeatherIcons.Sunny, Condition = "SUNNY"}
        },
        {
            1, new () { Icon = WeatherIcons.MostlySunny, Condition = "MOSTLY SUNNY"}
        },
        {
            2, new () { Icon = WeatherIcons.PartlySunny, Condition = "PARTLY SUNNY"}
        },
        {
            3, new () { Icon = WeatherIcons.IntermittentCloudsDay, Condition = "INTERMITTENT CLOUDS"}
        },
        {
            45, new () { Icon = WeatherIcons.Fog, Condition = "FOG"}
        },
        {
            48, new () { Icon = WeatherIcons.Fog, Condition = "FOG"}
        },
        {
            51, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            53, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            55, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            56, new () { Icon = WeatherIcons.Flurries, Condition = "FLURRIES"}
        },
        {
            57, new () { Icon = WeatherIcons.Flurries, Condition = "FLURRIES"}
        },
        {
            61, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            63, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            65, new () { Icon = WeatherIcons.Rain, Condition = "RAIN"}
        },
        {
            66, new () { Icon = WeatherIcons.FreezingRain, Condition = "FREEZING RAIN"}
        },
        {
            67, new () { Icon = WeatherIcons.FreezingRain, Condition = "FREEZING RAIN"}
        },
        {
            71, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            73, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            75, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            77, new () { Icon = WeatherIcons.Snow, Condition = "SNOW"}
        },
        {
            80, new () { Icon = WeatherIcons.Showers, Condition = "SHOWERS"}
        },
        {
            81, new () { Icon = WeatherIcons.PartlyCloudyShowersDay, Condition = "SHOWERS"}
        },
        {
            82, new () { Icon = WeatherIcons.MostlyCloudyShowersDay, Condition = "SHOWERS"}
        },
        {
            85, new () { Icon = WeatherIcons.Snow, Condition = "SNOW SHOWERS"}
        },
        {
            86, new () { Icon = WeatherIcons.MostlyCloudySnowDay, Condition = "SNOW SHOWERS"}
        },
        {
            95, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
        {
            96, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
        {
            99, new () { Icon = WeatherIcons.Thunderstorms, Condition = "THUNDERSTORMS"}
        },
    };

    // Converted from Python (https://gist.github.com/RobertSudwarts/acf8df23a16afdb5837f)
    private static string DegreeToCardinalWindDir(double winddir)
    {
        string[] dirs = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"];
        int ix = (int)(winddir / (360 / dirs.Length));
        return dirs[ix % dirs.Length];
    }

    private static string GenerateDaypartHourlyTag(int timestamp)
    {
        int hour = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime().Hour;
        if (hour < 12)
        {
            if (hour < 6)
            {
                return "NITE";
            }
            else
            {
                return "MOR";
            }
        }
        else
        {
            if (hour < 18)
            {
                return "AFT";
            }
            else
            {
                if (hour < 23)
                {
                    return "EVE";
                }
                else
                {
                    return "NITE";
                }
            }
        }
    }

    private static string GenerateDaypartDayTag(int timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime().ToString("ddd");

    }

    private static string GenerateDaypartHourlyString(int timestamp)
    {
        int hour = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime().Hour;
        if (hour < 12)
        {
            if (hour < 6)
            {
                return "Midnight";
            }
            else
            {
                return "Morning";
            }
        }
        else
        {
            if (hour < 18)
            {
                return "Afternoon";
            }
            else
            {
                if (hour < 23)
                {
                    return "Evening";
                }
                else
                {
                    return "Midnight";
                }
            }
        }
    }

    public async Task<WeatherDataset> OpenMeteoWx(string api, bool keyenabled, string key, double latitude, double longitude, string name, string id)
    {
        try
        {
            HttpClient client = new();
            string url = api + $"/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min&hourly=temperature_2m,weather_code,is_day&current=temperature_2m,weather_code,surface_pressure,wind_speed_10m,wind_direction_10m,is_day&timezone=GMT&timeformat=unixtime&wind_speed_unit=mph&temperature_unit=fahrenheit&precipitation_unit=inch&forecast_hours=12";
            if (keyenabled)
            {
                api += $"&apikey={key}";
            }

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            OpenMeteo? wxData = JsonSerializer.Deserialize<OpenMeteo>(responseBody);
            if (wxData != null)
            {
                CurrentConditions ccond = new()
                {
                    WxInfo = new()
                    {
                        HeadendId = id,
                        Location = name,
                        Temperature = (int)wxData.current.temperature_2m,
                        Condition = wxData.current.is_day == 1 ? OpenMeteo2WxDay[wxData.current.weather_code] : OpenMeteo2WxNight[wxData.current.weather_code],
                        Pressure = wxData.current.surface_pressure / 33.8639,
                        WindSpeed = (int)wxData.current.wind_speed_10m,
                        WindDirection = DegreeToCardinalWindDir(wxData.current.wind_direction_10m)
                    }
                };
                ThreeDayForecast threeDay = new()
                {
                    WxInfo = [
                            new() {
                                HeadendId = id,
                                Location = name,
                                Temperature = (int)((wxData.daily.temperature_2m_max[0] + wxData.daily.temperature_2m_min[0]) / 2),
                                LowTemp = (int)wxData.daily.temperature_2m_min[0],
                                HighTemp = (int)wxData.daily.temperature_2m_max[0],
                                DaypartTag = GenerateDaypartDayTag(wxData.daily.time[0]),
                            },
                            new() {
                                HeadendId = id,
                                Location = name,
                                Temperature = (int)((wxData.daily.temperature_2m_max[1] + wxData.daily.temperature_2m_min[1]) / 2),
                                LowTemp = (int)wxData.daily.temperature_2m_min[1],
                                HighTemp = (int)wxData.daily.temperature_2m_max[1],
                                DaypartTag = GenerateDaypartDayTag(wxData.daily.time[1]),
                            },
                            new() {
                                HeadendId = id,
                                Location = name,
                                Temperature = (int)((wxData.daily.temperature_2m_max[2] + wxData.daily.temperature_2m_min[2]) / 2),
                                LowTemp = (int)wxData.daily.temperature_2m_min[2],
                                HighTemp = (int)wxData.daily.temperature_2m_max[2],
                                DaypartTag = GenerateDaypartDayTag(wxData.daily.time[2]),
                            },
                        ]
                };
                EighteenHourForecast eighteenHour = new()
                {
                    WxInfo = [
                            new() {
                                HeadendId = id,
                                Location = name,
                                LowTemp = (int)wxData.hourly.temperature_2m[0],
                                HighTemp = (int)wxData.hourly.temperature_2m[0],
                                DaypartTag = GenerateDaypartHourlyTag(wxData.hourly.time[0]),
                                DaypartStr = GenerateDaypartHourlyString(wxData.hourly.time[0]),
                                Condition = wxData.hourly.is_day[0] == 1 ? OpenMeteo2WxDay[wxData.hourly.weather_code[0]] : OpenMeteo2WxNight[wxData.hourly.weather_code[0]]
                            },
                            new() {
                                HeadendId = id,
                                Location = name,
                                LowTemp = (int)wxData.hourly.temperature_2m[3],
                                HighTemp = (int)wxData.hourly.temperature_2m[3],
                                DaypartTag = GenerateDaypartHourlyTag(wxData.hourly.time[3]),
                                DaypartStr = GenerateDaypartHourlyString(wxData.hourly.time[3]),
                                Condition = wxData.hourly.is_day[3] == 1 ? OpenMeteo2WxDay[wxData.hourly.weather_code[3]] : OpenMeteo2WxNight[wxData.hourly.weather_code[3]]
                            },
                            new() {
                                HeadendId = id,
                                Location = name,
                                LowTemp = (int)wxData.hourly.temperature_2m[7],
                                HighTemp = (int)wxData.hourly.temperature_2m[7],
                                DaypartTag = GenerateDaypartHourlyTag(wxData.hourly.time[7]),
                                DaypartStr = GenerateDaypartHourlyString(wxData.hourly.time[7]),
                                Condition = wxData.hourly.is_day[7] == 1 ? OpenMeteo2WxDay[wxData.hourly.weather_code[7]] : OpenMeteo2WxNight[wxData.hourly.weather_code[7]]
                            },
                            new() {
                                HeadendId = id,
                                Location = name,
                                LowTemp = (int)wxData.hourly.temperature_2m[11],
                                HighTemp = (int)wxData.hourly.temperature_2m[11],
                                DaypartTag = GenerateDaypartHourlyTag(wxData.hourly.time[11]),
                                DaypartStr = GenerateDaypartHourlyString(wxData.hourly.time[11]),
                                Condition = wxData.hourly.is_day[11] == 1 ? OpenMeteo2WxDay[wxData.hourly.weather_code[11]] : OpenMeteo2WxNight[wxData.hourly.weather_code[11]]
                            },
                        ]
                };
                return new()
                {
                    currentConditions = ccond,
                    threeDayForecast = threeDay,
                    eighteenHourForecast = eighteenHour
                };
            }
            else
            {
                Log.Error("Cannot parse Open Meteo data.");
                return new();
            }
        }
        catch (Exception ex)
        {
            Log.Error("Unable to grab Open Meteo data: " + ex.ToString());
            var edi = ExceptionDispatchInfo.Capture(ex);
            edi.Throw();
            return new();
        }
    }
}