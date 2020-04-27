using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class AppRole:IdentityRole
    {
        public AppRole(string name)
        {
            base.Name = name;
        }
    }
}
