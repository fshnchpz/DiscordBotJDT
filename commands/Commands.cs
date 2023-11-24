using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotJDT.commands
{
    public class Commands : BaseCommandModule
    {
        [Command("test")]
        public async Task Testcommand(CommandContext C)
        {
            await C.Channel.SendMessageAsync($"Hello {C.Member.Nickname} !");
        }

        [Command("stian")]
        public async Task Stian(CommandContext C)
        {
            var message = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Orange,
                Title = "Lærern med beste humor er:",
                Description = $"Stian Sundby !",
                ImageUrl = "https://getacademy.no/hubfs/GET_RGB_logo-01-2.png",
            };

            await C.Channel.SendMessageAsync(embed: message);
        }

        [Command("interact")]
        public async Task Interact(CommandContext C)
        {
            var userID = C.User;
            var interact = Program.Client.GetInteractivity();
            DiscordEmoji[] reactEmojis =
            {
                DiscordEmoji.FromName(Program.Client, ":white_check_mark:"),
            };

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Aquamarine,
                Title = "Test",
                Description = "Do you have Carl as hostage?",
            };

            var messageSent = await C.Channel.SendMessageAsync(embed: message);
            
            //Add Reaction emojis
            foreach (var reaction in reactEmojis)
            {
                await messageSent.CreateReactionAsync(reaction);
            }

            //Wait for input & check if user_reaction
            var MsgRetrieve = await interact.WaitForReactionAsync(msg => msg.Message.Id == messageSent.Id && msg.User == userID);
            if (MsgRetrieve.Result.Emoji.Name == reactEmojis[0].Name)
            {
                await C.Channel.SendMessageAsync($"That's very good!");
            }
            
        }

    }
}
