using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KuaforApp.Contexts;
using Microsoft.AspNetCore.SignalR;

namespace KuaforApp.ChatHub
{
    public class ChatHub:Hub
    {

        public string GetConnectionId() => Context.ConnectionId;
    }
}
