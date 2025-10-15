namespace Core.Entities;

public class Movie : StreamItem
{
    public string NameInCurrentLanguage => VideoOfMovie.Names[Globals.CurrentLanguage];
    public Video VideoOfMovie { get; set; } = new();
    public List<Video> Extras { get; } = [];
    public int YearOfRelease { get; set; } = -1;
    public double IMDbRating { get; set; } = -1;
    public int IMDbReviewAmout { get; set; } = -1;
}