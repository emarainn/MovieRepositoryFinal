using Microsoft.Extensions.Logging;

namespace MovieRepository.Dao;

/// <summary>
///     This concrete service and method only exists an example.
///     It can either be copied and modified, or deleted.
/// </summary>
public class StudentRepository : IRepository
{
    private readonly string _fileName;
    private readonly ILogger<IRepository> _logger;
    private readonly List<string> _movieGenres;

    //these should not be here
    private readonly List<int> _movieIds;
    private readonly List<string> _movieTitles;
    private int? _nextId;

    public StudentRepository(ILogger<StudentRepository> logger)
    {
        _logger = logger;
        logger.LogInformation("Here is some information");

        _fileName = "Files/movies.csv";

        _movieIds = new List<int>();
        _movieTitles = new List<string>();
        _movieGenres = new List<string>();
    }

    public void Display(int numberPerPage = 10)
    {
        // Display All Movies
        // loop thru Movie Lists
        for (var i = 0; i < _movieIds.Count; i++)
        {
            // display movie details
            Console.WriteLine($"Id: {_movieIds[i]}");
            Console.WriteLine($"Title: {_movieTitles[i]}");
            Console.WriteLine($"Genre(s): {_movieGenres[i]}");
            Console.WriteLine();
        }
    }

    public void Read()
    {
        _logger.LogInformation("Reading");
        Console.WriteLine("*** I am reading");

        // to populate the lists with data, read from the data file
        try
        {
            var sr = new StreamReader(_fileName);
            // first line contains column headers
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                // first look for quote(") in string
                // this indicates a comma(,) in movie title
                var idx = line.IndexOf('"');
                if (idx == -1)
                {
                    // no quote = no comma in movie title
                    // movie details are separated with comma(,)
                    var movieDetails = line.Split(',');
                    // 1st array element contains movie id
                    _movieIds.Add(int.Parse(movieDetails[0]));
                    // 2nd array element contains movie title
                    _movieTitles.Add(movieDetails[1]);
                    // 3rd array element contains movie genre(s)
                    // replace "|" with ", "
                    _movieGenres.Add(movieDetails[2].Replace("|", ", "));
                }
                else
                {
                    // quote = comma in movie title
                    // extract the movieId
                    _movieIds.Add(int.Parse(line.Substring(0, idx - 1)));
                    // remove movieId and first quote from string
                    line = line.Substring(idx + 1);
                    // find the next quote
                    idx = line.IndexOf('"');
                    // extract the movieTitle
                    _movieTitles.Add(line.Substring(0, idx));
                    // remove title and last comma from the string
                    line = line.Substring(idx + 2);
                    // replace the "|" with ", "
                    _movieGenres.Add(line.Replace("|", ", "));
                }
            }

            // close file when done
            sr.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        _logger.LogInformation("Movies in file {Count}", _movieIds.Count);
    }

    public void Write(int movieId, string movieTitle, string genresString)
    {
        Console.WriteLine("*** I am writing");

        var sw = new StreamWriter(_fileName, true);
        sw.WriteLine($"{movieId},{movieTitle},{genresString}");
        sw.Close();

        // add movie details to Lists
        _movieIds.Add(movieId);
        _movieTitles.Add(movieTitle);
        _movieGenres.Add(genresString);
        // log transaction
        _logger.LogInformation("Movie id {Id} added", movieId);
    }

    private bool CheckTitles(string nextTitle)
    {
        var present = false;
        foreach (var title in _movieTitles)
        {
            if (title.Equals(nextTitle))
            {
                present = true;
                break;
            }
        }

        if (present)
        {
            _logger.LogInformation($"Movie titled {nextTitle} already exists");
        }
        else
        {
            _logger.LogInformation($"Movie titled {nextTitle} not found on database");
        }

        return present;
    }

    private int? GetNextInt()
    {
        if (_nextId == null)
        {
            _nextId = _movieIds.Max() + 1;
        }
        else
        {
            _nextId++;
        }

        _logger.LogInformation($"Next ID found successfully, ID: {_nextId}");
        return _nextId;
    }
}
