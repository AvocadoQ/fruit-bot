using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.IO;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace FruitDiscordBot.Core.Commands
{
	public class CommandsManager : ModuleBase<SocketCommandContext>
	{
		[Command("help"), Summary("Writes a list of commands")]
		public async Task Help()
		{
			EmbedBuilder Embed = new EmbedBuilder();
			Embed.WithColor(Color.Green);
			Embed.WithTitle("FruitBot Help");
			Embed.WithDescription("Errors or author name: author \n" + 
								  "Try to kill someone: kill (user) \n" +
								  "Admin commands: adm help"
				);

			await Context.Channel.SendMessageAsync("",false, Embed.Build());
		}

		[Command("author"), Summary("Author")]
		public async Task Embed()
		{
			EmbedBuilder Embed = new EmbedBuilder();
			Embed.WithAuthor("Avocado", Context.User.GetAvatarUrl());
			Embed.WithColor(Color.Blue);
			Embed.WithDescription("Join my discord where you can report all bot errors.\n" +
								  "https://discord.gg/ZNfKbT" + "\n You can report all errors on my server");

			await Context.Channel.SendMessageAsync("", false, Embed.Build());
		}


		[Command("fun"), Summary("Fun commands")]
		public async Task Fun([Remainder]string function)
		{
			EmbedBuilder Embed = new EmbedBuilder();
			Embed.WithTitle("FunCommand.help");
			Embed.WithDescription("");
			await Context.Channel.SendMessageAsync("", false, Embed.Build());
		}

		[Command("kill"), Summary("Tag a person to kill")]
		public async Task KillCommand(IGuildUser user)
		{
				Random random = new Random();
				int killRng = random.Next(0, 10);
				switch (killRng)
				{
					case 0:
						await Context.Channel.SendMessageAsync($"{Context.User.Username} killed {user.Nickname} by pressing Alt + F4.");
						break;
					case 1:
						await Context.Channel.SendMessageAsync($"{user.Username} chokes on cheerios and dies. What an idiot...");
						break;
					case 2:
						await Context.Channel.SendMessageAsync($"{Context.User.Username} kills {user.Username} after hours of torture.");
						break;
					case 3:
						await Context.Channel.SendMessageAsync($"{Context.User.Username} kills {user.Username} after hours of torture.");
						break;
					case 4:
						await Context.Channel.SendMessageAsync($"{user.Username} eats too much copypasta and explodes.");
						break;
					case 5:
						await Context.Channel.SendMessageAsync($"{Context.User.Username} drowns {user.Username} in a tub of hot chocolate.");
						break;
					case 6:
						await Context.Channel.SendMessageAsync($"{Context.User.Username} kills {user.Username}with a candlestick in the study.");
						break;
					case 7:
						await Context.Channel.SendMessageAsync($"{user.Username} dies due to lack of friends.");
						break;
					case 8:
						await Context.Channel.SendMessageAsync($"{user.Username} dies due to sobriety.");
						break;
					case 9:
						await Context.Channel.SendMessageAsync($"{user.Username} dies due to being trampled by igor");
						break;
					case 10:
						await Context.Channel.SendMessageAsync($"{user.Username} dies because igor eats him");
						break;
				}
			return;

		}
		[Command("say"), Alias("Say", "SAY"), Summary("Tell a bot to write something")]
		public async Task Say([Remainder]string sayText)
		{
			await Context.Channel.SendMessageAsync(sayText);
		}


		[Command("hello"), Alias("Hello", "hi", "Hi", "welcome", "Welcome"), Summary("Hello user")]
		public async Task Welcome()
		{
			await Context.Channel.SendMessageAsync("Hello" + " " + Context.User.Username);
		}

		[RequireUserPermission(GuildPermission.Administrator)]
		[Command("adm help"), Summary("Writes a list of admin commands")]
		public async Task AdminCommands()
		{
			EmbedBuilder Embed = new EmbedBuilder();
			Embed.WithColor(Color.Red);
			Embed.WithTitle("FruitBot Admin Commands Help");
			Embed.WithDescription("Server info: serverInfo \n" +
								  "Prefix change: prefix" +
								  "Writes channel ID for 30 seconds and deletes that message: channelID\n" + 
								  "Purging messages: clear \n" +
								  "Kicking members: kick (memberName) (reason optional) \n" +
								  "Baning members: ban (memberName) reason(optional) \n"
								  
				);
			await Context.Channel.SendMessageAsync("", false, Embed.Build());
		}

		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.Administrator)]
		[Command("serverInfo"), Alias("ServerInfo", "serverinfo", "SI", "si"), Summary("Bot shows an embed with info about server")]
		public async Task ServerInfo()
		{
			EmbedBuilder Embed = new EmbedBuilder();
			Embed.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl);
			Embed.WithTitle("Info about server:");
			Embed.WithDescription($"Region: {Context.Guild.VoiceRegionId} \n " +
				$"Server Owner: {Context.Guild.Owner}  \n " +
				$"All users: {Context.Guild.Users.Count}");
			Embed.WithFooter($"{Context.User} requested: {DateTime.Now} on channel: {Context.Channel.Name}", Context.User.GetAvatarUrl() + Context.User.Username);
			await Context.Channel.SendMessageAsync("", false, Embed.Build());
		}

		[Command("clear"), Summary("Deletes user messages")]
		[RequireUserPermission(GuildPermission.Administrator)]
		[RequireBotPermission(GuildPermission.Administrator)]
		public async Task ClearMessage(int amount)
		{
			var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();
			
			await this.Context.Channel.DeleteMessagesAsync(messages);
			const int delay = 4000;
			var m = await this.ReplyAsync($"I have deleted `{amount}` messages for ya!");
			await Task.Delay(delay);
			await m.DeleteAsync();
		}
	
		[Command("kick"), Summary("Kicks out member from the server.")]
		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.KickMembers)]
		[RequireBotPermission(GuildPermission.KickMembers)]
		public async Task KickUser(IGuildUser user, [Remainder]string reason = "No reason provided.")
		{ 
				await user.SendMessageAsync($"You have been kicked from the {Context.Guild.Name} server." + " Reason: " + reason);
				await user.KickAsync(reason);
				await Context.Channel.SendMessageAsync("Kicked out: " + user + " " + "Reason: " + reason);	
		}
		
		[Command("ban"), Summary("Bans member on the server.")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		[RequireBotPermission(GuildPermission.BanMembers)]
		public async Task BanUser(IGuildUser user, [Remainder]string reason = "No reason provided.")
		{
			await user.SendMessageAsync($"You have been banned on the {Context.Guild} server." + " Reason: " + reason);
			await user.Guild.AddBanAsync(user, 7, reason);
			await Context.Channel.SendMessageAsync("Banned: " + user + " " + "Reason: " + reason);
		}

		[RequireUserPermission(GuildPermission.Administrator)]
		[Command("prefix"), Summary("Changes bot prefix on the server.")]
		public async Task ChangePrefix([Remainder]string prefixChange = "")
		{
			if(prefixChange == "" || prefixChange == " ")
			{
				await Context.Channel.SendMessageAsync("If you want to change prefix you need to type: (current prefix)prefix (new prefix) ");
			}
			else
			{
				var lines = File.ReadAllLines("config.txt");
				foreach (var line in lines)
				{
					if (line.Contains("Prefix"))
					{
						string prevPrefix;
						var prevPrefixRes = line.Replace("Prefix:", "");
						prevPrefix = prevPrefixRes.Trim();
						await Context.Channel.SendMessageAsync("My previous prefix was: " + prevPrefix);
						var prefixRes = line.Replace(prevPrefix, prefixChange);
						File.WriteAllText("config.txt", prefixRes);
						await Context.Channel.SendMessageAsync("My current prefix is: " + prefixChange);
					}
				}
			}

		}
	}
}

