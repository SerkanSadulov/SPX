using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IOngoingOrderDAO
    {
        IEnumerable<OngoingOrderEntity> SelectByProvider(Guid providerID);
        IEnumerable<OngoingOrderEntity> SelectByUser(Guid userID);
        void Insert(OngoingOrderEntity order);
        void UpdateStatus(Guid orderID, string status);
    }
}
