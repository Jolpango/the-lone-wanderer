using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace LoneWandererGame.MongoDBManagers;
public sealed class Singleton
{
    private static readonly Lazy<Singleton> lazy = new Lazy<Singleton>(() => new Singleton());
    public static Singleton Instance { get { return lazy.Value; } }
    private Singleton() {}
}
public sealed class MongoDBManager
    {
        private MongoClient client;
        private IMongoDatabase db;
        private string version;
        private static readonly Lazy<MongoDBManager> lazy = new Lazy<MongoDBManager>(() => new MongoDBManager());
        public static MongoDBManager Instance { get { return lazy.Value; } }
        private MongoDBManager() {
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            if (connectionString == null)
            {
                Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
                Environment.Exit(0);
            }
            client = new MongoClient(connectionString);
            db = client.GetDatabase("testdb");
            version = "0.1";
        }

        public async Task<BsonDocument> GetOrCreatePlayer(string playerName)
        {
            var playerCollection = db.GetCollection<BsonDocument>("players");
            var playerFilter = Builders<BsonDocument>.Filter.Eq("name", playerName);
            var player = await playerCollection.Find(playerFilter).FirstOrDefaultAsync();

            if (player != null)
                return player;

            var newPlayer = new BsonDocument
            {
                { "name", playerName },
                { "created_at", DateTime.Now },
                { "updated_at", DateTime.Now }
            };
            await playerCollection.InsertOneAsync(newPlayer);
            return newPlayer;
        }

        public async Task UpsertScore(BsonDocument player, int score)
        {
            var highscoreCollection = db.GetCollection<BsonDocument>("highscores");
            var highscoreFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("player_id", player["_id"]),
                Builders<BsonDocument>.Filter.Eq("version", version)
            );

            var highscore = await highscoreCollection.Find(highscoreFilter).FirstOrDefaultAsync();
            if (highscore != null)
            {
                if (score > highscore["score"])
                {
                    var update = Builders<BsonDocument>.Update
                        .Set("score", score)
                        .Set("updated_at", DateTime.Now);
                    await highscoreCollection.UpdateOneAsync(highscoreFilter, update);
                }
            }
            else
            {
                await highscoreCollection.InsertOneAsync(new BsonDocument
                {
                    { "player_id", player["_id"] },
                    { "version", version },
                    { "score", score },
                    { "created_at", DateTime.Now },
                    { "updated_at", DateTime.Now }
                });
            }
        }

        public async Task UpdatePlayerScore(string playerName, int score)
        {
            var player = await GetOrCreatePlayer(playerName);
            await UpsertScore(player, score);
        }

        public async Task<List<string>> GetHighScores()
        {
            var highscoreCollection = db.GetCollection<BsonDocument>("highscores");

            // TODO: change this pipeline to not use pure Bson like a caveman
            var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$match", new BsonDocument{{ "version", version }}), // filter on version
                new BsonDocument("$lookup", new BsonDocument // a strange way to do a left join on the player collection
                {
                    { "from", "players" },
                    { "localField", "player_id" },
                    { "foreignField", "_id" },
                    { "as", "player" }
                }),
                new BsonDocument("$unwind", "$player"), // unwind the array(?) created from the left join
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 }, // exclude
                    { "name", "$player.name" }, // include
                    { "score", "$score" } // include
                })
            };

            var highscores = await highscoreCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return highscores.Select(highscore => $"{highscore["name"]}: {highscore["score"]}").ToList();
        }
    }
