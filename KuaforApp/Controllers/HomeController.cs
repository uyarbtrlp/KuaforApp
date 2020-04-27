using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using KuaforApp.Contexts;

namespace KuaforApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IdentityDataContext _context;

        public HomeController(ILogger<HomeController> logger, IdentityDataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IWebHostEnvironment env)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _env = env;

        }

        public async Task<IActionResult> Index(string? city,string? type, string? rate)
        {
            List<Store> store = new List<Store>();
            store = _context.Stores.Select(i => new Store()
            {
                Id = i.Id,
                UserId = i.UserId,
                City = i.City,
                Type = i.Type,
                Rate = i.Rate,
                Images = i.Images,
                PhoneNumber = i.PhoneNumber,
                Name = i.Name,
                District = i.District,
                Description = i.Description,
                Comments = i.Comments,
                Address = i.Address,
                ProfilePhoto=i.ProfilePhoto

                





            }).ToList();

            if (city != null)
            {
                var storex = _context.Stores.Where(i => i.City == city).Select(i => new Store()
                {
                    Id=i.Id,
                    UserId=i.UserId,
                    City=i.City,
                    Type=i.Type,
                    Rate=i.Rate,
                    Images=i.Images,
                    PhoneNumber=i.PhoneNumber,
                    Name=i.Name,
                    District=i.District,
                    Description=i.Description,
                    Comments=i.Comments,
                    Address=i.Address,
                    ProfilePhoto = i.ProfilePhoto





                });
                store = storex.ToList();
                
            }
            if (type != null)
            {
                var storex = _context.Stores.Where(i => i.Type == type && i.City==city).Select(i => new Store()
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    City = i.City,
                    Type = i.Type,
                    Rate = i.Rate,
                    Images = i.Images,
                    PhoneNumber = i.PhoneNumber,
                    Name = i.Name,
                    District = i.District,
                    Description = i.Description,
                    Comments = i.Comments,
                    Address = i.Address,
                    ProfilePhoto = i.ProfilePhoto





                });
                store = storex.ToList();

            }
            if (rate != null)
            {
                var storex = _context.Stores.Where(i=>i.Rate>=Convert.ToDouble(rate) && i.City==city &&i.Type==type).Select(i => new Store()
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    City = i.City,
                    Type = i.Type,
                    Rate = i.Rate,
                    Images = i.Images,
                    PhoneNumber = i.PhoneNumber,
                    Name = i.Name,
                    District = i.District,
                    Description = i.Description,
                    Comments = i.Comments,
                    Address = i.Address,
                    ProfilePhoto = i.ProfilePhoto





                });
                store = storex.ToList();

            }
            return View(store);







           

        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var rates = _context.Rates.Where(i => i.UserId == user.Id).ToList();
            var store = _context.Stores.Where(i => i.Id == id).Select(i => new Store()
            {
                Id = i.Id,
                Name = i.Name,
                Images = i.Images,
                Address = i.Address,
                Description = i.Description,
                PhoneNumber = i.PhoneNumber,
                Rate = i.Rate,
                Type = i.Type,
                Comments = i.Comments,
                Employees = i.Employees,
                Rates=rates



            });
            return View(store.ToList());
        }
        public async Task<IActionResult> AddComment(string commentText, int? id)
        {
            var comment = new Comment();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Stores.Where(i => i.Id == id).FirstOrDefault();
            comment.StoreModelId = store.Id;
            comment.UserName = user.UserName;
            comment.CommentText = commentText;
            comment.Photo = user.ProfilePhoto;





            await _context.Comments.AddAsync(comment);

            _context.SaveChanges();




            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AddRate(string rate, int? id)
        {
            Rate ratex = new Rate();
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Stores.Where(i => i.Id == id).FirstOrDefault();
            ratex.StoreModelId = store.Id;
            ratex.UserId = user.Id;
            ratex.RateNumber = Convert.ToInt32(rate);
            await _context.Rates.AddAsync(ratex);
            _context.SaveChanges();
            var rates = _context.Rates.Where(i => i.StoreModelId == id).ToList();
            double sumRate = 0;
            foreach (var item in rates)
            {
                sumRate += item.RateNumber;

            }
            store.Rate = Math.Round(sumRate / rates.Count(),1);
            _context.Stores.Update(store);
            _context.SaveChanges();
            












            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
