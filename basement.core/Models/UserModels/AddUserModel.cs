using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.UserModels
{
    public class AddUserModel // or manage user post -> add ; delete -> delete ; no get or put
    {
        //[Required] public string Token { get; set; }
       // [Required] public string FirstName { get; set; }
      //  [Required] public string LastName { get; set; }
        [Required] public string UserName { get; set; }
       // [Required] public string Email { get; set; }
    }
}