using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ServerApp.Tables;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp
{
    public class DiscordCommands
    {
        [Command("join")]
        public async Task CommandJoinSr(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Message.DeleteAsync("!join command");

            await using Database database = new Database();

            User user = database.Users.FirstOrDefault(x => x.DiscordId == ctx.Member.Id.ToString());

            if (user != null)
            {
                // Member is already apart of the SR or in application process
                if (user.MemberStatus > MemberStatus.NonMember)
                {
                    await ctx.Member.SendMessageAsync(
                        $"Hey {ctx.Member.Mention}! I've seen you are currently either an SR member or going through the process! If this is an error, please ask in #server-help");
                    return;
                }
            }

            await ctx.RespondAsync(
                $"Hey {ctx.Member.Mention}! I'm going to send you a DM right now with some questions to answer!");

            await Task.Delay(2000);

            string nickname = ctx.Member.Nickname ?? ctx.Member.Username;

            await ApplicationProcess.StartApplicationProcess(nickname, ctx.Member);
        }
    }
}