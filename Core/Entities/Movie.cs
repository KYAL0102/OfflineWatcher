namespace Core.Entities;

public class Movie : StreamItem
{
    public string NameInCurrentLanguage => VideoOfMovie.Names[Globals.CurrentLanguage];
    public Video VideoOfMovie { get; set; } = new();
    public List<Video> Extras { get; } = [];
}