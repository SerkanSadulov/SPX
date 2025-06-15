using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class OngoingOrderEntity
    {
        [Required]
        public Guid OrderID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ServiceID { get; set; }

        [Required]
        public Guid ProviderID { get; set; }

        [Required]
        [StringLength(255)]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public Guid CategoryID { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
