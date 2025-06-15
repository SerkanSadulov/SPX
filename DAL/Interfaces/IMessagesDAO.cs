using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IMessagesDAO
    {
        IEnumerable<MessagesEntity> GetMessages(Guid senderId, Guid receiverId);
        public IEnumerable<MessagesEntity> GetMessagesByReceiverID(Guid receiverId);
        IEnumerable<Guid> GetMessagedPeople(Guid userId);
        void SaveMessage(MessagesEntity message);
    }
}