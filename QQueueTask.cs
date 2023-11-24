using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBotJDT
{
    public class QQueueTask
    {
        public DiscordUser AssignedTeacher { get; set; }
        public DiscordUser Student { get; set; }
        public DiscordMessage DiscordMsg { get; set; }
        public bool IsCompleted { get; set; }
        private string Content { get; set; }
        public List<string> codeLanguages = new List<string>();

        public QQueueTask(DiscordMessage _discordMsg, DiscordUser _student, string _content, DiscordUser _assignedTeacher = null)
        {
            this.DiscordMsg = _discordMsg;
            this.Student = _student;
            this.AssignedTeacher = _assignedTeacher;
            this.IsCompleted = false;
            this.Content = _content;
        }

        public async Task Update(MessageReactionAddEventArgs e)
        {
            bool isInVoiceChannel = false;
            DiscordChannel VoiceChannel = e.Guild.VoiceStates[Student.Id].Channel;
            string ChannelMention = "";

            if (e.Guild.VoiceStates.Any(vc => vc.Value.User == Student))
            {
                isInVoiceChannel = true;
                ChannelMention = VoiceChannel.Parent.Mention + " - " + VoiceChannel.Mention;
            }

            bool hasLanguages = false;
            string AllCodeLanguages = "";
            if (codeLanguages.Count > 0)
            {
                hasLanguages = true;
                for (int i=0; i<codeLanguages.Count; i++)
                {
                    if (i > 0)
                    {
                        AllCodeLanguages += $", {codeLanguages[i]}";
                    }
                    else
                        AllCodeLanguages += $"{codeLanguages[i]}";
                }
            }

            var embedbuild = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Orange,
                Title = "[ Spørsmål Kø ]",
                Description = $"**Bruker:** {Student.Mention} {(isInVoiceChannel ? " er i Kanalen: " + ChannelMention : "")} \n\n" +
                              $"{Content} \n" +
                              $"\n **Lærer:** {(AssignedTeacher != null ? AssignedTeacher.Mention : "")} \n" +
                              $"{(hasLanguages ? "**Språk:** " + AllCodeLanguages : "")}" +
                              $"\n **Status:** {(IsCompleted ? "Ferdig" : "Ikke Ferdig")} \n",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Height = 100,
                    Width = 100,
                    Url = AssignedTeacher is object ? AssignedTeacher.AvatarUrl : Student.AvatarUrl
            },
            };
            if (IsCompleted)
                embedbuild.Color = DiscordColor.SpringGreen;

            var newEmbed = embedbuild.Build();

            await DiscordMsg.ModifyAsync(embed: newEmbed);
        }
        public async Task Update(MessageCreateEventArgs e)
        {
            bool isInVoiceChannel = false;
            DiscordChannel VoiceChannel = e.Guild.VoiceStates[Student.Id].Channel;
            string ChannelMention = "";

            if (e.Guild.VoiceStates.Any(vc => vc.Value.User == Student))
            {
                isInVoiceChannel = true;
                ChannelMention = VoiceChannel.Parent.Mention + " - " + VoiceChannel.Mention;
            }

            bool hasLanguages = false;
            string AllCodeLanguages = "";
            if (codeLanguages.Count > 0)
            {
                hasLanguages = true;
                for (int i = 0; i < codeLanguages.Count; i++)
                {
                    if (i > 0)
                    {
                        AllCodeLanguages += $", {codeLanguages[i]}";
                    }
                    else
                        AllCodeLanguages += $"{codeLanguages[i]}";
                }
            }

            var embedbuild = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Orange,
                Title = "[ Spørsmål Kø ]",
                Description = $"**Bruker:** {Student.Mention} {(isInVoiceChannel ? " er i Kanalen: " + ChannelMention : "")} \n\n" +
                              $"{Content} \n" +
                              $"\n **Lærer:** {(AssignedTeacher != null ? AssignedTeacher.Mention : "")} \n" +
                              $"{(hasLanguages ? "**Språk:** " + AllCodeLanguages : "")}" +
                              $"\n **Status:** {(IsCompleted ? "Ferdig" : "Ikke Ferdig")} \n",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Height = 100,
                    Width = 100,
                    Url = AssignedTeacher is object ? AssignedTeacher.AvatarUrl : Student.AvatarUrl
                },
            };
            if (IsCompleted)
                embedbuild.Color = DiscordColor.SpringGreen;

            var newEmbed = embedbuild.Build();

            await DiscordMsg.ModifyAsync(embed: newEmbed);
        }
        public void Remove()
        {
            Program.QuestionQueueTask.Remove(this);
        }
    }
}
