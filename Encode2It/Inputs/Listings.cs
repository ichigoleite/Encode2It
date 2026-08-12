using Encode2It.Encoders;
using Encode2It.Core;
using Encode2It.Schemas.Inputs.Listing;
using System.Text.Json;
using XmlTvSharp;
using System.Runtime.ExceptionServices;

namespace Encode2It.Inputs;

public class ListingsInputs
{
    private readonly Logger Log = new("Inputs - Listing");
    public async Task<List<Listing>> MistStreaming(string api)
    {
        try
        {
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(api);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            MistStreamingPublicChannel[]? publicChannels = JsonSerializer.Deserialize<MistStreamingPublicChannel[]>(responseBody);
            if (publicChannels != null)
            {
                List<Listing> listings = [];
                Random rnd = new();
                foreach (MistStreamingPublicChannel publicChannel in publicChannels)
                {
                    ListingTypes type = ListingTypes.Invisible;
                    if (publicChannel.is_public)
                    {
                        if (publicChannel.channel_status == "online")
                        {
                            if (publicChannel.channel_category != null)
                            {
                                if (publicChannel.channel_category.Any("Movie".Contains))
                                {
                                    type = ListingTypes.Movies;
                                }
                                else if (publicChannel.channel_category.Any("News".Contains))
                                {
                                    type = ListingTypes.News;
                                }
                                else if (publicChannel.channel_category.Any("Sports".Contains))
                                {
                                    type = ListingTypes.Sports;
                                }
                                else if (publicChannel.channel_category.Any("Information".Contains))
                                {
                                    type = ListingTypes.News;
                                }
                                else if (publicChannel.channel_category.Any("Shopping".Contains))
                                {
                                    type = ListingTypes.News;
                                }
                                else if (publicChannel.channel_category.Any("Weather".Contains))
                                {
                                    type = ListingTypes.News;
                                }
                                else
                                {
                                    type = ListingTypes.Default;
                                }
                            }
                            else
                            {
                                type = ListingTypes.Default;
                            }
                        }
                    }
                    listings.Add(new()
                    {
                        ChannelNumber = publicChannel.channel_number ?? -1,
                        Callsign = publicChannel.channel_id.ToUpper(),
                        Duration = 72000,
                        Titles = [
                            (publicChannel.title ?? publicChannel.channel_id.ToUpper()),
                            "",
                            "",
                            publicChannel.channel_id.ToUpper(),
                            ""
                        ],
                        Subtitle = "",
                        RatingA = "",
                        ProgramType = type,
                        Description = publicChannel.channel_description ?? "",
                        Category = ((publicChannel.channel_category ?? [""]).Length == 0 ? [""] : (publicChannel.channel_category ?? [""]))[0]
                    });
                }

                return listings;
            }
            else
            {
                Log.Error("Cannot parse Mist Streaming data.");
                return [];
            }
        }
        catch (Exception ex)
        {
            Log.Error("Unable to grab Mist Streaming data: " + ex.ToString());
            var edi = ExceptionDispatchInfo.Capture(ex);
            edi.Throw();
            return [];
        }
    }

    public async Task<List<Listing>> Zap2ItDelimited(string api)
    {
        try
        {
            HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(api);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Delimited publicChannels = new Delimited();
            publicChannels.Read(responseBody);

            Listings listings = new();
            listings.Read(publicChannels);

            return listings.Listing;

        }
        catch (Exception ex)
        {
            Log.Error("Unable to grab Zap2It data: " + ex.ToString());
            var edi = ExceptionDispatchInfo.Capture(ex);
            edi.Throw();
            return [];
        }
    }

    public async Task<List<Listing>> XMLTV(string path)
    {
        // Code taken from example: https://github.com/eddami/XmlTvSharp/tree/main
        // Cancellation token
        var cancellationToken = new CancellationToken();

        // Customize the parsing behaviour
        var settings = new XmlTvReaderSettings();

        // Read all TV channels and programmes asynchronously
        var result = await XmlTvReader.ReadAllAsync(path, settings, cancellationToken);

        if (result != null)
        {
            Dictionary<string, string[]> channels = new();
            channels[string.Empty] = ["0", "UNKN"];

            foreach (XmlTvChannel channel in result.Channels)
            {
                string[] names = ["0", "UNKN"];
                foreach (string? name in channel.DisplayNames.Values)
                {
                    if (name != null)
                    {
                        string[] split = name.Split(" ");
                        if (split.Length == 2)
                        {
                            if (Convert.ToDouble(split[0]) != 0)
                            {
                                names[0] = split[0];
                            }
                            names[1] = split[1];
                        }
                        else
                        {
                            names[1] = name;
                        }
                    }
                }
                channels[channel.Id] = names;
                Console.WriteLine(channel.Id + ": " + names[0] + "/" + names[1]);
            }

            List<Listing> listings = [];
            foreach (XmlTvProgramme program in result.Programmes)
            {
                int onscreen_epi = 0;
                string zap2it_epi = "";
                foreach (XmlTvEpisode episode in program.Episodes ?? [])
                {
                    if (episode.System == "onscreen")
                    {
                        onscreen_epi = Convert.ToInt16(episode.Value);
                    }
                    else if (episode.System == "dd_progid")
                    {
                        zap2it_epi = episode.Value ?? "";
                    }
                }
                listings.Add(new()
                {
                    ChannelNumber = Convert.ToInt16(channels[program.ChannelId][0]),
                    Callsign = channels[program.ChannelId][1],
                    Time = program.Start.DateTime,
                    Duration = (int)(program.Stop.ToUnixTimeSeconds() - program.Start.ToUnixTimeSeconds()),
                    Titles = [program.Titles.Values.First(), "", "", "", ""],
                    Subtitle = (program.SubTitles ?? new Dictionary<string, string>() { { "en", "" } }).Values.First(),
                    Description = (program.Descriptions ?? new Dictionary<string, string>() { { "en", "" } }).Values.First(),
                    Actor = string.Join(", ", program.Actors ?? [""]),
                    Country = (program.Countries ?? [""]).First(),
                    Category = (program.Categories ?? [""]).First(),
                    StarRating = (int)Convert.ToDouble(program.StarRating ?? "0"),
                    Episode = onscreen_epi.ToString(),
                    TMSId = zap2it_epi
                });
            }

            return listings;
        }
        else
        {
            Log.Error("Unable to parse XMLTV");
            return [];
        }

    }
}
