using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DiscordBotJDT.commands;
using DiscordBotJDT.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotJDT
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }

        public static List<QQueueTask> QuestionQueueTask = new List<QQueueTask>();

        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordCfg = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordCfg);

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;
            Client.MessageCreated += MessageCreatedHandler;
            Client.MessageReactionAdded += ReactionAdded;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                IgnoreExtraArguments = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Commands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (!e.User.IsBot)
            {
                DiscordMessage Msg = e.Message;
                DiscordMember _user = (DiscordMember)e.User;

                // Reaction added to QuestionQueueTask
                if (QuestionQueueTask.Any(q => q.DiscordMsg == Msg))
                {
                    if (_user.Roles.Any(role => role.Name == "Prog.Teacher"))
                    {
                        QQueueTask queueTask = QuestionQueueTask.Find(q => q.DiscordMsg == Msg);
                        if (e.Emoji.Name == "\u2705")
                        {
                            queueTask.AssignedTeacher = (DiscordUser)_user;
                            queueTask.IsCompleted = true;
                            await queueTask.Update(e);
                        }
                        else
                        {
                            queueTask.AssignedTeacher = (DiscordUser)_user;
                            await queueTask.Update(e);
                        }
                    }
                }
            }
        }
        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                if (e.Message.MentionedRoles.Count > 0 && e.Message.MentionedRoles.Any(r => r.Name == "Prog.Teacher"))
                {
                    DiscordRole _role = e.Message.MentionedRoles.FirstOrDefault(r => r.Name == "Prog.Teacher");
                    DiscordUser _user = e.Author;

                    if (_role.IsMentionable)
                    {
                        bool isInVoiceChannel = false;
                        DiscordChannel VoiceChannel = e.Guild.VoiceStates[_user.Id].Channel;
                        string ChannelMention = "";
                        try
                        {
                            if (e.Guild.VoiceStates.Any(vc => vc.Value.User == _user))
                            {
                                isInVoiceChannel = true;
                                ChannelMention = VoiceChannel.Parent.Mention + " - " + VoiceChannel.Mention;
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);

                        }
                        var interact = Program.Client.GetInteractivity();
                        DiscordEmoji[] reactEmojis =
                        {
                            DiscordEmoji.FromName(Program.Client, ":white_check_mark:"),
                        };

                        var message = new DiscordEmbedBuilder()
                        {
                            Color = DiscordColor.Orange,
                            Title = "[ Spørsmål Kø ]",
                            Description =
                                $"**Bruker:** {_user.Mention} {(isInVoiceChannel ? " er i Kanalen: " + ChannelMention : "")} \n\n" +
                                $"{e.Message.Content} \n" +
                                $"\n **Status:** Ikke Ferdig \n",
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                            {
                                Height = 100,
                                Width = 100,
                                Url = _user.AvatarUrl
                            },
                        };




                        var messageSent = await e.Channel.SendMessageAsync(embed: message);
                        var QueuTask = new QQueueTask(messageSent, _user, e.Message.Content);

                        if (e.Message.Content.ToLower().Contains("c#"))
                            QueuTask.codeLanguages.Add("C#");

                        if (e.Message.Content.ToLower().Contains("js") || e.Message.Content.ToLower().Contains("javascript"))
                            QueuTask.codeLanguages.Add("JavaScript");

                        if (e.Message.Content.ToLower().Contains("css"))
                            QueuTask.codeLanguages.Add("CSS");

                        if (e.Message.Content.ToLower().Contains("html"))
                            QueuTask.codeLanguages.Add("HTML");

                        if (e.Message.Content.ToLower().Contains("sql"))
                            QueuTask.codeLanguages.Add("SQL");

                        QuestionQueueTask.Add(QueuTask);
                        QueuTask.Update(e);

                        e.Message.DeleteAsync("BOT Removal");
                    }
                }
            }
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
