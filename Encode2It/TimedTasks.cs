using System.Runtime.CompilerServices;
using Encode2It.Core;
using Encode2It.Encoders;
using Encode2It.Inputs;
using Encode2It.Schemas.Core;

namespace Encode2It;

public class TimedTasks
{
    Config config;

    public TimedTasks(Config configobj)
    {
        config = configobj;
        // Make sure directories exist.
        Directory.CreateDirectory(config.config.HeadendConfig.Path);
        Directory.CreateDirectory(Path.Join(config.config.HeadendConfig.Path, "/OnCable/"));
        Directory.CreateDirectory(Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT/"));
        Directory.CreateDirectory(Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT/" + config.config.HeadendConfig.Id));
    }

    public async Task ListingLoop()
    {
        // Make new ListingsInputs.
        ListingsInputs listingsInputs = new();

        // Make new logger.
        Logger logger = new("TimedTasks - ListingLoop");

        logger.Info("Starting listing loop.");

        while (true)
        {
            logger.Info("Listing loop start!");

            HttpClient client = new();
            string content = await client.GetStringAsync("https://api.mistweather.com/api/delimited.del");

            logger.Info("Writing down data...");
            // Now write!
            string path = Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT", config.config.HeadendConfig.Id, DateTime.Now.ToString("MMddyyyy") + ".del");
            logger.Debug("Path: " + path);

            logger.Debug("File Content:\n" + content);

            File.WriteAllText(path, content);
            logger.Info("Wrote down!");
            logger.Info($"Now waiting for {config.config.TimingConfig.ListingInt} ms. for next loop...");

            await Task.Delay(config.config.TimingConfig.ListingInt);
        }
    }

    public async Task WeatherLoop()
    {
        // Make new WeatherInputs.
        WeatherInputs weatherInputs = new();

        // Make new logger.
        Logger logger = new("TimedTasks - WeatherLoop");

        // Enabled bool
        bool enabled = true;

        // Check weather input
        WeatherInputConfigClass weatherInput = config.config.InputConfig.Weather;

        logger.Info("Starting weather loop.");

        while (enabled)
        {
            logger.Info("Weather loop start!");
            WeatherDataset weatherDataset = new();

            if (weatherInput.Type == "openmeteo")
            {
                logger.Info("Generating weather data from Open-Meteo input...");
                weatherDataset = await weatherInputs.OpenMeteoWx(weatherInput.Value, weatherInput.KeyEnabled, weatherInput.Key, weatherInput.Latitude, weatherInput.Longitude);
                logger.Info("Finished generating weather data from Open-Meteo input.");
            }
            else
            {
                enabled = false;
                logger.Warn($"Unknown weather type {weatherInput.Type}! Stopping weather data generation...");
            }

            // Now write!
            logger.Info("Writing down current conditions data...");
            string path = Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT", config.config.HeadendConfig.Id, "uscur.txt");
            logger.Debug("Path: " + path);
            File.WriteAllText(path, weatherDataset.currentConditions.Generate());
            logger.Info("Wrote down current conditions data!");

            logger.Info("Writing down 3 day forecast data...");
            path = Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT", config.config.HeadendConfig.Id, "us3day.txt");
            logger.Debug("Path: " + path);
            File.WriteAllText(path, weatherDataset.threeDayForecast.Generate());
            logger.Info("Wrote down 3 day forecast data!");

            logger.Info("Writing down 18 hour forecast data...");
            path = Path.Join(config.config.HeadendConfig.Path, "/OnCable/EXPORT", config.config.HeadendConfig.Id, "18hour.txt");
            logger.Debug("Path: " + path);
            File.WriteAllText(path, weatherDataset.eighteenHourForecast.Generate());
            logger.Info("Wrote down 18 hour forecast data!");

            logger.Info($"Now waiting for {config.config.TimingConfig.WeatherInt} ms. for next loop...");
            await Task.Delay(config.config.TimingConfig.WeatherInt);
        }
    }
}