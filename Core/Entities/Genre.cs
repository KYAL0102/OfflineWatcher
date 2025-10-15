namespace Core.Entities;

public class Genre
{
    public string GenreName { get; } = string.Empty;

    public Genre(string name)
    {
        GenreName = name;
    }
}
