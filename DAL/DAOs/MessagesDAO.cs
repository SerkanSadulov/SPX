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
    // DAO class for handling message-related database operations
    public class MessagesDAO : IMessagesDAO
    {
        private readonly IFactory<SqlConnection> _databaseFactory;
        private readonly ILogger<MessagesDAO> _logger;

        // Constructor to initialize the database factory and logger
        public MessagesDAO(IFactory<SqlConnection> databaseFactory, ILogger<MessagesDAO> logger)
        {
            _databaseFactory = databaseFactory;
            _logger = logger;
        }

        // Retrieve messages between a sender and a receiver
        public IEnumerable<MessagesEntity> GetMessages(Guid senderId, Guid receiverId)
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<MessagesEntity>(SQLQueries.SelectMessages, new { SenderId = senderId, ReceiverId = receiverId });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in GetMessages between SenderID: {senderId} and ReceiverID: {receiverId}.", DateTime.UtcNow, senderId, receiverId);
                return Enumerable.Empty<MessagesEntity>();
            }
        }

        // Retrieve messages between a sender and a receiver
        public IEnumerable<MessagesEntity> GetMessagesByReceiverID(Guid receiverId)
        {
            using SqlConnection sqlConnection = _databaseFactory.Get();
            try
            {
                return sqlConnection.Query<MessagesEntity>(SQLQueries.SelectMessagesByRecieverID, new {ReceiverId = receiverId });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{DateTime} Error in GetMessages ReceiverID: {receiverId}.", DateTime.UtcNow, receiverId);
                return Enumerable.Empty<MessagesEntity>();
            }
        }


        // Get a list of people the user has messaged with
        public IEnumerable<Guid> GetMessagedPeople(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    return sqlConnection.Query<Guid>(SQLQueries.GetMessagedPeople, new { UserId = userId });
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in GetMessagedPeople for UserID: {userId}.", DateTime.UtcNow, userId);
                    return Enumerable.Empty<Guid>();
                }
            }
            else
            {
                throw new ArgumentException("Invalid user identifier!");
            }
        }

        // Save a message to the database
        public void SaveMessage(MessagesEntity message)
        {
            if (message != null)
            {
                using SqlConnection sqlConnection = _databaseFactory.Get();
                try
                {
                    sqlConnection.Execute(SQLQueries.InsertMessage, message);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "{DateTime} Error in SaveMessage.", DateTime.UtcNow);
                }
            }
            else
            {
                throw new ArgumentException("Invalid message object!");
            }
        }
    }
}
