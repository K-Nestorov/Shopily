using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.User;

namespace Shopily.Services
{
    public class UserServices
    {
       public static EditVM createEditVM(User item)
        {
            EditVM model = new EditVM
            {
                Id = item.Id,
                Username = item.Username,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
               
            };
            return model;
        }
        
    }
    
}
