using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DAL.DAOs
{
    // DAO class for handling ratings-related database operations
    public class RatingsDAO : IRatingsDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<RatingsDAO> _logger;

        // Constructor to initialize the database factory and logger
        public RatingsDAO(IFactory<SqlConnection> databaseFactory, ILogger<RatingsDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Get all ratings
        public IEnumerable<RatingsEntity> Select()
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<RatingsEntity>(SQLQueries.SelectRatings);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Select all ratings.", DateTime.UtcNow);
                return Array.Empty<RatingsEntity>(); // Return empty array on failure
            }
        }

        // Get all ratings over 4 stars
        public IEnumerable<RatingsEntity> SelectHighestRated()
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<RatingsEntity>(SQLQueries.SelectHighestRated);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Select all ratings.", DateTime.UtcNow);
                return Array.Empty<RatingsEntity>(); // Return empty array on failure
            }
        }

        // Get ratings by client ID
        public IEnumerable<RatingsEntity> SelectByClientID(Guid clientId)
        {
            if (clientId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<RatingsEntity>(SQLQueries.SelectRatingsByClientID, new { ClientID = clientId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select ratings by ClientID: {clientId}.", DateTime.UtcNow, clientId);
                    return Array.Empty<RatingsEntity>(); // Return empty array on failure
                }
            }
            throw new ArgumentException("Invalid ClientID!");
        }

        // Get rating by service ID and client ID
        public IEnumerable<RatingsEntity> Select(Guid serviceId, Guid clientId)
        {
            if (serviceId != Guid.Empty && clientId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<RatingsEntity>(SQLQueries.SelectRatings, new { ServiceID = serviceId, ClientID = clientId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select rating by ServiceID: {serviceId} and ClientID: {clientId}.", DateTime.UtcNow, serviceId, clientId);
                    return Array.Empty<RatingsEntity>(); // Return empty array on failure
                }
            }
            throw new ArgumentException("Invalid ServiceID or ClientID!");
        }

        // Get ratings by service ID
        public IEnumerable<RatingsEntity> SelectByServiceID(Guid serviceId)
        {
            if (serviceId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<RatingsEntity>(SQLQueries.SelectRatingsByServiceID, new { ServiceID = serviceId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select ratings by ServiceID: {serviceId}.", DateTime.UtcNow, serviceId);
                    return Array.Empty<RatingsEntity>(); // Return empty array on failure
                }
            }
            throw new ArgumentException("Invalid ServiceID!");
        }

        // Insert a new rating
        public void Insert(RatingsEntity ratingsEntity)
        {
            if (ratingsEntity == null)
            {
                throw new ArgumentException("Invalid RatingsEntity object!");
            }

            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                sqlConnection.Execute(SQLQueries.InsertRating, ratingsEntity);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Insert rating.", DateTime.UtcNow);
                throw; // Rethrow exception to notify the caller of the failure
            }
        }

        // Update an existing rating
        public void Update(RatingsEntity ratingsEntity)
        {
            if (ratingsEntity != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.UpdateRating, ratingsEntity);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Update rating.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid RatingsEntity object!");
            }
        }

        // Delete a rating by service ID and client ID
        public void Delete(Guid serviceId, Guid clientId)
        {
            if (serviceId != Guid.Empty && clientId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.DeleteRating, new { ServiceID = serviceId, ClientID = clientId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Delete rating by ServiceID: {serviceId} and ClientID: {clientId}.", DateTime.UtcNow, serviceId, clientId);
                }
            }
            else
            {
                throw new ArgumentException("Invalid ServiceID or ClientID!");
            }
        }
    }
}
