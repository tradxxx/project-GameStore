using project_GameStore_models;
using project_GameStore_models.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace project_GameStore_dblayer
{
    public partial class EntityGateway : IDisposable
    {
        internal ProjectManagerContext Context { get; set; } = new ProjectManagerContext();

        public Guid AddOrUpdate(IEntity entity)
        {

            if (entity.Id == Guid.Empty)
                Context.Add(entity);
            else
                Context.Update(entity);
            Context.SaveChanges();
            return entity.Id;

        }

        public void AddOrUpdate(params IEntity[] entities)
        {
            var toAdd = entities.Where(x => x.Id == Guid.Empty);
            var toUpdate = entities.Except(toAdd);
            Context.AddRange(toAdd);
            Context.UpdateRange(toUpdate);
            Context.SaveChanges();
        }


        public int EntitiesInGame(ActionType action,Guid gameId, params Guid[] entitiesIds)
        {
            var game = Context.Games.FirstOrDefault(x => x.Id == gameId)
                 ?? throw new Exception("Game is not found.");

            var entities = Context.Entities.Where(x =>
                entitiesIds.Contains(x.Id)).Except(game.Keys_Game).ToArray();

            foreach(Entity_Game entity in entities)         
                if (action == ActionType.Add)         
                    game.Keys_Game.Add(entity);          
                else  
                    game.Keys_Game.Remove(entity);
            return entities.Length;
        }




        public void Delete(params IEntity[] entities)
        {
            Context.RemoveRange(entities);
            Context.SaveChanges();
        }




        #region IDisposable implementation
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion


        [JsonConverter(typeof(StringEnumConverter))]
        public enum ActionType
        {
            Add,
            Remove
        }
    }
}
