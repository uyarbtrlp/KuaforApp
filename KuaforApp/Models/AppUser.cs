using KuaforApp.Contexts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class AppUser:IdentityUser
    {
        public string Adress { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ProfilePhoto { get; set; }
        public ICollection<ChatUser> Chats { get; set; }
    }
}

