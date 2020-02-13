﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Application.Helpers.Query;
using Application.Specifications;
using Domain.EvcVerifications;
using Shared.Interfaces;

namespace Infrastructure.EntityFrameworkSqlDataAccess.Repositories
{
    public sealed class EvcVerificationSpecificationEvaluator : BaseSpecification<EvcVerificationTest>
    {
        public EvcVerificationSpecificationEvaluator(Guid id) : base(v => v.Id == id)
        {
            AddInclude(v => v.Tests);
            AddIncludes(agg => agg.Include(v => v.Tests));
        }
    }
}