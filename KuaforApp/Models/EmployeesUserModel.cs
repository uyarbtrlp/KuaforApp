using KuaforApp.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class EmployeesUserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string TimeOfArrival { get; set; }
        public string Transactions { get; set; }
        public string Payment { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
