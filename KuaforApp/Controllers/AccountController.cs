using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KuaforApp.Contexts;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KuaforApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IdentityDataContext _context;

        public AccountController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IWebHostEnvironment env, IdentityDataContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _context = context;

        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var store = _context.Stores.Where(i => i.UserId == user.Id).Select(i => new Store()
            {
                Name = i.Name,
                Images = i.Images,
                Address = i.Address,
                Description = i.Description,
                City = i.City,
                District = i.District,
                PhoneNumber = i.PhoneNumber,
                Rate = i.Rate,
                Type = i.Type,
                Comments = i.Comments,
                Employees = i.Employees



            });
            return View(store.ToList());
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new AppRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult AdminRegister()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(AdminUserModel userModel, IFormFile file)
        {
            string fileName = "";

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var extensition = Path.GetExtension(file.FileName);
                    if (extensition == ".jpg" || extensition == ".png")
                    {
                        var dir = _env.ContentRootPath + "\\upload";
                        var randomFilename = Path.GetRandomFileName();
                        fileName = Path.ChangeExtension(randomFilename, ".jpg");
                        var path = Path.Combine(dir, fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                            AppUser user = new AppUser()
                            {
                                Name = userModel.Name,
                                Surname = userModel.Surname,
                                UserName = userModel.Username,
                                Email = userModel.Email,
                                PhoneNumber = userModel.PhoneNumber,
                                Adress = userModel.Adress,

                                ProfilePhoto = fileName





                            };
                            var result = await _userManager.CreateAsync(user, userModel.Password);
                            if (result.Succeeded)
                            {


                                await _userManager.AddToRoleAsync(user, "admin");


                                return RedirectToAction("Login");
                            }
                            else
                            {
                                foreach (var item in result.Errors)
                                {
                                    if (item.Code == "PasswordRequiresNonAlphanumeric")
                                        ModelState.AddModelError("", "Şifre en az bir adet simge içermelidir.");
                                    else if (item.Code == "PasswordRequiresLower")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet küçük harf içermelidir.");
                                    }
                                    else if (item.Code == "PasswordRequiresUpper")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet büyük harf içermelidir.");
                                    }
                                    else if (item.Code == "DuplicateUserName")
                                    {
                                        ModelState.AddModelError("", "Kullanıcı adı " + userModel.Username + " şu anda kullanılıyor. Lütfen başka kullanıcı adı seçiniz.");
                                    }
                                    else if (item.Code == "DuplicateEmail")
                                    {
                                        ModelState.AddModelError("", userModel.Email + " adında zaten üyeliğimiz bulunmaktadır. Şifrenizi unuttuysanız şifrenizi sıfırlayınız.");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Resim dosyası seçiniz");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Bir dosya seçiniz");
                }




            }
            return View(userModel);


        }
        [Authorize(Roles = "admin")]
        public IActionResult AddStore()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStore(StoreModel model, IEnumerable<IFormFile> files, IFormFile fileProfile)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var storex = _context.Stores.Where(i => i.UserId == user.Id);
            if (storex.Count() > 0)
            {
                ModelState.AddModelError("", "Zaten bir dükkanınız bulunmakta.");
            }
            else
            {
                if (ModelState.IsValid)
                {

                    var list = new List<Image>();
                    string fileName = "";
                    string fileNamee = "";
                    int i = 0;
                    if (files.Count() > 10)
                    {
                        ModelState.AddModelError("", "10 adet fotoğraf yüklemelisiniz.");
                    }
                    else if (files.Count() <= 0)
                    {
                        ModelState.AddModelError("", "Fotoğraf yüklemelisiniz.");
                    }
                    else
                    {
                        if (fileProfile == null || fileProfile.Length <= 0)
                        {
                            ModelState.AddModelError("", "Profil fotoğrafı seçmelisiniz.");
                        }
                        else
                        {
                            var extensition = Path.GetExtension(fileProfile.FileName);
                            if (extensition == ".jpg" || extensition == ".png")
                            {
                                var dir = _env.ContentRootPath + "\\upload";
                                var randomFilename = Path.GetRandomFileName();
                                fileNamee = Path.ChangeExtension(randomFilename, ".jpg");
                                var path = Path.Combine(dir, fileNamee);
                                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                                {
                                    fileProfile.CopyTo(fileStream);
                                    foreach (var file in files)
                                    {
                                        var dirr = _env.ContentRootPath + "\\upload";
                                        var randomFilenamee = Path.GetRandomFileName();
                                        fileName = Path.ChangeExtension(randomFilenamee, ".jpg");
                                        var pathh = Path.Combine(dir, fileName);

                                        using (var filestream = new FileStream(pathh, FileMode.Create, FileAccess.Write))
                                        {

                                            list.Add(new Image() { Name = fileName });

                                            file.CopyTo(filestream);

                                        }

                                    }


                                    var store = new StoreModel()
                                    {
                                        Address = model.Address,
                                        Description = model.Description,
                                        Name = model.Name,
                                        PhoneNumber = model.PhoneNumber,
                                        Rate = 0,
                                        Type = model.Type,
                                        UserId = user.Id,
                                        City = model.City,
                                        District = model.District,
                                        Images = list,
                                        ProfilePhoto=fileNamee




                                    };
                                    await _context.Stores.AddAsync(store);
                                    await _context.SaveChangesAsync();







                                    return RedirectToAction("Index");


                                }
                            }

                        }

                    }
                }


                else
                {
                    return View(model);

                }
            }
            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult AddComment()
        {

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string commentText, int? id)
        {

            var comment = new Comment();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Stores.Where(i => i.Id == id).FirstOrDefault();
            comment.StoreModelId = store.Id;
            comment.UserName = user.UserName;
            comment.CommentText = commentText;





            await _context.Comments.AddAsync(comment);

            _context.SaveChanges();




            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel userModel, string returnUrl)
        {
            
           
            if (ModelState.IsValid)
            {


                var user = await _userManager.FindByNameAsync(userModel.Username);

                if (user != null)
                {
                    var store = _context.Stores.Where(i => i.UserId == user.Id).ToList();
                    var signInResult = await _signInManager.PasswordSignInAsync(userModel.Username, userModel.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        var role = await _userManager.GetRolesAsync(user);

                        foreach (var item in role)
                        {
                            if (item == "admin")
                            {
                                if (store.Count() == 0)
                                {
                                    return Redirect("/Account/AddStore" );
                                }
                                else
                                {
                                    return Redirect(string.IsNullOrEmpty(returnUrl) ? "/Account" : returnUrl);
                                }
                                
                            }
                            else
                            {
                                return Redirect(string.IsNullOrEmpty(returnUrl) ? "/Home" : returnUrl);
                            }
                        }










                    }
                    else
                    {
                        ModelState.AddModelError("", "Yanlış kullanıcı adı veya şifre girdiniz.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı bulunamadı. Lütfen üye olunuz.");
                }

            }
            ViewBag.returnUrl = returnUrl;


            return View(userModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel userModel, IFormFile file)
        {
            string fileName = "";

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var extensition = Path.GetExtension(file.FileName);
                    if (extensition == ".jpg" || extensition == ".png")
                    {
                        var dir = _env.ContentRootPath + "\\upload";
                        var randomFilename = Path.GetRandomFileName();
                        fileName = Path.ChangeExtension(randomFilename, ".jpg");
                        var path = Path.Combine(dir, fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                            AppUser user = new AppUser()
                            {
                                Name = userModel.Name,
                                Surname = userModel.Surname,
                                UserName = userModel.Username,
                                Email = userModel.Email,
                                PhoneNumber = userModel.PhoneNumber,
                                ProfilePhoto = fileName





                            };
                            var result = await _userManager.CreateAsync(user, userModel.Password);
                            if (result.Succeeded)
                            {


                                await _userManager.AddToRoleAsync(user, "user");


                                return RedirectToAction("Login");
                            }
                            else
                            {
                                foreach (var item in result.Errors)
                                {
                                    if (item.Code == "PasswordRequiresNonAlphanumeric")
                                        ModelState.AddModelError("", "Şifre en az bir adet simge içermelidir.");
                                    else if (item.Code == "PasswordRequiresLower")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet küçük harf içermelidir.");
                                    }
                                    else if (item.Code == "PasswordRequiresUpper")
                                    {
                                        ModelState.AddModelError("", "Şifre en az bir adet büyük harf içermelidir.");
                                    }
                                    else if (item.Code == "DuplicateUserName")
                                    {
                                        ModelState.AddModelError("", "Kullanıcı adı " + userModel.Username + " şu anda kullanılıyor. Lütfen başka kullanıcı adı seçiniz.");
                                    }
                                    else if (item.Code == "DuplicateEmail")
                                    {
                                        ModelState.AddModelError("", userModel.Email + " adında zaten üyeliğimiz bulunmaktadır. Şifrenizi unuttuysanız şifrenizi sıfırlayınız.");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Resim dosyası seçiniz");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Bir dosya seçiniz");
                }




            }
            return View(userModel);

        }
        public IActionResult AddEmployee()
        {
            return View();
        }
        public async Task<IActionResult> Employees()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var store = _context.Stores.Where(i => i.UserId == user.Id).Select(i => new Store()
            {
                Name = i.Name,
                Images = i.Images,
                Address = i.Address,
                Description = i.Description,
                PhoneNumber = i.PhoneNumber,
                Rate = i.Rate,
                Type = i.Type,
                Comments = i.Comments,
                Employees = i.Employees



            });
            return View(store.ToList());

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Stores.Where(i => i.UserId == user.Id).FirstOrDefault();
            employee.StoreModelId = store.Id;
            await _context.Employees.AddAsync(employee);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(int? id, string name, string surname, string phone, string hour, string transactions, string payment)
        {
            var employee = _context.Employees.Where(i => i.Id == id).FirstOrDefault();
            var user = new EmployeesUser()
            {
                Name = name,
                Surname = surname,
                PhoneNumber = phone,
                TimeOfArrival = hour,
                Payment = payment,
                Transactions = transactions,
                EmployeeId = employee.Id
            };


            await _context.EmployeesUsers.AddAsync(user);
            _context.SaveChanges();
            return RedirectToAction("DetailsUser", new { id = employee.Id });
        }
        public IActionResult DetailsUser(int? id)
        {
            if (id != null)
            {
                var employee = _context.Employees.Where(i => i.Id == id).FirstOrDefault();
                var users = _context.EmployeesUsers.Where(i => i.EmployeeId == employee.Id);
                return View(users.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }


        public IActionResult Approve(int? id)
        {
            var employeeUser = _context.EmployeesUsers.Where(i => i.Id == id).FirstOrDefault();
            var employee = _context.Employees.Where(i => i.Id == employeeUser.EmployeeId).FirstOrDefault();
            var approvedUser = new ApprovedUser()
            {

                Employee = employeeUser.Employee,
                EmployeeId = employeeUser.EmployeeId,
                Name = employeeUser.Name,
                PhoneNumber = employeeUser.PhoneNumber,
                Surname = employeeUser.Surname,
                TimeOfArrival = employeeUser.TimeOfArrival,
                Payment = employeeUser.Payment,
                Transactions = employeeUser.Transactions

            };

            _context.EmployeesUsers.Remove(employeeUser);
            _context.ApprovedUser.Add(approvedUser);

            _context.SaveChanges();

            return RedirectToAction("DetailsUser", new { id = employee.Id });
        }

        public async Task<IActionResult> UpdateUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeesUser = await _context.EmployeesUsers.FindAsync(id);
            if (employeesUser == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", employeesUser.EmployeeId);
            return View(employeesUser);
        }

        // POST: EmployeesUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(int id, [Bind("Id,Name,Surname,PhoneNumber,TimeOfArrival,Transactions,Payment,EmployeeId")] EmployeesUser employeesUser)
        {
            if (id != employeesUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeesUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeesUserExists(employeesUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DetailsUser", new { id = employeesUser.EmployeeId });
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", employeesUser.EmployeeId);
            return RedirectToAction("DetailsUser", new { id = employeesUser.EmployeeId });




        }

        private bool EmployeesUserExists(int ıd)
        {
            throw new NotImplementedException();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeesUser = await _context.EmployeesUsers
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeesUser == null)
            {
                return NotFound();
            }

            return View(employeesUser);
        }

        // POST: EmployeesUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeesUser = await _context.EmployeesUsers.FindAsync(id);
            _context.EmployeesUsers.Remove(employeesUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("DetailsUser", new { id = employeesUser.EmployeeId });
        }
        public async Task<IActionResult> GeneralSummary()
        {
            //hata var düzelt.
            float total = 0;
            int totalP = 0;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var store = _context.Stores.Where(i => i.UserId == user.Id).FirstOrDefault();
            var products = _context.Products.Where(p => p.StoreModelId == store.Id);
            var employees = _context.Employees.Where(i => i.StoreModelId == store.Id).ToList();
            List<ApprovedUser> approvedUsers =new List<ApprovedUser>();


            List<ApprovedUser> approvedUsersList = _context.ApprovedUser.ToList();

            int employeeCount = 0;
            int userCount = 0;
           
            var stores = _context.Stores.Where(i => i.UserId == user.Id).Select(store =>

            new Store()
            {
                Id = store.Id,
                Name = store.Name,
                Description = store.Description,
                Address = store.Address,
                Employees = store.Employees,
                Comments = store.Comments,
                Images = store.Images,
                Products = products.ToList(),
                PhoneNumber = store.PhoneNumber,
                Rate = Convert.ToDouble(store.Rate),
                Type = store.Type,
                UserId = store.UserId,
                ApprovedUsers = approvedUsersList





            }).ToList();



            foreach (var item in stores)
            {
                foreach (var product in item.Products)
                {



                    total += product.Price;
                    totalP += 1;

                }
                foreach (var employee in item.Employees)
                {
                    
                    approvedUsers.AddRange(_context.ApprovedUser.Where(i => i.EmployeeId == employee.Id).ToList());
                    employeeCount += 1;
                    foreach (var userApproved in approvedUsers.ToList())
                    {
                        if (userApproved.EmployeeId == employee.Id)
                        {
                            userCount += 1;
                        }
                       

                    }

                }

            }
            ViewBag.EmployeeCount = employeeCount;
            ViewBag.TotalPrice = total;
            ViewBag.TotalProduct = totalP;
            ViewBag.UserCount = userCount;
            var storess = _context.Stores.Where(i => i.UserId == user.Id).Select(store =>

           new Store()
           {
               Id = store.Id,
               Name = store.Name,
               Description = store.Description,
               Address = store.Address,
               Employees = store.Employees,
               Comments = store.Comments,
               Images = store.Images,
               Products = products.ToList(),
               PhoneNumber = store.PhoneNumber,
               Rate = Convert.ToDouble(store.Rate),
               Type = store.Type,
               UserId = store.UserId,
               ApprovedUsers = approvedUsers





           }).ToList();
            return View(storess); ;
        }


    }
}
