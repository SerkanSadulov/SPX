using DAL.Interfaces;
using Dapper;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DAL.DAOs
{
    // DAO for handling user-related database operations
    public class UserDAO : IUserDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<UserEntity> _logger;

        // Constructor to initialize database factory and logger
        public UserDAO(IFactory<SqlConnection> databaseFactory, ILogger<UserEntity> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Get all users
        public IEnumerable<UserEntity> Select()
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<UserEntity>(SQLQueries.SelectUsers);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in Select all users.", DateTime.UtcNow);
                return Enumerable.Empty<UserEntity>(); // Return empty list on failure
            }
        }

        // Get user by ID
        public UserEntity Select(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.QuerySingle<UserEntity>(SQLQueries.SelectUserById, new { UserId = userId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select user by UserId: {userId}.", DateTime.UtcNow, userId);
                    return new UserEntity(); // Return default user on failure
                }
            }
            else
                throw new ArgumentException("Invalid user identifier!");
        }
        //Get user by username
        public UserEntity SelectByUsername(string username)
        {
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrWhiteSpace(username))
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.QuerySingle<UserEntity>(SQLQueries.SelectUserByUsername, new { Username = username });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Select user by Username: {username}.", DateTime.UtcNow, username);
                    return new UserEntity(); // Return default user on failure
                }
            }
            else
                throw new ArgumentException("Invalid user identifier!");
        }

        // Get user by login details
        public UserEntity Select(LoginEntity logIn)
        {
            if (logIn != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.QuerySingle<UserEntity>(SQLQueries.SelectUserByNameAndPassword, logIn);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} No user found", DateTime.UtcNow);
                    return new UserEntity(); // Return default user on failure
                }
            }
            else
                throw new ArgumentException("Invalid login credentials!");
        }

        // Insert new user
        public void Insert(UserEntity user)
        {
            if (user != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertUser, user);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Insert user.", DateTime.UtcNow);
                }
            }
            else
                throw new ArgumentException("Invalid user object!");
        }

        // Update all user fields
        public void UpdateAll(UserEntity user)
        {
            if (user != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.UpdateUser, user);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Update user.", DateTime.UtcNow);
                }
            }
            else
                throw new ArgumentException("Invalid user object!");
        }

        // Update specific user fields
        public void Update(EditUserEntity user)
        {
            if (user != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.UpdateUserPPName, user);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Update user.", DateTime.UtcNow);
                }
            }
            else
                throw new ArgumentException("Invalid user object!");
        }

        // Delete user by ID
        public void Delete(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.DeleteUser, new { UserId = userId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in Delete user by UserId: {userId}.", DateTime.UtcNow, userId);
                }
            }
            else
                throw new ArgumentException("Invalid user identifier!");
        }
    }
}
