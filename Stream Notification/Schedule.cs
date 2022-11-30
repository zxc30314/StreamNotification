public class Schedule
{
    private Timer holoSchedule;

    public Schedule()
    {
        holoSchedule = new Timer(async (objState) => await Execute(), null, TimeSpan.FromSeconds(20), TimeSpan.FromMinutes(5));
    }

    private async Task Execute()
    {
        try
        {
            List<YoutubeTool.LiveData> list = new List<YoutubeTool.LiveData>();
            foreach (var id in new DataBase().GetAllData().Result.SelectMany(x => x?.YoutubeChannelID).Distinct())
            {
                if (id.Key.ChannelIdToUrl(out var uri))
                {
                    var liveData = await new YoutubeTool(uri).LiveInfo;
                    if (liveData.isLiving)
                    {
                        list.Add(liveData);
                    }
                }
            }

            await SendMessage(list);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }

        await Task.CompletedTask;
    }


    private async Task SendMessage(List<YoutubeTool.LiveData> list)
    {
        var allData = new DataBase().GetAllData();
        foreach (var dataBase in allData.Result)
        {
            var channelAsync = DiscordBot._discordClient.GetChannelAsync(dataBase.DiscordChannelID).Result;
            var messages = channelAsync.GetMessagesAsync(50).Result.Select(x => x.Content);
            foreach (var s in dataBase.YoutubeChannelID)
            {
                var liveData = list.Find(x => x.ChannelId == s.Key);
                if (liveData != null)
                {

                    var content = $"{liveData.ChannelName} Start Streaming \n{liveData.Title} \n{liveData.Description} \n{liveData.Url}";
                    var contains = messages.Contains(content);
                    if (!contains)
                    {
                        await DiscordBot._discordClient.SendMessageAsync(channelAsync, content);
                    }

                    await Task.Delay(100);
                }
            }
        }
    }
}