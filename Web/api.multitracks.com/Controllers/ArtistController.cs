using Microsoft.AspNetCore.Mvc;
using MTDataAccess;
using MTDataAccess.Models;

namespace api.multitracks.com.Controllers
{
    [ApiController]
    [Route("artist")]
    public class ArtistController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public ArtistController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string artistName)
        {
            //Validation checks
            if (string.IsNullOrWhiteSpace(artistName))
                return BadRequest("Artist name is required");
            if (artistName.Length > Artist.TITLE_MAX)
                return BadRequest($"Artist name must be less than {Artist.TITLE_MAX}");

            var artists = _dataAccess.GetArtistsByName(artistName);
            return Ok(artists);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(Artist artist)
        {
            //Validation
            if (artist == null)
                return BadRequest("POST body must contain artist information to be added.");

            Artist newArtist = _dataAccess.AddArtist(artist);
            return Ok(newArtist);
        }
    }
}