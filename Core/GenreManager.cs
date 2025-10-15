using System;
using System.Collections.Concurrent;
using Core.Entities;

namespace Core;

public class GenreManager
{
    private static GenreManager? _instance = null;
    public static GenreManager Instance
    {
        get
        {
            if (_instance == null) _instance = new();
            return _instance;
        }
    }

    private ConcurrentBag<Genre> genres = [];
    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

    public async Task<List<Genre>> GetGenresFromStringAsync(string strGenres)
    {
        var array = strGenres
            .Split(',')
            .Select(genre => genre.TrimStart())
            .Select(genre => genre.TrimEnd())
            .ToList();

        var finalList = new List<Genre>();

        foreach (var item in array)
        {
            await semaphoreSlim.WaitAsync();
            var genre = genres.SingleOrDefault(genre => genre.GenreName.ToLower() == item.ToLower());
            if (genre == null)
            {
                genre = new Genre(item);
                genres.Add(genre);
            }
            semaphoreSlim.Release();

            finalList.Add(genre);
        }

        return finalList.ToList();
    }

    public void PrintAllGenres()
    {
        Console.WriteLine($"Present genres (Count: {genres.Count()})");
        foreach (var genre in genres.OrderBy(g => g.GenreName).ToList())
        {
            Console.WriteLine($"\t{genre.GenreName}");
        }
    }

    public Dictionary<Genre,int> GetMenuGenres()
    {
        var pool = genres.ToList();
        var chosenGenres = new Dictionary<Genre, int>();
        var permanentGenres = new string[]
        {
            "Animation",
            "Sci-Fi",
            "Comedy",
            "Sword & Sorcery",
            "Jungle Adventure",
            "Anime"
        };

        Random random = new Random();
        for (int x = 0; x < 9; x++)
        {
            Genre? genre;
            if (x < permanentGenres.Length) genre = pool.SingleOrDefault(i => i.GenreName == permanentGenres[x]);
            else
            {
                var randomIndex = random.Next(0, pool.Count());
                genre = pool.ElementAtOrDefault(randomIndex);
            }

            if (genre != null)
            {
                pool.Remove(genre);
                chosenGenres.Add(genre, x + 1);
            }
        }

        return chosenGenres;
        
    }
}

