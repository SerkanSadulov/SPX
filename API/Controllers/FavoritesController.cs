using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoritesDAO _favoritesDAO;

        public FavoritesController(IFavoritesDAO favoritesDAO)
        {
            _favoritesDAO = favoritesDAO;
        }

        [HttpGet("{id:guid}")]
        public ActionResult<FavoritesEntity> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid favorite identifier.");

            var favorite = _favoritesDAO.Select(id);
            if (favorite != null)
            {
                return Ok(favorite);
            }

            return NotFound("Favorite not found.");
        }
        
        [HttpPost]
        public ActionResult<FavoritesEntity> Post(FavoritesEntity favorite)
        {
            if (favorite == null || favorite.FavoriteID == Guid.Empty)
                return BadRequest("Invalid favorite entity or favoriteID.");

            _favoritesDAO.Insert(favorite);

            return CreatedAtAction(nameof(Get), new { id = favorite.FavoriteID }, favorite);
        }


        [HttpDelete("{favoriteID:guid}")]
        public ActionResult Delete(Guid favoriteID)
        {
            if (favoriteID == Guid.Empty)
                return BadRequest("Invalid FavoriteID.");

            var existingFavorite = _favoritesDAO.Select(favoriteID);
            if (existingFavorite == null)
                return NotFound("Favorite not found.");

            _favoritesDAO.Delete(favoriteID);
            return NoContent();
        }
    }
}
