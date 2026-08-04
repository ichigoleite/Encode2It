namespace Encode2It.Core;

public class Delimited
{
    // An array of string arrays. Each string array is a line, and each line contains values.
    public string[][] Lines { get; set; } = [];

    // Read delimited.
    public void Read(string content)
    {
        List<string[]> lines = [];

        foreach (string lineString in content.Split("\n"))
        {
            List<string> line = [];
            foreach (string value in lineString.Split("|"))
            {
                line.Add(value);
            }
            lines.Add([.. line]);
        }
        Lines = [.. lines];

    }

    // Generate!
    public string Generate()
    {
        // The content.
        string content = "";

        // For each line, generate a line.
        foreach (string[] line in Lines)
        {
            string lineContent = "";

            // Add each value to the line.
            foreach (string value in line)
            {
                lineContent += value + "|";
            }

            // Once that's done, add a newline.
            lineContent += "\n";

            // Add line's content to file content.
            content += lineContent;
        }

        // Return content.
        return content;
    }
}