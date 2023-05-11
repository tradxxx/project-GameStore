using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using project_GameStore_models;
using project_GameStore_models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public int AddEntityToGame(Game game, params Entity_Game[] entity_Games)
        {
            if (Context.Entry(game).State == EntityState.Detached)
                game = Context.Games.FirstOrDefault(x => x.Id == game.Id) ??
                    throw new Exception("Game dooes not exist.");
            List<Entity_Game> entity_GamesList = new(entity_Games);
            for (int i = 0; i < entity_Games.Length; i++)
            {
                Entity_Game entity = entity_GamesList[i];
                if (Context.Entry(entity).State == EntityState.Detached ||
                    (entity = Context.Entities.FirstOrDefault(x => x.Id == entity!.Id)!) is null)
                    entity_GamesList.RemoveAt(i--);
            }
            var toChange = Context.Entities
                .Where(x => entity_GamesList.Contains(x))
                .Except(game.Keys_Game)
                .ToArray();



            AddOrUpdate(game);
            Context.SaveChanges();
            return toChange.Length;
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
    }
}
