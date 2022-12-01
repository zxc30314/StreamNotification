using System.Web;
using Emzi0767.Utilities;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

public class HoloTool
{
    public async Task<List<YoutubeTool.LiveData>> GetHoloNowStreamingChannel()
    {
        List<YoutubeTool.LiveData> result = new();
        try
        {
            List<string> urlList = new List<string>();
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync("https://schedule.hololive.tv/lives/all");
            urlList.AddRange(htmlDocument.DocumentNode.Descendants()
                .Where((x) => x.Name == "a" &&
                              x.Attributes["href"].Value.StartsWith("https://www.youtube.com/watch") &&
                              x.Attributes["style"].Value.Contains("border: 3px"))
                .Select((x) => x.Attributes["href"].Value));

            foreach (var url in urlList)
            {
                var liveInfo = await new YoutubeTool(url).LiveInfo;
                result.Add(liveInfo);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }

        return result;
    }
}

static class YoutubeToolEx
{
    public static bool ChannelIdToUrl(this string id, out string uri)
    {
        uri = "https://www.youtube.com/channel/" + id;
        return true;
    }

    public static async Task<string> WatchUrlToChannelUrl(this string url)
    {
        var (id, name) = await new YoutubeTool(url).GetYoutubeChannelId();

        if (id.ChannelIdToUrl(out var result))
        {
            return result;
        }

        return "";
    }
}

public class YoutubeTool
{
    private readonly string _url;
    private HtmlDocument _channelContent = new();
    private Task<HtmlDocument> ChannelContent => GetChannelContent();

    private string _channelUrl = "";
    public Task<string> ChannelUrl => GetYoutubeChannelUri();
    public Task<LiveData> LiveInfo => GetYoutubeWatchInfo();

    public YoutubeTool(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException();
        }

        _url = url.Trim();
    }

    private async Task<HtmlDocument> GetChannelContent()
    {
        if (!string.IsNullOrEmpty(_channelContent.Text))
        {
            return _channelContent;
        }

        var tempUrl = _url.StartsWith("https://www.youtube.com/watch") ? await GetYoutubeChannelUri() : _url;

        var htmlWeb = new HtmlWeb();
        return _channelContent = await htmlWeb.LoadFromWebAsync(tempUrl);
    }

    private async Task<string> GetYoutubeChannelUri()
    {
        if (!string.IsNullOrEmpty(_channelUrl))
        {
            return _channelUrl;
        }


        var (id, name) = await GetYoutubeChannelId();
        var channelUrl = "https://www.youtube.com/" + name;
        return _channelUrl = channelUrl;
    }

    public async Task<(string id, string name)> GetYoutubeChannelId()
    {
        var htmlWeb = new HtmlWeb();
        var value = await htmlWeb.LoadFromWebAsync((_url));

        var meta = value.DocumentNode.SelectNodes("//meta");
        var id = meta
            .Select(x => x.Attributes)
            .Where(x => x.AttributesWithName("itemprop").Any(x => x.Value == "channelId"))
            .SelectMany(x => x)
            .LastOrDefault().Value;

        var link = value.DocumentNode.SelectNodes("//link");
        var name = link
            .Select(x => x.Attributes)
            .Where(x => x.AttributesWithName("itemprop").Any(x => x.Value == "name"))
            .SelectMany(x => x)
            .LastOrDefault().Value;

        return (id, name);
    }

    private async Task<LiveData> GetYoutubeWatchInfo()
    {
        var liveUrl = "";
        var youtubeWatchTitle = "";
        var livingVideoId = await GetLivingVideoId();
        if (!string.IsNullOrEmpty(livingVideoId))
        {
            liveUrl = "https://www.youtube.com/watch?v=" + livingVideoId;
            youtubeWatchTitle = await GetYoutubeWatchTitle(liveUrl);
        }

        var (id, name) = await GetYoutubeChannelId();

        return new LiveData()
        {
            isLiving = !string.IsNullOrEmpty(liveUrl),
            ChannelId = id,
            ChannelName = name,
            Url = liveUrl,
            Title = youtubeWatchTitle
        };
    }

    private async Task<string> GetYoutubeWatchTitle(string url)
    {
        if (!url.StartsWith("https://www.youtube.com/watch"))
        {
            return "";
        }

        var doc = new HtmlDocument();
        HtmlWeb htmlWeb = new HtmlWeb();
        doc = await htmlWeb.LoadFromWebAsync(url);
        var nodes = doc.DocumentNode.SelectNodes("//meta");

        var htmlAttributes = nodes
            .Select(x => x.Attributes)
            .Where(x => x.AttributesWithName("name").Any(x => x.Value == "title"))
            .SelectMany(x => x)
            .LastOrDefault().Value ?? "";

        return htmlAttributes;
    }

    public async Task<string> GetLivingVideoId()
    {
        var filter = """"videoId":"""";
        var liveFilter = """"style":"LIVE"""";

        var doc = await ChannelContent;
        if (!doc.Text.StringHas(liveFilter))
        {
            return "";
        }
        var skipWhile = doc.Text.StringSkipWhile(filter).Skip(filter.Length + 1).Take(11);
        var s = new string(skipWhile.ToArray());
        return s;
    }

    public class LiveData
    {
        public bool isLiving;
        public string Title;
        public string Description;
        public string Url;
        public string ChannelId;
        public string ChannelName;
    }
}

public static class Exp
{
    public static string StringSkipWhile(this string input, string fitter)
    {
        var temp = "";
        for (var i = 0; i < input.Length; i++)
        {
            var count = Math.Min(fitter.Length, input.Length - i);
            var substring = input.Substring(i, count);
            if (fitter == substring)
            {
                temp = new string(input.Skip(i).ToArray());
                break;
            }
        }

        return temp;
    }

    public static bool StringHas(this string input, string fitter)
    {
        for (var i = 0; i < input.Length; i++)
        {
            var count = Math.Min(fitter.Length, input.Length - i);
            var substring = input.Substring(i, count);
            if (fitter == substring)
            {
                return true;
                break;
            }
        }

        return false;

    }
}