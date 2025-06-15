using DAL.Interfaces;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DAL.DAOs
{
    // DAO for handling service-related database operations
    public class ServicesDAO : IServicesDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<ServicesDAO> _logger;

        // Constructor to initialize database factory and logger
        public ServicesDAO(IFactory<SqlConnection> databaseFactory, ILogger<ServicesDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Get all services
        public IEnumerable<ServicesEntity> Select()
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<ServicesEntity>(SQLQueries.GetAllServices);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Select all services.", DateTime.UtcNow);
                return Enumerable.Empty<ServicesEntity>(); // Return empty collection on failure
            }
        }

        // Get services by name
        public IEnumerable<ServicesEntity> GetServicesByName(string serviceName)
        {
            if (!string.IsNullOrEmpty(serviceName))
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<ServicesEntity>(SQLQueries.GetServiceByServiceName, new { ServiceName = serviceName });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in GetServicesByName for ServiceName: {serviceName}.", DateTime.UtcNow, serviceName);
                    return Enumerable.Empty<ServicesEntity>(); // Return empty collection on failure
                }
            }
            else
            {
                throw new ArgumentException("Service name cannot be null or empty!");
            }
        }

        // Get services by provider ID
        public IEnumerable<ServicesEntity> SelectProviderID(Guid providerID)
        {
            if (providerID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<ServicesEntity>(SQLQueries.GetServiceByProviderID, new { ProviderID = providerID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select service by providerID: {providerID}.", DateTime.UtcNow, providerID);
                    return Enumerable.Empty<ServicesEntity>(); // Return empty collection on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid providerID identifier!");
            }
        }

        // Get services by category ID
        public IEnumerable<ServicesEntity> SelectCategoryID(Guid categoryID)
        {
            if (categoryID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<ServicesEntity>(SQLQueries.GetServiceByCategoryID, new { CategoryID = categoryID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select service by categoryID: {categoryID}.", DateTime.UtcNow, categoryID);
                    return Enumerable.Empty<ServicesEntity>(); // Return empty collection on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid categoryID identifier!");
            }
        }

        // Get service by service ID
        public ServicesEntity SelectServiceID(Guid serviceID)
        {
            if (serviceID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.QuerySingle<ServicesEntity>(SQLQueries.GetServiceByServiceID, new { ServiceID = serviceID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select service by serviceID: {serviceID}.", DateTime.UtcNow, serviceID);
                    return new ServicesEntity(); // Return default entity on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid serviceID identifier!");
            }
        }

        // Get service DTO by service ID
        public ServiceDTO SelectDTOByID(Guid serviceID)
        {
            if (serviceID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    ServicesEntity service = sqlConnection.QuerySingle<ServicesEntity>(SQLQueries.GetServiceByServiceID, new { ServiceID = serviceID });
                    CategoriesEntity category = sqlConnection.QuerySingle<CategoriesEntity>(SQLQueries.SelectCategoryById, new { CategoryID = service.CategoryID });
                    UserEntity user = sqlConnection.QuerySingle<UserEntity>(SQLQueries.SelectUserById, new { UserID = service.ProviderID });

                    // Constructing and returning ServiceDTO
                    return new ServiceDTO()
                    {
                        Services = service,
                        Categories = category,
                        User = user
                    };
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in SelectDTOByID for serviceID: {serviceID}.", DateTime.UtcNow, serviceID);
                    return new ServiceDTO(); // Return default DTO on failure
                }
            }
            else
            {
                throw new ArgumentException("Invalid serviceID identifier!");
            }
        }

        // Update service
        public void Update(ServicesEntity service)
        {
            if (service != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.UpdateService, service);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Update service.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid service object!");
            }
        }

        // Delete service by ID
        public void Delete(Guid serviceID)
        {
            if (serviceID != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.DeleteService, new { ServiceID = serviceID });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Delete service by serviceID: {serviceID}.", DateTime.UtcNow, serviceID);
                }
            }
            else
            {
                throw new ArgumentException("Invalid serviceID identifier!");
            }
        }

        // Insert new service
        public void Insert(ServicesEntity service)
        {
            if (service != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertService, service);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Insert service.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid service object!");
            }
        }
    }
}
