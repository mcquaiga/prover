#region

using System;
using System.Collections.Generic;
using Prover.Core.Shared.Domain;

#endregion

namespace Prover.Core.Shared.DTO.TestRuns
{
    public abstract class TestDtoBase : EntityWithId
    {
        protected TestDtoBase() : base(Guid.NewGuid())
        {
        }

        protected TestDtoBase(Guid id)
            : base(id)
        {
        }

        protected TestDtoBase(Guid id, Dictionary<string, string> itemData)
            : base(id)
        {
            ItemData = itemData;
        }

        public Dictionary<string, string> ItemData { get; set; } = new Dictionary<string, string>();
    }
}