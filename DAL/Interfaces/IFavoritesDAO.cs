using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IFavoritesDAO
    {
        void Insert(FavoritesEntity favorite);
        void Delete(Guid favoriteID);
        IEnumerable<FavoritesEntity> Select(Guid UserId);
    }
}
