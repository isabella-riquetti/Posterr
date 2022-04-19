using System.ComponentModel.DataAnnotations;

namespace Posterr.Services.Model.User
{
    public class CreateUserRequestModel
    {
        public string Name { get; set; }
        
        [Required]
        public string Username { get; set; }
    }
}
