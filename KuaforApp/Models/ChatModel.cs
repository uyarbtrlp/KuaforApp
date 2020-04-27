using KuaforApp.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforApp.Models
{
    public class ChatModel
    {
        public ChatModel()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
            Chats = new List<Chat>();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string StoreUserName { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }
        public ICollection<Chat> Chats { get; set; }
    }
}
