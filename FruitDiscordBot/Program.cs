using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace FruitDiscordBot
{
	class Program 
	{
		private DiscordSocketClient Client;
		private CommandService Commands;


		static void Main(string[] args)
		=> new Program().MainAsync().GetAwaiter().GetResult();

		private async Task MainAsync()
		{
			Client = new DiscordSocketClient(new DiscordSocketConfig
			{
				LogLevel = LogSeverity.Debug
			});

			Commands = new CommandService(new CommandServiceConfig
			{
				CaseSensitiveCommands = true,
				DefaultRunMode = RunMode.Async,
				LogLevel = LogSeverity.Debug
			});

			Client.UserJoined += AnnounceJoinedUser;
			Client.MessageReceived += Client_MessageReceived;
			await Commands.AddModulesAsync(Assembly.GetEntryAssembly());

			Client.Ready += Client_Ready;
			Client.Log += Client_Log;

			string Token;
			using (StreamReader sr = new StreamReader("BotID.txt"))
			{
				Token = sr.ReadToEnd();
			}
			await Client.LoginAsync(TokenType.Bot, Token);
			await Client.StartAsync();
				await Task.Delay(-1);
		}

		private async Task Client_Log(LogMessage Message)
		{
			Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
		}

		private async Task Client_Ready()
		{
			await Client.SetGameAsync("Fruit Bot");
		}
		
		public async Task AnnounceJoinedUser(SocketGuildUser user) 
		{
			ulong welcomeChannelId = 0;
			string welcomeChannelIdString;
			var lines = File.ReadAllLines("config.txt");
			foreach (var line in lines)
			{
				if (line.Contains("WelcomeChannelID"))
				{
					var prefixRes = line.Replace("WelcomeChannelId:", "");
					welcomeChannelIdString = prefixRes.Trim();
					welcomeChannelId = ulong.Parse(welcomeChannelIdString);
				}
			}

			var channel = Client.GetChannel(welcomeChannelId) as SocketTextChannel; // Gets the channel to send the message in
			await channel.SendMessageAsync($"Welcome {user.Mention} to {"Fruit test..."}"); 
		}

		private async Task Client_MessageReceived(SocketMessage MessageParam)
		{

			string Prefix = String.Empty;
			var lines = File.ReadAllLines("config.txt");
			foreach (var line in lines)
			{
				if (line.Contains("Prefix"))
				{
					var prefixRes = line.Replace("Prefix:", "");
					Prefix = prefixRes.Trim();
				}
			}


			var Message = MessageParam as SocketUserMessage;
			var Context = new SocketCommandContext(Client, Message);

			if (Context.Message == null || Context.Message.Content == "") return;
			if (Context.User.IsBot) return;

			int ArgPos = 0;
			if (!(Message.HasStringPrefix(Prefix, ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

			var Result = await Commands.ExecuteAsync(Context, ArgPos);
			if (!Result.IsSuccess)
				Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
		}

	}
}
