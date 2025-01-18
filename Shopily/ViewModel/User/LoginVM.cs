namespace Shopily.ViewModel.User
{
    public class LoginVM
    {
       public int Id { get; set; }  
        public string Username {  get; set; }   
        public string Password { get; set; }    
        public bool isAdmin {  get; set; }
        public bool RememberMe { get;set; }
    }
}
