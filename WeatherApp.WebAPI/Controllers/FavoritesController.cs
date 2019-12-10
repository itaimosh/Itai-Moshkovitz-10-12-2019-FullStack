using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly WeatherAppContext _context;

        public FavoritesController(WeatherAppContext context)
        {
            _context = context;
        }

        // GET: api/Favorites
        [HttpGet]
        public IEnumerable<Favorites> GetFavorites()
        {
            return _context.Favorites;
        }

       

        // POST: api/Favorites
        [HttpPost]
        public async Task<IActionResult> PostFavorites([FromBody] Favorites favorites)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existing = await _context.Favorites.FindAsync(favorites.Key);
            if (existing == null)
            {
                _context.Favorites.Add(favorites);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction("GetFavorites", new { id = favorites.Key }, favorites);
        }

        // DELETE: api/Favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorites([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var favorites = await _context.Favorites.FindAsync(id);
            if (favorites == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorites);
            await _context.SaveChangesAsync();

            return Ok(favorites);
        }

        private bool FavoritesExists(string id)
        {
            return _context.Favorites.Any(e => e.Key == id);
        }
    }
}