using System;

namespace Prover.Web.API.Common
{
    public abstract class Entity
    {    
        public Guid Id { get; protected set; }
    }
}
