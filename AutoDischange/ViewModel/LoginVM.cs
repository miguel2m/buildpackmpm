using AutoDischange.Model;
using AutoDischange.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDischange.ViewModel
{
    public class LoginVM
    {
        private User user;
        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public RegisterCommand RegisterCommand { get; set; }
        public LoginCommand LoginCommand { get; set; }


        //Constructor (ctor)
        public LoginVM()
        {
            RegisterCommand = new RegisterCommand(this);
            LoginCommand = new LoginCommand(this);
        }
    }
}
