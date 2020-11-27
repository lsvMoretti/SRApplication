using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using ServerApp.Tables;

namespace ServerApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            await using Database database = new Database();

            await DiscordBot.StartDiscordBot();
        }
    }
}