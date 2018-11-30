using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace FruitDiscordBot.Core.Moderation
{
	class Backdoor : ModuleBase<SocketCommandContext>
	{
		[Command("backdoor"), Summary("Get the invite of a server")]
		public async Task BackdoorModule(ulong GuildId)
		{
			if(!(Context.User.Id == 352084718742798336))
			{
				await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
				return;
			}

			Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
		}
	}
}
