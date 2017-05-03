namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    public class SubscribeRequest
    {
        public string Path { get; }

        public SubscribeRequest(string path)
        {
            this.Path = path;
        }
    }
}