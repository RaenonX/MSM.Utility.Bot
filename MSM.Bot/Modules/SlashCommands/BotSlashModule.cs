﻿using Discord.Interactions;
using JetBrains.Annotations;

namespace MSM.Bot.Modules.SlashCommands;

public class BotSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    [UsedImplicitly]
    public async Task PingAsync() =>
        await RespondAsync(
            text: $"Bot Latency: {Context.Client.Latency} ms",
            ephemeral: true
        );
}