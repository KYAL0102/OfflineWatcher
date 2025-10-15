namespace Core.Entities;

public class Video : StreamItem
{
    public string PathToVideoFile { get; set; } = string.Empty;

    public int DurationInMinutes { get; set; } = -1;
}