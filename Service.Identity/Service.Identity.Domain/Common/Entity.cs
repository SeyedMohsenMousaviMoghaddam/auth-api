using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Common
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; protected set; }
    }

    public abstract class Entity : Entity<long>
    {
    }
}
