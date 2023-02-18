namespace MovieRepository.Dao;

/// <summary>
///     This service interface only exists an example.
///     It can either be copied and modified, or deleted.
/// </summary>
public interface IRepository
{
    void Display(int numberPerPage = 10);
    void Read();
    void Write(int movieId, string movieTitle, string genresString);
}
