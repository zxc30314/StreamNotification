using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

internal class DiscordBot
{
    public static DiscordClient _discordClient;
    private static string _token;

    public async Task Main()
    {
        Log.Info("DiscordBot Start");
        try
        {        
            _token = Environment.GetEnvironmentVariable("TOKEN");
            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = _token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            _discordClient.Ready += Ready;
            var cnext = _discordClient.UseSlashCommands(new());
            cnext.RegisterCommands<Commands>();
            cnext.SlashCommandExecuted += CommandExecuted;
            cnext.SlashCommandErrored += CommandErrored;
            await _discordClient.ConnectAsync();

            await Task.Delay(-1);

            Task CommandExecuted(SlashCommandsExtension slashCommandsExtension, SlashCommandExecutedEventArgs slashCommandExecutedEventArgs)
            {
                var userUsername = slashCommandExecutedEventArgs.Context.User.Username;
                var contextCommandName = slashCommandExecutedEventArgs.Context.CommandName;
                Log.Info($"{userUsername} Call {contextCommandName}");
                return Task.CompletedTask;
            }

            Task CommandErrored(SlashCommandsExtension slashCommandsExtension, SlashCommandErrorEventArgs slashCommandErrorEventArgs)
            {
                Log.Error(slashCommandErrorEventArgs.Exception.ToString());
                return Task.CompletedTask;
            }

        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }

    }

    private static void CheckToken(string[] args)
    {
        _token = string.Empty;
        foreach (var value in args)
        {
            if (value.Contains("token"))
            {
                _token = value.Split('=')[1];
            }
        }
    }

    private Task Ready(DiscordClient sender, ReadyEventArgs e)
    {
        var schedule = new Schedule();
        return Task.CompletedTask;
    }
}