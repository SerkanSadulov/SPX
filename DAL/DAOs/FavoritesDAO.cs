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
    // DAO class for handling favorite-related database operations
    public class FavoritesDAO : IFavoritesDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<FavoritesDAO> _logger;

        // Constructor to initialize the database factory and logger
        public FavoritesDAO(IFactory<SqlConnection> databaseFactory, ILogger<FavoritesDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Select favorites by UserID
        public IEnumerable<FavoritesEntity> Select(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<FavoritesEntity>(SQLQueries.SelectFavoriteByUserId, new { userId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select favorites by UserID: {userId}.", DateTime.UtcNow, userId);
                    return Enumerable.Empty<FavoritesEntity>(); // Return an empty list if an error occurs
                }
            }
            else
            {
                throw new ArgumentException("Invalid User ID!");
            }
        }

        // Delete a favorite by FavoriteID
        public void Delete(Guid favoriteID)
        {
            if (favoriteID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.DeleteFavorite, new { FavoriteID = favoriteID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Delete favorite by FavoriteID: {favoriteID}.", DateTime.UtcNow, favoriteID);
                }
            }
            else
            {
                throw new ArgumentException("Invalid favorite identifier!");
            }
        }

        // Insert a new favorite
        public void Insert(FavoritesEntity favorite)
        {
            if (favorite != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertFavorite, favorite);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Insert favorite.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid favorite object!");
            }
        }
    }
}
