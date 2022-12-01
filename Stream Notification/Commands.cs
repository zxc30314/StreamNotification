using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Tool.Observable;

public class Commands : ApplicationCommandModule
{
    [SlashCommand("setChannel", "Set Channel")]
    public async Task SetChannel(InteractionContext ctx, [Option("DiscordChannel", "Set Channel")] DiscordChannel channel)
    {

        var success = channel.Type == ChannelType.Text;
        if (!success)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Option Requirement <@{ChannelType.Text}> Type"));
        }
        else
        {
            var dataBase = new DataBase()
            {
                GuildId = ctx.Guild.Id,
                DiscordChannelID = channel.Id,
            };
            await dataBase.Save();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Set {channel.Name} Success"));
        }
    }

    [SlashCommand("add", "Add Youtube Channel to Notification")]
    public async Task AddYoutubeChannel(InteractionContext ctx, [Option("string", "Youtube Channel Url")] string uri)
    {
        var dataBase = new DataBase();
        var load = await dataBase.Load(ctx.Guild.Id);
        if (load)
        {
            var youtubeChannelId = await new YoutubeTool(uri).GetYoutubeChannelId();
            dataBase.YoutubeChannelID.Add(youtubeChannelId.id, youtubeChannelId.name);
            await dataBase.Save();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Add {youtubeChannelId.name} Success"));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please Set Channel"));
        }
    }

    [SlashCommand("list", "Show Youtube Channel to Notification List")]
    public async Task ListYoutubeChannel(InteractionContext ctx)
    {
        var dataBase = new DataBase();
        var load = await dataBase.Load(ctx.Guild.Id);
        if (load)
        {
            var stringBuilder = new StringBuilder();
            foreach (var keyValuePair in dataBase.YoutubeChannelID)
            {
                stringBuilder.Append($"{keyValuePair.Value}\n");
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{stringBuilder}"));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please Set Channel"));
        }
    }

    [SlashCommand("remove", "Remove Youtube Channel to Notification List")]
    public async Task RemoveYoutubeChannel(InteractionContext ctx, [Option("string", "Youtube Channel Url")] string uri)
    {
        var dataBase = new DataBase();
        var load = await dataBase.Load(ctx.Guild.Id);
        if (load)
        {
            var youtubeChannelId = await new YoutubeTool(uri).GetYoutubeChannelId();

            if (dataBase.YoutubeChannelID.Remove(youtubeChannelId.id))
            {
                await dataBase.Save();
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Remove {youtubeChannelId.name} Success"));
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Remove {youtubeChannelId.name} Fail"));
            }
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Please Set Channel"));
        }
    }
}

public class TestChoiceProvider : IChoiceProvider
{
    private ulong _guid;

    public TestChoiceProvider(ulong guid)
    {
        _guid = guid;

    }

    public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        return new DiscordApplicationCommandOptionChoice[]
        {
            //You would normally use a database call here
            new DiscordApplicationCommandOptionChoice("testing", "testing"),
            new DiscordApplicationCommandOptionChoice("testing2", "test option 2")
        };
    }
}

class DataBase
{
    public ulong GuildId { get; set; }
    public ulong DiscordChannelID { get; set; }
    public Dictionary<string, string> YoutubeChannelID { get; set; } = new();

    public Task Save()
    {
        var serializeObject = JsonConvert.SerializeObject(this, Formatting.Indented);
        var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save");
        var path = Path.Combine(folder, $"{GuildId}.json");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(path, serializeObject);
        return Task.CompletedTask;
    }

    public async Task<bool> Load(ulong id)
    {
        var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save");
        var path = Path.Combine(folder, $"{id}.json");
        if (File.Exists(path))
        {
            var readAllTextAsync = await File.ReadAllTextAsync(path);
            var deserializeObject = JsonConvert.DeserializeObject<DataBase>(readAllTextAsync) ?? new DataBase() {DiscordChannelID = id};
            GuildId = deserializeObject.GuildId;
            DiscordChannelID = deserializeObject.DiscordChannelID;
            YoutubeChannelID = deserializeObject.YoutubeChannelID;
            return true;
        }

        return false;
    }

    public Task<DataBase?[]> GetAllData()
    {
        var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "save");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var strings = Directory.GetFiles(folder, "*.json");

        return Task.FromResult(strings.Select(x => JsonConvert.DeserializeObject<DataBase>(File.ReadAllText(x))).ToArray()!);
    }
}