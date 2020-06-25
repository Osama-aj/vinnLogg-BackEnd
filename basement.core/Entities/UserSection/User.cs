using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basement.core.Entities.UserSection
{
    public class User
    {
        [Column(TypeName = "VARCHAR")]
        [StringLength(48)]
        [Key] public string UserId { get; set; }
        //  [Required]public string  FirstName { get; set; }
        // [Required]public string  LastName { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(30)]
        [Required] public string Username { get; set; }
        // [Required] public string Email { get; set; }
        //  [Required] public byte[] Hash { get; set; }
        // public bool EmailConfirmed { get; set; } = false;
        // public string PhoneNumber { get; set; }
        // public bool PhoneNumberConfirmed { get; set; } = false;


        // public ICollection<Shelf> Shelves { get; set; }
    }
}