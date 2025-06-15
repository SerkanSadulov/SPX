using Domain.Entities;

namespace DAL.Interfaces
{
    public interface ICategoriesDAO
    {
        IEnumerable<CategoriesEntity> Select();
        void Insert(CategoriesEntity category);
        CategoriesEntity Select(Guid CategoryID);
    }
}
