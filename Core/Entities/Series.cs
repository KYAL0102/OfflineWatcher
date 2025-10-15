namespace Core.Entities;

public class Series : StreamItem
{
    public List<Video> Episodes { get; } = [];
    public double IMDbRating { get; set; } = -1;
    public int IMDbReviewAmout { get; set; } = -1;
}