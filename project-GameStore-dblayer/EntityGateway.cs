using Microsoft.Identity.Client;
using project_GameStore_models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_GameStore_dblayer
{
    public partial class EntityGateway
    {
        public ProjectManagerContext Context { get; set; }

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
        public void Delete(params IEntity[] entities)
        {
            Context.RemoveRange(entities);
            Context.SaveChanges();
        }
    }
}
