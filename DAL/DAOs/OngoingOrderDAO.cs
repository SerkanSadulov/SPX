using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.DAOs
{
    // DAO class for handling ongoing orders-related database operations
    public class OngoingOrderDAO : IOngoingOrderDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<OngoingOrderDAO> _logger;

        // Constructor to initialize the database factory and logger
        public OngoingOrderDAO(IFactory<SqlConnection> databaseFactory, ILogger<OngoingOrderDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Get ongoing orders by provider ID
        public IEnumerable<OngoingOrderEntity> SelectByProvider(Guid providerId)
        {
            if (providerId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<OngoingOrderEntity>(SQLQueries.SelectOngoingOrderByProviderID, new { ProviderID = providerId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select orders by ProviderID: {providerId}.", DateTime.UtcNow, providerId);
                    return Enumerable.Empty<OngoingOrderEntity>(); // Return empty list on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid provider identifier!");
            }
        }

        // Get ongoing orders by user ID
        public IEnumerable<OngoingOrderEntity> SelectByUser(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<OngoingOrderEntity>(SQLQueries.SelectOngoingOrderByUserID, new { UserID = userId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select orders by UserID: {userId}.", DateTime.UtcNow, userId);
                    return Enumerable.Empty<OngoingOrderEntity>(); // Return empty list on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid user identifier!");
            }
        }

        // Insert a new ongoing order
        public void Insert(OngoingOrderEntity order)
        {
            if (order != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertOngoingOrder, order);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Insert order.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid order object!");
            }
        }

        // Update the status of an ongoing order
        public void UpdateStatus(Guid orderId, string status)
        {
            if (orderId != Guid.Empty && !string.IsNullOrWhiteSpace(status))
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.UpdateOngoingOrderStatus, new { OrderID = orderId, Status = status });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Update order status.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid order identifier or status!");
            }
        }
    }
}
