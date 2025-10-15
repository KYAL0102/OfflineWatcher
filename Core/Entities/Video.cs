namespace Core.Entities;

public class Video
{
    public Dictionary<Language, string> Names { get; } = [];
    
    public string CurrentName => Names[Globals.CurrentLanguage];

    public string PathToVideoFile { get; set; } = string.Empty;

    public int DurationInMinutes { get; set; } = -1;
}