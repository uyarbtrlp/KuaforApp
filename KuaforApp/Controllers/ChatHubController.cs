using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KuaforApp.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KuaforApp.Controllers
{
    [Route("[controller]")]
    public class ChatHubController : Controller
    {
        private  IHubContext<ChatHub.ChatHub> _chat;

        public ChatHubController(IHubContext<ChatHub.ChatHub> chat)
        {
            _chat = chat;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId,string roomName,int chatId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId,roomName);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(int chatId,string message,string roomName,[FromServices] IdentityDataContext _context)
        {
            var Message = new Message()
            {
                ChatId = chatId,
                Name = User.Identity.Name,
                Text = message,
                Timestamp = DateTime.Now
            };
            
           
            _context.Messages.Add(Message);
            _context.SaveChanges();
            await _chat.Clients.Group(roomName).SendAsync("ReceiveMessage",new {
                text=Message.Text,
                name=Message.Name,
                timestamp=Message.Timestamp.ToString("dd/MM/yyyy HH:mm:ss")
            });
            return Ok();
        }
    }
}