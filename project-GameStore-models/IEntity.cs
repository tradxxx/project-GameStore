using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_GameStore_models
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}
