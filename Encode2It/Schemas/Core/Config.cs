using System.Xml.Serialization;
using System.Xml;

namespace Encode2It.Schemas.Core;

[XmlRoot(ElementName = "ListingInput")]
public class ListingInputConfigClass
{
    [XmlAnyElement(Name = "TypeComment")]
    public XmlComment TypeComment { get { return new XmlDocument().CreateComment("\n            This sets the type of input this is.\n            Values:\n              - mist_v1\n              - xmltv\n              - zap2it\n        "); } set { } }

    [XmlElement(ElementName = "Type")]
    public string Type { get; set; } = "INSERT_TYPE_HERE";

    [XmlAnyElement(Name = "ValueComment")]
    public XmlComment ValueComment { get { return new XmlDocument().CreateComment("\n            This sets the value of the type (ex: api url, path to file, etc).\n            Examples:\n              - mist_v1:\n                - https://api.mistweather.com/api/public-channels\n              - xmltv:\n                - ./xmltv.xml\n              - zap2it:\n                - https://example.com/delimited.del\n        "); } set { } }

    [XmlElement(ElementName = "Value")]
    public string Value { get; set; } = "INSERT_VALUE_HERE";

    [XmlAnyElement(Name = "KeyEnabledComment")]
    public XmlComment KeyEnabledComment { get { return new XmlDocument().CreateComment("If set to true, the supplied API key is sent to the provider (DEPENDS ON TYPE)"); } set { } }

    [XmlElement(ElementName = "KeyEnabled")]
    public bool KeyEnabled { get; set; } = false;

    [XmlAnyElement(Name = "KeyComment")]
    public XmlComment KeyComment { get { return new XmlDocument().CreateComment("This sets the API key, and will be sent if KeyEnabled is set to true. (DEPENDS ON TYPE)"); } set { } }

    [XmlElement(ElementName = "Key")]
    public string Key { get; set; } = "INSERT_KEY_HERE";
}

[XmlRoot(ElementName = "WeatherInput")]
public class WeatherInputConfigClass
{

    [XmlAnyElement(Name = "TypeComment")]
    public XmlComment TypeComment
    {
        get
        {
            return new XmlDocument().CreateComment(
        """

                  This sets the type of input this is.
                  Values:
                    - openmeteo
              
        """
    );
        }
        set { }
    }

    [XmlElement(ElementName = "Type")]
    public string Type { get; set; } = "INSERT_TYPE_HERE";

    [XmlAnyElement(Name = "LocationNameComment")]
    public XmlComment LocationNameComment { get { return new XmlDocument().CreateComment("The location name as shown on the forecasts."); } set { } }

    [XmlElement(ElementName = "LocationName")]
    public string LocationName { get; set; } = "INSERT_LOCATION_NAME_HERE";

    [XmlAnyElement(Name = "ValueComment")]
    public XmlComment ValueComment
    {
        get
        {
            return new XmlDocument().CreateComment(
        """

                  This sets the value of the type (ex: api url, path to file, etc).
                  Examples:
                    - openmeteo:
                      - https://api.open-meteo.com (if non-commercial)
                      - https://customer-api.open-meteo.com (if commercial)
              
        """
    );
        }
        set { }
    }

    [XmlElement(ElementName = "Value")]
    public string Value { get; set; } = "INSERT_VALUE_HERE";

    [XmlAnyElement(Name = "GeocodeComment")]
    public XmlComment GeocodeComment { get { return new XmlDocument().CreateComment("Sets where to get weather data for."); } set { } }

    [XmlElement(ElementName = "Latitude")]
    public double Latitude { get; set; } = 0.0;

    [XmlElement(ElementName = "Longitude")]
    public double Longitude { get; set; } = 0.0;

    [XmlAnyElement(Name = "KeyEnabledComment")]
    public XmlComment KeyEnabledComment { get { return new XmlDocument().CreateComment("If set to true, the supplied API key is sent to the provider (DEPENDS ON TYPE)"); } set { } }

    [XmlElement(ElementName = "KeyEnabled")]
    public bool KeyEnabled { get; set; } = false;

    [XmlAnyElement(Name = "KeyComment")]
    public XmlComment KeyComment { get { return new XmlDocument().CreateComment("This sets the API key, and will be sent if KeyEnabled is set to true. (DEPENDS ON TYPE)"); } set { } }

    [XmlElement(ElementName = "Key")]
    public string Key { get; set; } = "INSERT_KEY_HERE";
}

[XmlRoot(ElementName = "ListingInputs")]
public class ListingInputsClass
{
    [XmlElement(ElementName = "ListingInput")]
    public ListingInputConfigClass[] ListingInputs { get; set; } = [new()];
}

[XmlRoot(ElementName = "InputConfig")]
public class InputConfigClass
{
    [XmlAnyElement(Name = "WeatherInputComment")]
    public XmlComment WeatherInputComment { get { return new XmlDocument().CreateComment("This sets the provider for the weather data provided by this encoder. Only one can be set."); } set { } }

    [XmlElement(ElementName = "WeatherInput")]
    public WeatherInputConfigClass Weather { get; set; } = new();

    [XmlAnyElement(Name = "ListingInputComment")]
    public XmlComment ListingInputComment { get { return new XmlDocument().CreateComment("This sets the provider(s) for the listing data provided by this encoder. Multiple can be set."); } set { } }

    [XmlElement(ElementName = "ListingInputs")]
    public ListingInputsClass ListingInputs { get; set; } = new();
}

[XmlRoot(ElementName = "HeadendConfig")]
public class HeadendConfigClass
{
    [XmlAnyElement(Name = "IdComment")]
    public XmlComment IdComment { get { return new XmlDocument().CreateComment("This sets the ID of the Zap2It headend. Do not change if you don't know what you're doing."); } set { } }

    [XmlElement(ElementName = "Id")]
    public string Id { get; set; } = "ZAP2IT";

    [XmlAnyElement(Name = "PathComment")]
    public XmlComment PathComment { get { return new XmlDocument().CreateComment("This is where the Zap2It program is installed. Do not change if you don't know what you're doing."); } set { } }

    [XmlElement(ElementName = "Path")]
    public string Path { get; set; } = "C:\\ZAP2IT\\";

}

[XmlRoot(ElementName = "TimingConfig")]
public class TimingConfigClass
{
    [XmlAnyElement(Name = "ListingIntervalComment")]
    public XmlComment ListingIntComment { get { return new XmlDocument().CreateComment("This defines the interval between each time Encode2It generates listings in milliseconds."); } set { } }
    [XmlElement(ElementName = "ListingInterval")]
    public int ListingInt = 300000;

    [XmlAnyElement(Name = "WeatherIntervalComment")]
    public XmlComment WeatherIntComment { get { return new XmlDocument().CreateComment("This defines the interval between each time Encode2It generates weather data in milliseconds."); } set { } }
    [XmlElement(ElementName = "WeatherInterval")]
    public int WeatherInt = 3600000;
}

[XmlRoot(ElementName = "LogConfig")]
public class LogConfigClass
{
    [XmlAnyElement(Name = "LogLevelComment")]
    public XmlComment LogLevelComment { get { return new XmlDocument().CreateComment("This sets the log level. 0 for Debug, 1 for Info, 2 for Warning, 3 for Error."); } set { } }
    [XmlElement(ElementName = "LogLevel")]
    public int LogLevel = 3;
}

[XmlRoot(ElementName = "Encode2ItConfig")]
public class ConfigSchema
{
    [XmlAnyElement(Name = "IntroComment")]
    public XmlComment IntroComment { get { return new XmlDocument().CreateComment("This is the Encode2It config file. For more information, check the wiki."); } set { } }
    [XmlAnyElement(Name = "HeadendComment")]
    public XmlComment HeadendComment { get { return new XmlDocument().CreateComment("This is where headend-specific config data is set."); } set { } }

    [XmlElement(ElementName = "HeadendConfig")]
    public HeadendConfigClass HeadendConfig { get; set; } = new();

    [XmlAnyElement(Name = "InputComment")]
    public XmlComment InputComment { get { return new XmlDocument().CreateComment("This is where input-specific config data is set."); } set { } }

    [XmlElement(ElementName = "InputConfig")]
    public InputConfigClass InputConfig { get; set; } = new();

    [XmlAnyElement(Name = "TimingComment")]
    public XmlComment TimingComment { get { return new XmlDocument().CreateComment("This is where timing-specific config data is set."); } set { } }

    [XmlElement(ElementName = "TimingConfig")]
    public TimingConfigClass TimingConfig { get; set; } = new();

    [XmlAnyElement(Name = "LogComment")]
    public XmlComment LogComment { get { return new XmlDocument().CreateComment("This is where log-specific config data is set."); } set { } }

    [XmlElement(ElementName = "LogConfig")]
    public LogConfigClass LogConfig { get; set; } = new();
}