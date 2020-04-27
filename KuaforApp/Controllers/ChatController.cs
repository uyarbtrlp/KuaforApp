using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KuaforApp.Contexts;
using KuaforApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KuaforApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IdentityDataContext _context;

        public ChatController(ILogger<ChatController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IWebHostEnvironment env, IdentityDataContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _context = context;

        }
        public async Task<IActionResult> ChatMessages(int id)
        {
            var chats = _context.Chats.Include(x => x.Users).Where(x => x.Users.Any(y => y.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)).ToList();
            var chat = _context.Chats.Include(b => b.Messages).Select(i=>new ChatModel() {
                Chats = chats,
                Id=i.Id,
                Messages=i.Messages,
                Name=i.Name,
                StoreUserName=i.StoreUserName,
                UserName=i.UserName,
                Users=i.Users
                
            }).FirstOrDefault(x=>x.Id==id);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.Where(i => i.Id == userId).FirstOrDefault();
            var role = await _userManager.GetRolesAsync(user);
            foreach (var item in role)
            {
                if (item == "user")
                {
                    
                    return View("ChatMessagesUser",chat);
                    
                }
                else
                {
                    
                    return View(chat);
                }
            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            var Message = new Message()
            {
                ChatId = chatId,
                Name = User.Identity.Name,
                Text = message,
                Timestamp = DateTime.Now
            };
             var user=await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.username = user.UserName;
            _context.Messages.Add(Message);
            _context.SaveChanges();
            return RedirectToAction("ChatMessages", new { id = chatId });

        }
      
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _context.Users.Where(i => i.Id == userId).FirstOrDefault();
                var role = await _userManager.GetRolesAsync(user);
                foreach (var item in role)
                {
                    if (item == "user")
                    {
                        var chats = _context.Chats.Include(x => x.Users).Where(x => x.Users.Any(y => y.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                        return View(chats);
                    }
                    else
                    {
                        var chats = _context.Chats.Include(x => x.Users).Where(x => x.Users.Any(y => y.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                        return View("Deneme",chats);
                    }
                }
                return View();


            }
            else
            {
                var storesUser = _context.Stores.Where(x => x.Id == id).Select(x => x.UserId).FirstOrDefault();
                var chats = _context.Chats.Include(x => x.Users).Where(x => x.Users.Any(y => y.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value) && x.Users.Any(y => y.UserId == storesUser)).ToList();
                if (chats.Count == 0)
                {
                    var chatId = CreateRoom(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, storesUser);
                    return RedirectToAction("ChatMessages", new { id = chatId });

                }
                else
                {
                    var chatsWithUsersId = _context.Chats.Include(x => x.Users).Where(x => x.Users.Any(y => y.UserId == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value) && x.Users.Any(y => y.UserId == storesUser)).ToList();
                    return RedirectToAction("ChatMessages", new { id = chatsWithUsersId[0].Id });
                }
            }




        }
        public IActionResult JoinedRoom()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var chats = _context.ChatUsers.Include(x => x.Chat).Where(x => x.UserId == userId).Select(x => x.Chat).ToList();



            return View(chats);
        }
        [HttpGet]
        public IActionResult JoinChat(int id)
        {
            var chatUser = new ChatUser()
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value

            };
            _context.ChatUsers.Add(chatUser);
            _context.SaveChanges();
            return RedirectToAction("ChatMessages", new { id = id });

        }
        public int CreateRoom(string currentUser, string storesUser)
        {
            var user = _context.Users.Where(i => i.Id == currentUser).FirstOrDefault();
            var storeUser = _context.Users.Where(i => i.Id == storesUser).FirstOrDefault();
            var store = _context.Stores.Where(i => i.UserId == storesUser).FirstOrDefault();


            var chat = new Chat()
            {
                Name = "deneme",
                UserName = user.Name +" "+ user.Surname,
                StoreUserName = storeUser.Name +" "+ storeUser.Surname,
                StoreProfile=store.ProfilePhoto,
                UserProfile=user.ProfilePhoto,





            };
            chat.Users.Add(new ChatUser()
            {
                UserId = currentUser

            });
            chat.Users.Add(new ChatUser()
            {
                UserId = storesUser
            });
            _context.Chats.Add(chat);



            _context.SaveChanges();
            return chat.Id;

        }
    }
}