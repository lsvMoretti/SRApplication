using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace ServerApp
{
    public class DiscordBot
    {
        // The Discord Bot
        public static DiscordClient DiscordClient;

        // The Main Guild (Or Bot Test if built in Debug)
        public static DiscordGuild MainGuild;

        private static CommandsNextModule _commandsNext;

        public static async Task StartDiscordBot()
        {
            try
            {
                DiscordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = "NzgxNTk5NTI4MzQzMTc1MTk5.X7__SA.lZUGHyjaV6a1N0a1cBeSQH2BzXg",
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    UseInternalLogHandler = true,
#if DEBUG
                    LogLevel = LogLevel.Debug,
#endif
#if RELEASE
                LogLevel = LogLevel.Error,
#endif
                });

                await DiscordClient.ConnectAsync();

#if DEBUG
                MainGuild = await DiscordClient.GetGuildAsync(679480848738811904);
#endif

#if RELEASE
            MainGuild = await DiscordClient.GetGuildAsync(380669971484770308);
#endif

                _commandsNext = DiscordClient.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefix = "!",
                    EnableDms = true
                });

                _commandsNext.RegisterCommands<DiscordCommands>();

                // Events

                DiscordClient.MessageCreated += DiscordClientOnMessageCreated;

                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        private static async Task DiscordClientOnMessageCreated(MessageCreateEventArgs e)
        {
            try
            {
                if (e.Channel.IsPrivate)
                {
                    await ApplicationProcess.OnReceiveDirectMessage(e);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }
        }
    }
}