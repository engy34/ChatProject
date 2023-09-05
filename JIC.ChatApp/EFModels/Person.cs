using System;
using System.Collections.Generic;

#nullable disable

namespace JIC.ChatApp.efmodels
{
    public partial class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
