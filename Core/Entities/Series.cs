namespace Core.Entities;

public class Series : StreamItem
{
    public string NameInCurrentLanguage => Names[Globals.CurrentLanguage];
    public Dictionary<Language, string> Names { get; } = [];
    public List<Video> Episodes { get; } = [];
    
}