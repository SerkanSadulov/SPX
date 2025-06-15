using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CategoriesEntity
    {
        [Required]
        public Guid CategoryID { get; set; } = Guid.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;

    }
}
