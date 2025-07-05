namespace Core.Entities;

public class StreamItem
{
    public string ImagePath { get; set; } = string.Empty;
    public List<Genre> Genres { get; } = [];
}