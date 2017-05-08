namespace Slalom.Stacks.AspNetCore.Swagger.Model
{
    public class ApiKeyScheme : SecurityScheme
    {
        public string Name { get; set; }

        public string In { get; set; }

        public ApiKeyScheme()
        {
            this.Type = "apiKey";
        }
    }
}
