﻿namespace Shopily.ViewModel.User
{
    public class EditVM
    {
        public Guid Id { get; set; }
        public string Username {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
    }
}
