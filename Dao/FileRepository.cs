using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Logging;
using MovieRepository.Models;

namespace MovieRepository.Dao;

public class FileRepository : IRepository
{
    private readonly string _fileName;
    private readonly ILogger<FileRepository> _logger;

    public List<Movie> Movies { get; set; } = new();

    public FileRepository(ILogger<FileRepository> logger)
    {
        _logger = logger;
        logger.LogInformation("Here is some information");

        _fileName = "Files/movies.csv";
    }

    public void Display(int numberPerPage = 10)
    {
        var pageNumber = 0;
        Console.WriteLine($"|{"Id",-5}|{"Title",-80}|{"Genres",-10}");
        foreach (var movie in Movies)
        {
            var movies = Movies.Skip(numberPerPage * pageNumber).Take(numberPerPage);
            foreach (var m in movies)
            {
                Console.WriteLine($"|{m.Id,-5}|{m.Title,-80}|{string.Join(",", m.Genres),-10}");
            }
            pageNumber++;
            movies = Movies.Skip(numberPerPage * pageNumber).Take(numberPerPage);

            if (!ContinueDisplaying())
            {
                break;
            }
        }
    }

    private bool ContinueDisplaying()
    {
        Console.WriteLine("Hit Enter to continue or ESC to cancel");
        var input = Console.ReadKey();
        while (input.Key != ConsoleKey.Enter && input.Key != ConsoleKey.Escape)
        {
            input = Console.ReadKey();
            Console.WriteLine("Hit Enter to continue or ESC to cancel");
        }

        if (input.Key == ConsoleKey.Escape)
        {
            return false;
        }

        return true;
    }

    public void Read()
    {
        if (!File.Exists(_fileName))
        {
            _logger.LogError($"File {_fileName} does not exist");
            return;
        }

        using (var stream = new StreamReader(_fileName))
        {
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<MovieMap>();

                var movie = new Movie();
                var records = csv.GetRecords<Movie>();
                Movies = records.ToList();
            }
        }
    }

    public void Write(int movieId, string movieTitle, string genresString)
    {
        if (!File.Exists(_fileName))
        {
            _logger.LogError($"File {_fileName} does not exist");
            return;
        }

        var movie = new Movie
        {
            Genres = genresString.Split('|').ToList(),
            Id = movieId,
            Title = movieTitle
        };
        Movies.Add(movie);

        using (var stream = new StreamWriter(_fileName))
        {
            using (var csv = new CsvWriter(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<MovieMap>();

                csv.WriteRecords(Movies);
            }
        }
    }
}
