using Domain.DTOs;
using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IServicesDAO
    {
        IEnumerable<ServicesEntity> Select();
        IEnumerable<ServicesEntity> SelectProviderID(Guid providerID);
        IEnumerable<ServicesEntity> SelectCategoryID(Guid categoryID);
        public IEnumerable<ServicesEntity> GetServicesByName(string serviceName);
        void Insert(ServicesEntity service);
        ServicesEntity SelectServiceID(Guid serviceID);
        void Update(ServicesEntity service);
        void Delete(Guid serviceID);
        public ServiceDTO SelectDTOByID(Guid serviceID);




    }
}
