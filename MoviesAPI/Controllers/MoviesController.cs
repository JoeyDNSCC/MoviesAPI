using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;
using System.Data;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IDbConnection _db;

        //Constructor
        public MoviesController(IDbConnection db)
        {
            // Initialize any dependencies or services here
            _db = db;
        }

        // GET api/movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetAllMovies()
        {
            var sql =
            @"
                SELECT m.*, g.*, a.*, r.*, u.*
                FROM Movie m
                INNER JOIN Genre g ON m.GenreId = g.Id
                INNER JOIN AudienceRating a ON m.AudienceRatingId = a.Id
                LEFT OUTER JOIN CriticReview r ON m.Id = r.MovieId
                INNER JOIN UserSnapshot u ON u.UserId = r.CreatedBy
            ";

            var movies = await _db.QueryAsync<Movie, Genre, AudienceRating, Movie>( // <table1, table2, table3, returnType>
                sql,
                (movie, genre, rating) => {
                    movie.Genre = genre;
                    movie.AudienceRating = rating;
                    return movie;
                },
                splitOn: "Id" // specify what column to split on (defaults to Id, just explicitly putting it here)
                );

            return Ok(movies); // 200OK
        }

        // GET api/movies/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovieById(int id)
        {
            var sql =
            @"
                SELECT m.*, g.*, a.*, r.*, u.*
                FROM Movie m
                INNER JOIN Genre g ON m.GenreId = g.Id
                INNER JOIN AudienceRating a ON m.AudienceRatingId = a.Id
                LEFT OUTER JOIN CriticReview r ON m.Id = r.MovieId
                INNER JOIN UserSnapshot u ON u.UserId = r.CreatedBy
                WHERE m.Id = @Id
            ";

            // create a null movie object that gets pupulated from query results
            Movie? movie = null;

            await _db.QueryAsync<Movie, Genre, AudienceRating, User, Review, Movie>( // <table1, table2, table3, returnType>
                sql,
                (m, g, a, u, r) => {
                    // populate movie objeect with first row of data
                    if (movie == null)
                    {
                        movie = m;
                        movie.Genre = g;
                        movie.AudienceRating = a;
                        movie.Reviews = new List<Review>();
                    }

                    // add review to movies review list if it exists
                    if (r != null )
                    {
                        r.User = u;
                        movie.Reviews.Add(r);
                    }

                    return m;
                },
                new { Id = id }, //replace @Id olaceholder with its value in the SQL string
                splitOn: "Id,Id,UserId" // specify what column to split on (defaults to Id, just explicitly putting it here)
                );

            return Ok(movie); // 200OK
        }
    }
}
