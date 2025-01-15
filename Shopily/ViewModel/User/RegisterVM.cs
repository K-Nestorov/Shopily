namespace Shopily.ViewModel.User
{
    public class RegisterVM
    {
     
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public  bool Role {  get; set; }
        public bool IsAdmin { get; set; }
    }
}
