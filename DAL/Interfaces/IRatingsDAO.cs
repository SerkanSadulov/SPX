using Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IRatingsDAO
    {
        void Delete(Guid serviceId, Guid clientId);
        void Insert(RatingsEntity serviceRating);
        IEnumerable<RatingsEntity> Select();
        public IEnumerable<RatingsEntity> SelectHighestRated();
        public IEnumerable<RatingsEntity> SelectByServiceID(Guid serviceId);
        IEnumerable<RatingsEntity> SelectByClientID(Guid clientId);
        public IEnumerable<RatingsEntity> Select(Guid serviceId, Guid clientId);
        void Update(RatingsEntity serviceRating);
    }
}
