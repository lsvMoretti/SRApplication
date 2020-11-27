using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.VisualBasic;
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

                User user = database.Users.FirstOrDefault(x => x.DiscordId == member.Id.ToString());

                if (user == null)
                {
                    user = new User
                    {
                        DiscordId = member.Id.ToString(),
                        Username = currentNickname,
                        ApplicationDate = DateTime.UtcNow,
                        BirthDate = DateTime.MinValue
                    };

                    await database.Users.AddAsync(user);

                    await database.SaveChangesAsync();
                }

                await newDmChannel.TriggerTypingAsync();

                await newDmChannel.SendMessageAsync(
                    "Please note, this process is automated to help the Admissions Team get to you faster! Please only respond with the answer one message at a time. Take your time!");

                await newDmChannel.TriggerTypingAsync();

                await newDmChannel.SendMessageAsync(
                    $"The first thing I want you to do is link me your steamID64, this is so we can verify who you are! \n To do this, the best way is to head over to here and enter your steam username!" +
                    $"\n https://steamid.io/lookup");

                await newDmChannel.TriggerTypingAsync();

                await newDmChannel.SendMessageAsync("Once this is done, just reply here with only your SteamID64!");

                activeApplicationAnswers.Add(user.DiscordId, 0);
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

                    ulong steam64Result;

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
                        activeApplicationAnswers.Add(currentUser.DiscordId, 2);

                        await activeDmChannel.SendMessageAsync("Great! Onto the next stage!");

                        await activeDmChannel.TriggerTypingAsync();

                        await activeDmChannel.SendMessageAsync(
                            "Next, we ask for your Date of Birth. This is to make sure you conform the minimum age of 17 and of course, get those birthday messages!\n\n" +
                            "Please use the following format. DD/MM/YYYY\n\nExample: 25/08/2020");
                        return;
                    }

                    await activeDmChannel.TriggerTypingAsync();

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 0);

                    await activeDmChannel.SendMessageAsync("Please try again, using the steamid.io link above.");
                    return;
                }

                if (currentStage == 2)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    bool tryBirthDateParse = DateTime.TryParse(e.Message.Content, out DateTime birthDate);

                    if (!tryBirthDateParse)
                    {
                        await activeDmChannel.SendMessageAsync(
                            "It looks like there was an error. Check above for an example!");
                        return;
                    }

                    currentUser.BirthDate = birthDate;

                    Debug.WriteLine($"Calculated Age: {currentUser.Age}");

                    if (currentUser.Age < 17)
                    {
                        // Not 17 years old
                        currentUser.AdmissionNotes = currentUser.AdmissionNotes + $"\n[AUTO] [{DateTime.UtcNow}] - Declined due to being under 17. Calculated age: {currentUser.Age}";

                        await database.SaveChangesAsync();

                        await activeDmChannel.SendMessageAsync(
                            $"Hey! Sorry to inform you, Smoking Rifles have a minimum age of 17. Your calculated age was {currentUser.Age}. You've been automatically declined from SR.");

                        activeDmChannels.Remove(currentUser.DiscordId);
                        activeApplicationAnswers.Remove(currentUser.DiscordId);
                        return;
                    }

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(
                        $"Thanks for this information! It's been added onto your application.");

                    await activeDmChannel.SendMessageAsync(
                        "So, which country are you from? We are a european clan and have members from all over the world!");

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 3);
                }

                if (currentStage == 3)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    // Country Answer
                    List<string> cultureList = new List<string>();

                    CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                    foreach (var cultureInfo in getCultureInfo)
                    {
                        RegionInfo getRegionInfo = new RegionInfo(cultureInfo.LCID);

                        if (!cultureList.Contains(getRegionInfo.EnglishName))
                        {
                            cultureList.Add(getRegionInfo.EnglishName);
                        }
                    }

                    string regionName = cultureList.FirstOrDefault(x => x.ToLower() == e.Message.Content.ToLower());

                    if (regionName == null)
                    {
                        await activeDmChannel.SendMessageAsync(
                            "I was unable to find the country! Please use the english name. Here's some examples!" +
                            "\nUnited Kingdom\nBelgium\nUnited States\nGermany\nNetherlands");

                        return;
                    }

                    currentUser.Country = regionName;

                    await activeDmChannel.SendMessageAsync($"Thanks for that! Now, the next question.");

                    await activeDmChannel.TriggerTypingAsync();

                    string question = "So, tell us. How did you find out about the Smoking Rifles community?\nRemember, to only reply in one message, not multiple.";

                    currentUser.AppQuestionOne = question;

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(question);

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 4);

                    return;
                }

                if (currentStage == 4)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    currentUser.AppAnswerOne = e.Message.Content;

                    string question =
                        "Nice! So, tell me. Why do you want to be part of the Smoking Rifles community?\nRemember, like last time to only reply in one message.";

                    currentUser.AppQuestionTwo = question;

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(question);

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 5);
                    return;
                }

                if (currentStage == 5)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    currentUser.AppAnswerTwo = e.Message.Content;

                    string question =
                        "Did someone refer you to the clan, or maybe someone who inspired you to join through their game play? If so, just write down if you can remember. If not, just put no.";

                    currentUser.AppQuestionThree = question;

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(question);

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 6);
                    return;
                }

                if (currentStage == 6)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    currentUser.AppAnswerThree = e.Message.Content;

                    string question =
                        "Cool! So, the last thing I want to know, what is your experience with Discord? It only needs to be a small reply. We are able to point you about if you don't use Discord much!\nThis just helps tailor the experience.";

                    currentUser.AppQuestionFour = question;

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(question);

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeApplicationAnswers.Add(currentUser.DiscordId, 7);
                    return;
                }

                if (currentStage == 7)
                {
                    await activeDmChannel.TriggerTypingAsync();

                    currentUser.AppAnswerFour = e.Message.Content;

                    string reply =
                        "Thanks for the information! Please wait for some more information whilst this gets sorted.\nI'll be leaving this channel in a minute.";

                    // TODO Trigger App-Pending stuff

                    currentUser.MemberStatus = MemberStatus.ApplicantPending;

                    await database.SaveChangesAsync();

                    await activeDmChannel.SendMessageAsync(reply);

                    await activeDmChannel.DeleteAsync();

                    activeApplicationAnswers.Remove(currentUser.DiscordId);
                    activeDmChannels.Remove(currentUser.DiscordId);
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