using Core.Entities;

namespace Core;

public static class Globals
{
    public static StreamItem? CurrentStreamItem { get; set; } = null;
    public static List<Movie> Movies = new List<Movie>();
    public static List<Series> Series = new List<Series>();
    public static Language CurrentLanguage { get; set; } = Language.English;
    public static string NavigateToMenuAction { get; set; } = "menu";
    public static string NavigateToMoviePageAction { get; set; } = "movie";
    public static string NavigateToSeriesPageAction { get; set; } = "series";
    public static string NavigateToVideoFullscreenAction { get; set; } = "video";
}