using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Timers;
using System;

using BehaveBot.Modules;
using BehaveBot.Services;

namespace BehaveBot.Classes
{
    public class InvitationManager
    {
        public delegate void InvitationHandler(object sender);
        public event InvitationHandler OnInvitationTimedOut;

        public SocketGuildUser user;
        public SocketRole role;
        Timer MessageTimer = new Timer(1000);
        DateTime ExpireTime = DateTime.Now;

        public InvitationManager(SocketGuildUser _user, SocketRole _role)
        {
            user = _user;
            role = _role;

            ExpireTime = DateTime.Now.AddHours(90);
            MessageTimer.Elapsed += MessageTimer_Elapsed;
            MessageTimer.Enabled = true;
            MessageTimer.Start();
        }

        public InvitationManager(SocketGuildUser _user, SocketRole _role, int time)
        {
            user = _user;
            role = _role;

            ExpireTime = DateTime.Now.AddHours(time);
            MessageTimer.Elapsed += MessageTimer_Elapsed;
            MessageTimer.Enabled = true;
            MessageTimer.Start();
        }

        private void MessageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now > ExpireTime)
            {
                MessageTimer.Stop();
                OnInvitationTimedOut?.Invoke(this);
            }
        }
    }
}
