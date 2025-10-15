
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Core.Entities;

namespace Core
{
    public static class ImportController
    {
        public static string RootDirectory
        {
            get
            {
                var currentPath = Environment.CurrentDirectory;
                if (OperatingSystem.IsWindows())
                {
                    #if DEBUG
                        return "D:";
                    #elif RELEASE
                        return Path.GetPathRoot(currentPath) ?? "D:";
                    #endif
                }
                if (OperatingSystem.IsLinux())
                {
                    var result = GetMountPoint(currentPath);
                    if (result != null) return result;
                }

                return "";
            }
        }
        private static string ThumbnailDirectory => Path.Combine(RootDirectory, "Video", "Thumbnails");
        private static string MovieDirectory => Path.Combine(RootDirectory, "Video", "Movies");
        private static string SeriesDirectory => Path.Combine(RootDirectory, "Video", "Series");
        
        public static async Task<List<Movie>> GetAllMoviesAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(25, 25);
            var list = new ConcurrentBag<Movie>();
            
            if (!Directory.Exists(MovieDirectory))
            {
                Console.WriteLine($"Directory {MovieDirectory} does not exist.");
                return [];
            }
            
            var directories = Directory.GetDirectories(MovieDirectory,string.Empty, SearchOption.TopDirectoryOnly);
            
            Console.WriteLine($"{directories.Length} directories (supposed movies) found");
            foreach (var dir in directories)
            {
                await semaphore.WaitAsync();
                
                var task = Task.Run(async () =>
                {
                    try
                    {
                        var movie = new Movie();
                        var showInfoFilePath = Path.Combine(dir, "info.csv");

                        if (!File.Exists(showInfoFilePath))
                        {
                            Console.WriteLine($"No info file found for {dir}");
                            return;
                        }

                        //Console.WriteLine($"Looking for {showInfoFilePath}...");
                        var movieDetails = (await File.ReadAllLinesAsync(showInfoFilePath))
                            .Skip(1)
                            .Select(line => line.Split(";"))
                            .ToList();

                        if (movieDetails.Select(d => d[3]).Count(d => d == "false") == 1)
                        {
                            foreach (var detail in movieDetails)
                            {
                                var video = new Video
                                {
                                    PathToVideoFile = Path.Combine(dir, $"{detail[0]}.mkv")
                                };
                                video.Names.Add(Language.English, detail[1]);
                                video.Names.Add(Language.German, detail[2]);

                                if (detail[3] == "false") await ExtractMovieDataAsync(movie, detail, video);
                                else movie.Extras.Add(video);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"info.csv file needs to have exactly 1 row entry where Extra is false. ({showInfoFilePath})");
                        }
                
                        list.Add(movie);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();
            Console.WriteLine($"Extracted {list.Count} movies in {stopwatch.ElapsedMilliseconds / 1000f}s!");
            //GenreManager.Instance.PrintAllGenres();

            return list.ToList();
        }

        private async static Task ExtractMovieDataAsync(Movie movie, string[] detail, Video video)
        {
            movie.VideoOfMovie = video;

            //Genres
            var genres = await GenreManager.Instance.GetGenresFromStringAsync(detail[4]);
            movie.Genres.AddRange(genres);

            //Thumbnail-Path
            var imagePath = Path.Combine(ThumbnailDirectory, detail[5]);
            movie.ImagePath = imagePath;

            //Movie-Duration
            var success = int.TryParse(detail[6], out int durationInMinutes);
            if (success) movie.VideoOfMovie.DurationInMinutes = durationInMinutes;
            else Console.WriteLine($"Parsing of the Movie-Duration for {movie.NameInCurrentLanguage} not possible! -> input: {detail[6]}");

            //Year of Release
            success = int.TryParse(detail[7], out var yearOfRelease);
            if (success) movie.YearOfRelease = yearOfRelease;
            else Console.WriteLine($"Parsing of {nameof(movie.YearOfRelease)} for {movie.NameInCurrentLanguage} not possible! -> input: {detail[7]}");

            //IMDb-Rating
            success = double.TryParse(detail[8], out var rating);
            if (success) movie.IMDbRating = rating;
            else Console.WriteLine($"Parsing of {nameof(movie.IMDbRating)} for {movie.NameInCurrentLanguage} not possible! -> input: {detail[8]}");

            //IMDb-Reviews
            success = double.TryParse(detail[9], out var reviewAmount);
            if (success) movie.IMDbRating = rating;
            else Console.WriteLine($"Parsing of {nameof(movie.IMDbReviewAmout)} for {movie.NameInCurrentLanguage} not possible! -> input: {detail[9]}");

        }

        public static async Task<List<Series>> GetAllSeriesAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(25, 25);
            var list = new ConcurrentBag<Series>();
            
            if (!Directory.Exists(SeriesDirectory))
            {
                Console.WriteLine($"Directory {SeriesDirectory} does not exist.");
                return [];
            }


            var directories = Directory.GetDirectories(SeriesDirectory,string.Empty, SearchOption.TopDirectoryOnly);

            //Console.WriteLine($"Found {directories.Length} directories!");
            foreach (var dir in directories)
            {
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    
                    try
                    {
                        var series = new Series();
                        var showInfoFilePath = Path.Combine(dir, "info.csv");

                        //Console.WriteLine($"Looking for {showInfoFilePath}...");
                        var namesOfShow = (await File.ReadAllLinesAsync(showInfoFilePath))
                            .Skip(1)
                            .Select(line => line.Split(";"))
                            .ToList()[0];

                        series.Names.Add(Language.English, namesOfShow[0]);
                        series.Names.Add(Language.German, namesOfShow[1]);
                        var genresTask = GenreManager.Instance.GetGenresFromStringAsync(namesOfShow[2]);
                        var addTask = genresTask.ContinueWith(genres => series.Genres.AddRange(genres.Result)); 

                        series.ImagePath = Path.Combine(ThumbnailDirectory, namesOfShow[3]);

                        var groupInfos = Directory.GetFiles(dir, "*.csv", SearchOption.AllDirectories);

                        //Console.WriteLine($"Found {groupInfos.Length} infos!");
                        foreach (var episodeGroupInfo in groupInfos)
                        {
                            if (showInfoFilePath == episodeGroupInfo) continue;
                            
                            var episodes = await GetEpisodesFromGroupInfoAsync(episodeGroupInfo);
                            series.Episodes.AddRange(episodes);
                        }

                        await addTask;
                        list.Add(series);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            Console.WriteLine($"Extracted series in {stopwatch.ElapsedMilliseconds / 1000f}s!");

            return list.ToList();
        }

        private static async Task<List<Video>> GetEpisodesFromGroupInfoAsync(string infoPath)
        {
            var parentDir = Path.GetDirectoryName(infoPath) ?? string.Empty;
            var episodes = new List<Video>();
            var lines = await File.ReadAllLinesAsync(infoPath);

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(';');
                if (parts.Length != 3) continue;

                var episode = new Video
                {
                    PathToVideoFile = Path.Combine(parentDir, $"{parts[0]}.mkv")
                };
                episode.Names.Add(Language.English, parts[1]);
                episode.Names.Add(Language.German, parts[2]);

                episodes.Add(episode);
            }

            return episodes;
        }
        
        public static string? GetMountPoint(string path)
        {
            // Use the 'df' command to get the mount point
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "df",
                Arguments = $"--output=target \"{path}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process? process = Process.Start(startInfo))
            {
                if (process == null) return null;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    string[] lines = result.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 1)
                    {
                        return lines[1].Trim();
                    }
                }
            } 

            return "/";
        }
    }
}
