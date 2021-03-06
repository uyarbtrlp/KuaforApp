﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class AdminUserModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "İsim gereklidir.")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Soyisim gereklidir.")]
        public string Surname { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Lütfen email adresinizi düzgün giriniz")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Şifre gereklidir gereklidir.")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Telefon numarası gereklidir.")]
        public string PhoneNumber { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Adres gereklidir.")]
        public string Adress { get; set; }        
        public string ProfilePhoto { get; set; }
    }
}
