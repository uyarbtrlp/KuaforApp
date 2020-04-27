using KuaforApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Contexts
{
    public class IdentityDataContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options):base(options)
        {

        }
        public DbSet<StoreModel> Stores { get; set; }
        public DbSet<Image> Images{ get; set; }
        public DbSet<Comment> Comments{ get; set; }
        public DbSet<Employee> Employees{ get; set; }
        public DbSet<EmployeesUser> EmployeesUsers { get; set; }
        public DbSet<ApprovedUser> ApprovedUser { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<StoreModel>();
            builder.Entity<Image>();
            builder.Entity<Comment>();
            builder.Entity<Employee>();
            builder.Entity<EmployeesUser>();
            builder.Entity<ApprovedUser>();
            builder.Entity<Product>();
            builder.Entity<Rate>();
            builder.Entity<Chat>();
            builder.Entity<Message>();
            builder.Entity<ChatUser>().HasKey(x=> new { x.ChatId,x.UserId});
            
        }
        public DbSet<KuaforApp.Models.EmployeesUserModel> EmployeesUserModel { get; set; }
    }

    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StoreModel Store { get; set; }
        public int StoreModelId { get; set; }
    }
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserProfile { get; set; }
        public string Name { get; set; }
        public string StoreUserName { get; set; }
        public string StoreProfile { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }
        
    }
    public class Message
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Text{ get; set; }
        public DateTime Timestamp{ get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
   

    public class StoreModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Dükkan ismi gereklidir.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Şehir gereklidir.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Semt gereklidir.")]
        public string District { get; set; }
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
        public string UserId{ get; set; }
        
        public List<Image> Images { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Product> Products { get; set; }
        public List<Rate> Rates { get; set; }
    }
    public class Rate
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int StoreModelId { get; set; }
        public int RateNumber { get; set; }
    }
    public class Comment
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string CommentText { get; set; }
        public string Photo { get; set; }

        public StoreModel Store { get; set; }
        public int StoreModelId { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Rate { get; set; }
        public string Hour { get; set; }
        public float Salary { get; set; }
        public StoreModel Store { get; set; }
        public int StoreModelId { get; set; }
    }
    public class EmployeesUser
    {
        
        public int Id { get; set; }
        [DisplayName("İsim")]
        public string Name { get; set; }
        [DisplayName("Soyisim")]
        public string Surname { get; set; }
        [DisplayName("Telefon Numarası")]
        public string PhoneNumber { get; set; }
        [DisplayName("Geldiği Saat")]
        public string TimeOfArrival { get; set; }
        [DisplayName("Yapılacak İşlemler")]
        public string Transactions { get; set; }
        [DisplayName("Ödeme")]
        public string Payment { get; set; }
        [DisplayName("Çalışan Id'si")]

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
    public class ApprovedUser
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
    public class Product
    {
        public int Id { get; set; }
        [DisplayName("İsmi")]
        [Required(ErrorMessage ="Ürün ismi girmeniz gereklidir.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Ürün fiyatı girmeniz gereklidir.")]
        [DisplayName("Fiyatı")]
        public float Price { get; set; }
        [Required(ErrorMessage = "Ürün açıklaması girmeniz gereklidir.")]
        [DisplayName("Açıklaması")]
        public string Description { get; set; }
        [DisplayName("Dükkan İsmi")]
        public StoreModel Store { get; set; }
        [DisplayName("Dükkan Id'si")]
        public int StoreModelId { get; set; }

    }
    public class ChatUser
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}

