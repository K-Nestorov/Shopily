using Shopily.Data;
using Shopily.Domain.Entity;
using Shopily.Domain.ViewModel.User;

namespace Shopily.Repositories
{
    public class UserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public void RegisterUserVM(RegisterVM model)
        {
          
            User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Password = model.Password 
            };

            
            _context.Users.Add(user);

         
            _context.SaveChanges();
        }

  
        public async Task SaveUser(User user)
        {
           
            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();
        }
    }
}
