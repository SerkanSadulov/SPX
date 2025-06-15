using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ServicesEntity
    {
        [Required]
        public Guid ServiceID { get; set; } = Guid.Empty;

        [Required]
        public Guid ProviderID { get; set; } = Guid.Empty;

        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public Guid CategoryID { get; set; } = Guid.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } = 0;

        public string Description { get; set; } = string.Empty;

        [StringLength(15)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string Images { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
