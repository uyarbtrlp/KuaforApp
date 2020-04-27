using KuaforApp.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string CommentText { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Açıklama gereklidir.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Dükkan tipi gereklidir.")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Dükkan adresi gereklidir.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Dükkanın telefon numarası gereklidir.")]
        public string PhoneNumber { get; set; }
        
        public string ProfilePhoto { get; set; }
        public double Rate { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Şehir gereklidir.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Semt gereklidir.")]
        public string District { get; set; }

        public List<Image> Images { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Product> Products { get; set; }
        public List<ApprovedUser> ApprovedUsers { get; set; }
        public List<Rate> Rates { get; set; }

    }
}
