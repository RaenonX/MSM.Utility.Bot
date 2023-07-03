﻿using Discord.WebSocket;
using MSM.Bot.Extensions;
using MSM.Common.Controllers;

namespace MSM.Bot.Workers;

public class PxUpdateCheckWorker : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxUpdateCheckWorker> _logger;

    private bool _failed;

    public PxUpdateCheckWorker(DiscordSocketClient client, ILogger<PxUpdateCheckWorker> logger) {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetSystemAlertChannelAsync();

        while (!cancellationToken.IsCancellationRequested) {
            var lastValidTickUpdate = await PxMetaController.GetLastValidTickUpdate();

            if (lastValidTickUpdate is null) {
                // No valid tick
                _logger.LogWarning("Last valid tick check failed (No valid tick)");
                await channel.SendMessageAsync("No last valid tick update found!");
                _failed = true;
            } else if (DateTime.UtcNow - lastValidTickUpdate > TimeSpan.FromSeconds(45)) {
                // No valid tick within certain time
                var secsAgo = (DateTime.UtcNow - lastValidTickUpdate.Value).TotalSeconds;

                _logger.LogWarning(
                    "Last valid tick check failed (Last tick at {LastValidTickUpdate})",
                    lastValidTickUpdate
                );
                await channel.SendMessageAsync(
                    $"No price update since **{secsAgo:0} secs ago**!\n" +
                    $"> Last valid tick updated at {lastValidTickUpdate} (UTC)"
                );
                _failed = true;
            } else {
                // Found valid tick
                if (_failed) {
                    await channel.SendMessageAsync(
                        $"Prices start ticking again!\n" +
                        $"> Received price update at **{lastValidTickUpdate}** (UTC)"
                    );
                }

                _logger.LogInformation(
                    "Last valid tick check succeed (Last tick at {LastValidTickUpdate})",
                    lastValidTickUpdate
                );
                _failed = false;
            }

            await Task.Delay(30000, cancellationToken);
        }
    }
}