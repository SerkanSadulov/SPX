using Domain.Entities;

namespace DAL.Interfaces
{
    public interface IUserDAO
    {
        void Delete(Guid userId);
        void Insert(UserEntity user);
        IEnumerable<UserEntity> Select();
        UserEntity Select(Guid userId);
        UserEntity SelectByUsername(string username);
        UserEntity Select(LoginEntity login);
        void UpdateAll(UserEntity user);
        void Update(EditUserEntity user);

    }
}