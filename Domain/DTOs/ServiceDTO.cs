using Domain.Entities;

namespace Domain.DTOs
{
    public class ServiceDTO
    {
        public ServicesEntity? Services { get; set; }
        public CategoriesEntity? Categories { get; set; }
        public UserEntity? User { get; set; }
    }
}
