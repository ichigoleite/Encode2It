# Encode2It

![Encode2It Logo](https://raw.githubusercontent.com/ichigoleite/Encode2It/refs/heads/main/Branding/Encode2ItLogo.svg)

An encoder for the Zap2It cable guide.

## Operating System Support

Encode2It is built with .NET 10 - meaning any Zap2It instances that are using operating systems older than Windows 10 will NOT work.  
This is due to that Zap2It CAN run on modern operating systems (e.g Debian 13 Trixie w. Wine), and I didn't want to limit myself to older .NET versions like .NET Framework 4.0  
(which also makes development more difficult due to myself using Linux, and that debug would not be very straightforward unlike with .NET 5 and up.)  
I do plan on making a "Legacy" version that would have support for older operating systems (e.g Windows XP).  
For more information, check out [.NET 10's operating system support](https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md).

## Roadmap

- Delimiter file generation: Done!
- Encoders for Listings/Weather: Done!
- Inputs: Done!
- Loop: Done!

## Supported Inputs

- Listings:
  - XMLTV
  - Mist Streaming
- Weather:
  - Open-Meteo

## Credits

- pajamafrix for research, sources:
  - [Delimited Schema](https://park-city.club/~frix/oncable/delimited-schema.html)
