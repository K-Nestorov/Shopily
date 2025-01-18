using Shopily.ViewModel.Admin;
using Shopily.ViewModel.User;
using System.ComponentModel.DataAnnotations;

namespace Shopily.Entity
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password {  get; set; }
        public bool IsAdmin { get; set; }
        public User()
        {

        }

        public User(RegisterVM model)
        {
            this.Username = model.Username;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = model.Email;
            this.Password = model.Password;
            this.IsAdmin = model.IsAdmin;
        }

        public void RegisterUser(RegisterVM model)
        {
            this.Username = model.Username;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = model.Email;
            this.Password = model.Password;
            this.IsAdmin = false;
        }

        public void EditUser(EditVM model)
        {
            this.Username = model.Username;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = model.Email;
        }

        public void AdminEditUser(AdminEditVM model)
        {
            this.Username = model.Username;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Email = model.Email;
            this.IsAdmin=model.IsAdmin;
        }

    }
}
