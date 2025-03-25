using Shopily.Domain.Entity.Base;
using Shopily.Domain.ViewModel.Admin;
using Shopily.Domain.ViewModel.User;

namespace Shopily.Domain.Entity
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
       
        public bool IsAdmin { get; set; }
        public User()
        {

        }

        public User(RegisterVM model)
        {
            Username = model.Username;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            Password = model.Password;
            IsAdmin = model.IsAdmin;
        }

        public void RegisterUser(RegisterVM model)
        {
            Username = model.Username;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            Password = model.Password;
            IsAdmin = false;
        }

        public void EditUser(EditVM model)
        {
            Username = model.Username;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
        }

        public void AdminEditUser(AdminEditVM model)
        {
            Username = model.Username;
            FirstName = model.FirstName;
            LastName = model.LastName;
            Email = model.Email;
            IsAdmin = model.IsAdmin;
        }

    }
}
