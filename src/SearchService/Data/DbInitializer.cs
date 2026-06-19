using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings.
                FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .CreateAsync();

            long count = await DB.CountAsync<Item>();

            if(count == 0)
            {
                Console.WriteLine("No data - will attempt to seed initial data.");

                string itemData = await File.ReadAllTextAsync("Data/auctions.json");

                JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

                List<Item>? items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                if (items != null)
                {
                    await DB.SaveAsync(items);
                }
            }
        }
    }
}
