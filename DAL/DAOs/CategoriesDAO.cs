using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DAL.DAOs
{
    // DAO class for handling category-related database operations
    public class CategoriesDAO : ICategoriesDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<CategoriesDAO> _logger;

        // Constructor to initialize the database factory and logger
        public CategoriesDAO(IFactory<SqlConnection> databaseFactory, ILogger<CategoriesDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Select all categories
        public IEnumerable<CategoriesEntity> Select()
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<CategoriesEntity>(SQLQueries.SelectCategory);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Select all categories.", DateTime.UtcNow);
                return new List<CategoriesEntity>(); // Return an empty list on error
            }
        }

        // Select a single category by CategoryID
        public CategoriesEntity Select(Guid categoryID)
        {
            if (categoryID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.QuerySingle<CategoriesEntity>(SQLQueries.SelectCategoryById, new { CategoryID = categoryID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select category by CategoryID: {CategoryID}.", DateTime.UtcNow, categoryID);
                    return null; // Return null if the query fails
                }
            }
            else
            {
                throw new ArgumentException("Invalid category identifier!");
            }
        }

        // Insert a new category
        public void Insert(CategoriesEntity category)
        {
            if (category != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertCategory, category);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Insert category.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid category object!");
            }
        }
    }
}
