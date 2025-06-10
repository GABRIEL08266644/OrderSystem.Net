﻿namespace OrderSystem.Web.ViewModels
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        
        public DateTime RegistrationDate { get; set; }
    }
}
