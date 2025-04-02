using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AngularApp2.Server.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required(ErrorMessage = "Address Line is required.")]
        public required string AddressLine { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public required string State { get; set; }

        [Required(ErrorMessage = "PinCode is required.")]
        public required string PinCode { get; set; }

        // FK to Student
        [ForeignKey("StudentId")]
        public int StudentId { get; set; }

        // Navigation property to Student
        public virtual Student? Student { get; set; }
    }
}
