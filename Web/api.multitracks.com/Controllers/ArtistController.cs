using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTDataAccess;
using MTDataAccess.Models;

namespace api.multitracks.com.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public ArtistController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        [HttpGet]
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
