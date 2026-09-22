using Encode2It.Encoders;
using Encode2It.Core;
using Encode2It.Schemas.Inputs.Listing;
using System.Text.Json;
using XmlTvSharp;
using System.Runtime.ExceptionServices;
using XmlTvSharp.Models;
using AnyAscii;

namespace Encode2It.Inputs;

public class ListingsInputs
{
    private readonly Logger Log = new("Inputs - Listing");
    private string StringCleaner(string str)
    {
        // Remove newlines and |. Transliterate Unicode characters.
        return str.Replace("|", "").Replace(Environment.NewLine, "").Transliterate();
    }

    private List<Listing> XMLTVProcess(XmlTvDocument result)
    {
        if (result != null)
        {
            Dictionary<string, string[]> channels = new();
            channels[string.Empty] = ["0", "UNKN"];

            int number = 1;

            foreach (XmlTvChannel channel in result.Channels)
            {
                bool foundNum = false;
                bool foundID = false;
                string channelId = (channel.Id.Split(".").FirstOrDefault() ?? "").ToUpper();
                int num = number;
                if (channel.DisplayNames.Count() > 0)
                {
                    foreach (XmlTvLocalizedText xmlTv in channel.DisplayNames)
                    {
                        string[] spiltText = xmlTv.Value.Split(" ");

                        if (spiltText.Length >= 2)
                        {
                            if (!foundNum)
                            {
                                if (Int32.TryParse(spiltText[0], out num))
                                {
                                    foundNum = true;
                                    foundID = true;
                                    channelId = spiltText[1].Replace(" ", "").ToUpper();
                                }
                                else
                                {
                                    foundID = true;
                                    channelId = xmlTv.Value.Replace(" ", "").ToUpper();
                                }
                            }

                        }
                        else
                        {
                            int tempNum = 0;
                            if (Int32.TryParse(xmlTv.Value, out tempNum))
                            {
                                if (!foundNum)
                                {
                                    foundNum = true;
                                    num = tempNum;
                                }

                            }
                            else
                            {
                                if (!foundID)
                                {
                                    channelId = xmlTv.Value.Replace(" ", "").ToUpper();
                                }
                            }
                        }

                    }
                }

                channelId = channelId.Length > 6 ? channelId.Substring(0, 6) : channelId;

                string[] names = [num.ToString(), channelId];
                channels[channel.Id] = names;
                if (!foundNum)
                {
                    number += 1;
                }

            }

            List<string> channelswprograms = [];

            List<Listing> listings = [];
            foreach (XmlTvProgramme program in result.Programmes)
            {
                string zap2it_epi = "";
                string? onscreen = null;
                int episodeNum = -1;

                foreach (XmlTvEpisodeNumber epiNum in program.EpisodeNumbers)
                {
                    if (epiNum.System == "xmltv_ns")
                    {
                        if (episodeNum == -1)
                        {
                            string[] splitnum = (epiNum.Value ?? "").Split("/").First().Split(".");
                            episodeNum = splitnum.Length >= 2 ? Convert.ToInt32(splitnum[0]) + 1 : 0;
                        }
                    }
                    else if (epiNum.System == "dd_progid")
                    {
                        if (zap2it_epi == "")
                        {
                            zap2it_epi = epiNum.Value ?? "";
                        }
                    }
                    else if (epiNum.System == "onscreen")
                    {
                        onscreen = epiNum.Value;
                    }
                }

                if (episodeNum == -1)
                {
                    episodeNum = 0;
                }

                string[]? starratingfrac = program.StarRatings?.FirstOrDefault()?.Value.Split("/") ?? null;
                float? starrating = null;
                if (starratingfrac != null)
                {
                    starrating = starratingfrac.Length == 2 ? Convert.ToInt32(starratingfrac[0]) / Convert.ToInt32(starratingfrac[1]) * 5 : 0;
                }

                string title = StringCleaner(program.Titles.FirstOrDefault()?.Value ?? "Unknown Program") + (onscreen != null ? ": " + onscreen : "");

                listings.Add(new()
                {
                    ChannelNumber = Convert.ToInt16(channels[program.ChannelId][0]),
                    Callsign = StringCleaner(channels[program.ChannelId][1]),
                    Time = program.Start.ToDateTimeOffset().UtcDateTime,
                    Duration = (int)(program.Length?.Value ?? (int)(program.Stop != null ? (program.Stop.ToDateTimeOffset() - program.Start.ToDateTimeOffset()).TotalSeconds : 60)),
                    Titles = [title, "", "", "", ""],
                    RatingA = StringCleaner(program.Ratings?.Count > 0 ? (program.Ratings.FirstOrDefault()?.Value.ToString() ?? "").Replace("|", "") : "UR"),
                    Subtitle = StringCleaner(program.SubTitles.FirstOrDefault()?.Value ?? ""),
                    Description = StringCleaner(program.Descriptions.FirstOrDefault()?.Value ?? ""),
                    Country = StringCleaner(program.Countries.FirstOrDefault()?.Value ?? ""),
                    Category = StringCleaner(program.Categories.FirstOrDefault()?.Value ?? ""),
                    StarRating = (int)(starrating ?? 0),
                    Episode = StringCleaner(episodeNum.ToString()),
                });

                if (!channelswprograms.Contains(program.ChannelId))
                {
                    channelswprograms.Add(program.ChannelId);
                }
            }

            // Add listings for channels without programs
            foreach (XmlTvChannel channel in result.Channels)
            {
                if (!channelswprograms.Contains(channel.Id))
                {
                    listings.Add(new()
                    {
                        ChannelNumber = Convert.ToInt16(channels[channel.Id][0]),
                        Callsign = StringCleaner(channels[channel.Id][1]),
                        Duration = 72000,
                        Titles = [
                            StringCleaner(channel.DisplayNames.FirstOrDefault()?.Value ?? channels[channel.Id][1]),
                            "",
                            "",
                            StringCleaner(channels[channel.Id][1]),
                            ""
                        ],
                        Subtitle = "",
                        RatingA = "",
                        ProgramType = ListingTypes.Invisible,
                        Description = "",
                        Category = ""
                    });
                }

            }

            return listings;
        }
        else
        {
            Log.Error("Unable to parse XMLTV");
            return [];
        }
    }

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
                        Callsign = StringCleaner(publicChannel.channel_id.ToUpper()),
                        Duration = 72000,
                        Titles = [
                            StringCleaner(publicChannel.title ?? publicChannel.channel_id.ToUpper()),
                            "",
                            "",
                            StringCleaner(publicChannel.channel_id.ToUpper()),
                            ""
                        ],
                        Subtitle = "",
                        RatingA = "",
                        ProgramType = type,
                        Description = StringCleaner(publicChannel.channel_description ?? ""),
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

    public async Task<List<Listing>> Zap2ItDelimited(string path)
    {
        try
        {
            string contents = File.ReadAllText(path);

            Delimited publicChannels = new Delimited();
            publicChannels.Read(contents);

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

    public async Task<List<Listing>> Zap2ItDelimitedURL(string api)
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

        // Read all TV channels and programmes asynchronously
        var result = await XmlTvReader.ReadAsync(path, new XmlTvReaderOptions { UnknownElementHandling = XmlTvUnknownContentHandling.Ignore, UnknownAttributeHandling = XmlTvUnknownContentHandling.Ignore });

        return XMLTVProcess(result);

    }

    public async Task<List<Listing>> XMLTVHTTP(string url)
    {
        // Code taken from example: https://github.com/eddami/XmlTvSharp/tree/main

        // Grab XMLTV
        HttpClient http = new();
        var response = await http.GetAsync(url);
        StringReader reader = new(await response.Content.ReadAsStringAsync());

        // Read all TV channels and programmes asynchronously
        var result = await XmlTvReader.ReadAsync(reader, new XmlTvReaderOptions { UnknownElementHandling = XmlTvUnknownContentHandling.Ignore, UnknownAttributeHandling = XmlTvUnknownContentHandling.Ignore });


        return XMLTVProcess(result);
    }
}
