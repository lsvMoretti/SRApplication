using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ServerApp.Tables;

namespace ServerApp
{
    public class ApplicationProcess
    {
        /// <summary>
        /// Current DM channels opened
        /// </summary>
        private static Dictionary<string, DiscordDmChannel> activeDmChannels = new Dictionary<string, DiscordDmChannel>();

        /// <summary>
        /// Current answer / application stage per user
        /// </summary>
        private static Dictionary<string, int> activeApplicationAnswers = new Dictionary<string, int>();

        /// <summary>
        /// The first message received during the application process
        /// </summary>
        /// <param name="currentNickname">Nickname when starting process</param>
        /// <param name="member"></param>
        public static async Task StartApplicationProcess(string currentNickname, DiscordMember member)
        {
            try
            {
                DiscordDmChannel newDmChannel;

                if (activeDmChannels.ContainsKey(member.Id.ToString()))
                {
                    newDmChannel = activeDmChannels.FirstOrDefault(x => x.Key == member.Id.ToString()).Value;
                }
                else
                {
                    newDmChannel = await DiscordBot.DiscordClient.CreateDmAsync(member);
                    activeDmChannels.Add(member.Id.ToString(), newDmChannel);
                }

                await newDmChannel.TriggerTypingAsync();

                await newDmChannel.SendMessageAsync(
                    $"Hey {member.Mention}! Thanks for thinking about applying to Smoking Rifles! You won't regret this!");

                await using Database database = new Database();

                User newUser = new User
                {
                    DiscordId = member.Id.ToString(),
                    Username = currentNickname,
                    ApplicationDate = DateTime.UtcNow
                };

                await database.Users.AddAsync(newUser);

                await database.SaveChangesAsync();

                await newDmChannel.TriggerTypingAsync();

                await Task.Delay(3000);

                await newDmChannel.SendMessageAsync(
                    "Please note, this process is automated to help the Admissions Team get to you faster! Please only respond with the answer one message at a time. Take your time!");

                await newDmChannel.TriggerTypingAsync();

                await Task.Delay(3000);

                await newDmChannel.SendMessageAsync(
                    $"The first thing I want you to do is link me your steamID64, this is so we can verify who you are! \n To do this, the best way is to head over to here and enter your steam username!" +
                    $"\n https://steamid.io/lookup");

                await Task.Delay(5000);

                await newDmChannel.TriggerTypingAsync();

                await newDmChannel.SendMessageAsync("Once this is done, just reply here with only your SteamID64 link!");

                activeApplicationAnswers.Add(newUser.DiscordId.ToString(), 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        /// <summary>
        /// When a DM is received, this event is fired
        /// </summary>
        /// <param name="e"></param>
        /// <returns>False = Not in application stage</returns>
        public static async Task OnReceiveDirectMessage(MessageCreateEventArgs e)
        {
            try
            {
                bool hasApplicationProcess = activeApplicationAnswers.TryGetValue(e.Author.Id.ToString(), out int currentStage);

                if (!hasApplicationProcess) return;

                await using Database database = new Database();

                User currentUser = database.Users.FirstOrDefault(x => x.DiscordId == e.Author.Id.ToString());

                if (currentUser == null) return;

                bool hasActiveDmChannel = activeDmChannels.TryGetValue(e.Author.Id.ToString(), out DiscordDmChannel activeDmChannel);

                if (!hasActiveDmChannel) return;

                if (currentStage == 0)
                {
                    // First stage - Replying with SteamID64
                    await activeDmChannel.TriggerTypingAsync();

                    ulong steam64Result = 0;

                    try
                    {
                        steam64Result = Convert.ToUInt64(e.Message.Content);
                    }
                    catch (Exception exception)
                    {
                        await activeDmChannel.SendMessageAsync(
                            $"I've had an issue trying to make sure your ID is correct. Make sure it's your steamID64!");
                        Console.WriteLine(exception);
                        return;
                    }

                    if (steam64Result == 0)
                    {
                        await activeDmChannel.SendMessageAsync(
                            $"I've had an issue trying to make sure your ID is correct. Make sure it's your steamID64!");
                        return;
                    }

                    currentUser.Steam64 = steam64Result.ToString();
                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(
                        $"I've found the following profile. Please type yes or no if this is your profile! http://steamcommunity.com/profiles/{steam64Result}");

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 1);
                }

                if (currentStage == 1)
                {
                    // Asked yes or no to confirm steam 64
                    if (e.Message.Content.ToLower().Contains("ye"))
                    {
                        await activeDmChannel.TriggerTypingAsync();

                        activeApplicationAnswers.Remove(currentUser.DiscordId);
                        activeApplicationAnswers.Add(currentUser.DiscordId, 1);

                        await activeDmChannel.SendMessageAsync("Great! Onto the next stage!");
                        // TODO Next question here
                        return;
                    }

                    await activeDmChannel.TriggerTypingAsync();

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 0);

                    await activeDmChannel.SendMessageAsync("Please try again, using the steamid.io link above.");
                    return;
                }
            }
            catch (Exception exception)
            {
                await e.Channel.SendMessageAsync("There was an error. Check your answer and try again!");
                Console.WriteLine(exception);
                return;
            }
        }
    }
}