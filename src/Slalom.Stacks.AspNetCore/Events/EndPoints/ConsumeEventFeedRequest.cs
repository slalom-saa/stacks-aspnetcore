namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    public class ConsumeEventFeedRequest
    {
        public string Url { get; }

        public ConsumeEventFeedRequest(string url)
        {
            this.Url = url;
        }
    }
}