using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.UserModels
{
    public class UpdateUserModel
    {
        [Required] public string Username { get; set; }
    }
}