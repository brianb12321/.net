using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTDataAccess;
using MTDataAccess.Models;

namespace api.multitracks.com.Controllers
{
    [Route("song")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        public SongController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Route("list")]
        [HttpGet]
        public IActionResult List(uint pageNumber = 1, uint numberPerPage = 999999999)
        {
            IEnumerable<Song> songs = _dataAccess.GetAllSongs(pageNumber, numberPerPage);
            return Ok(songs);
        }
    }
}