using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AngularApp2.Server.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

        // Optional field for Profile Picture stored as byte[]
        public byte[]? ProfilePicture { get; set; }

        // Navigation property for related addresses
        public virtual ICollection<Address>? Addresses { get; set; }

        //[NotMapped] // Don't map this to the database
        //public string? ProfilePictureBase64 { get; set; }

        //// Property accessor that handles both byte[] and string
        //[JsonIgnore] // Ignore this when serializing to JSON
        //public object? ProfilePictureObject
        //{
        //    get => ProfilePicture;
        //    set
        //    {
        //        if (value is string base64)
        //        {
        //            ProfilePictureBase64 = base64;
        //            // The actual conversion to byte[] happens in the controller
        //        }
        //        else if (value is byte[] bytes)
        //        {
        //            ProfilePicture = bytes;
        //        }
        //    }
        //}
    }
}
