﻿using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Core;

public static class ImageHelper
{
    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        Console.WriteLine(resourceUri.AbsoluteUri);
        return new Bitmap(AssetLoader.Open(resourceUri));
    }

    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}