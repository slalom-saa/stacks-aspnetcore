namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    public class PublishEventsRequest
    {
        public string Content { get; }

        public PublishEventsRequest(string content)
        {
            this.Content = content;
        }
    }
}