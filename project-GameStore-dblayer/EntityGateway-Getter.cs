using project_GameStore_models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_GameStore_dblayer
{
    public partial class EntityGateway
    {
        public IEnumerable<Category> GetCategories(Func<Category, bool> predicate) =>
          Context.Categories.Where(predicate).ToArray();
        public IEnumerable<Category> GetCategories() =>
            GetCategories((x) => true);


        public IEnumerable<Client> GetClients(Func<Client,bool> predicate) =>
            Context.Clients.Where(predicate).ToArray();
        public IEnumerable<Client> GetClients() =>
            GetClients((x) => true);


        public IEnumerable<Entity_Game> GetEntity_Games(Func<Entity_Game,bool> predicate) =>
            Context.Entities.Where(predicate).ToArray();
        public IEnumerable<Entity_Game> GetEntity_Games() =>
            GetEntity_Games((x) => true);


        public IEnumerable<Game> GetGames(Func<Game, bool> predicate) =>
           Context.Games.Where(predicate).ToArray();
        public IEnumerable<Game> GetGames() =>
            GetGames((x) => true);


        public IEnumerable<Order> GetOrders(Func<Order, bool> predicate) =>
           Context.Orders.Where(predicate).ToArray();
        public IEnumerable<Order> GetOrders() =>
            GetOrders((x) => true);


        public IEnumerable<Platform> GetPlatforms(Func<Platform, bool> predicate) =>
           Context.Platforms.Where(predicate).ToArray();
        public IEnumerable<Platform> GetPlatforms() =>
            GetPlatforms((x) => true);



    }
}
