﻿using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Core.Entities;

public class StreamItem
{
    public string NameInCurrentLanguage => Names[Globals.CurrentLanguage];
    public virtual Dictionary<Language, string> Names { get; init; } = [];
    public string ImagePath { get; set; } = string.Empty;
    public List<Genre> Genres { get; } = [];
}