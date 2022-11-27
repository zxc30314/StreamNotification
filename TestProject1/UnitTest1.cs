namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [TestCase(" https://www.youtube.com/@LofiGirl ", "UCSJ4gkVC6NrvII8umztf0Ow", "@LofiGirl")]
    [TestCase("https://www.youtube.com/@LofiGirl", "UCSJ4gkVC6NrvII8umztf0Ow", "@LofiGirl")]
    [TestCase("https://www.youtube.com/watch?v=jfKfPfyJRdk&ab_channel=LofiGirl", "UCSJ4gkVC6NrvII8umztf0Ow", "@LofiGirl")]
    public async Task GetYoutubeChannelId(string uri, string resultId, string resultName)
    {
        var youtubeChannelId = await new YoutubeTool(uri).GetYoutubeChannelId();
        Assert.AreEqual(youtubeChannelId.id, resultId);
    }

    [TestCase("https://www.youtube.com/watch?v=jfKfPfyJRdk&ab_channel=LofiGirl", "lofi hip hop radio - beats to relax/study to")]
    [TestCase("https://www.youtube.com/@LofiGirl", "lofi hip hop radio - beats to relax/study to")]
    [TestCase(" https://www.youtube.com/watch?v=jfKfPfyJRdk&ab_channel=LofiGirl ", "lofi hip hop radio - beats to relax/study to")]
    public async Task GetYoutubeWatchInfo(string url, string result)
    {
        var youtubeWatchInfo = await new YoutubeTool(url).LiveInfo;
        Assert.AreEqual(youtubeWatchInfo?.Title??"", result);
    }

    [TestCase(@"abcddfg", "dd", "ddfg")]
    public void Test1(string input, string fitter, string result)
    {
        var temp = input.StringSkipWhile(fitter);

        Assert.AreEqual(temp, result);
    }

    [TestCase(@"abcddfg", "ddd", false)]
    [TestCase(@"abcddfg", "dd", true)]
    public void Test1(string input, string fitter, bool result)
    {
        var temp = input.StringHas(fitter);

        Assert.AreEqual(temp, result);
    }

    [TestCase("https://www.youtube.com/watch?v=Py6EZ6E7zQI&ab_channel=AtLojart", "")]
    [TestCase("https://www.youtube.com/watch?v=jfKfPfyJRdk&ab_channel=LofiGirl", "jfKfPfyJRdk")] //living
    [TestCase("https://www.youtube.com/@shasha77", "")]
    [TestCase("https://www.youtube.com/@LofiGirl", "jfKfPfyJRdk")] //living
    public async Task Test2(string uri, string id)
    {
        var s = await new YoutubeTool(uri).GetLivingVideoId();
        Assert.AreEqual(s, id);
    }

    // [Test]
    public void GetHoloNowStreamingChannel()
    {
        var youtubeWatchInfo = new HoloTool().GetHoloNowStreamingChannel().Result;
    }
}