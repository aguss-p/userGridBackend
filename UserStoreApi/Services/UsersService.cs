using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserStoreApi.Models;


namespace UserStoreApi.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<UserModel> _usersCollection;

        public UsersService(
            IOptions<UserStoreDatabaseSettings> UserStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                UserStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                UserStoreDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<UserModel>(
                UserStoreDatabaseSettings.Value.UsersCollectionName);
        }

        public async Task<List<UserModel>> GetAsyncWithSearchText(string? searchText = "")
        {
            var results = new List<UserModel>();
            if (string.IsNullOrEmpty(searchText))
            {
                results = await _usersCollection.Find(_ => true).ToListAsync();
            }
            else
            {
                var filter = Builders<UserModel>.Filter.Regex(x=>x.username, new BsonRegularExpression(searchText, "i"));
                results = await _usersCollection.Find(filter).ToListAsync();
            }

            return results;

        }

        public async Task<UserModel?> GetAsync(string id) =>
            await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(UserModel newUser) =>
            await _usersCollection.InsertOneAsync(newUser);

        public async Task UpdateAsync(string id, UserModel updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task RemoveAsync(string id) =>
            await _usersCollection.DeleteOneAsync(x => x.Id == id);
    }
}
