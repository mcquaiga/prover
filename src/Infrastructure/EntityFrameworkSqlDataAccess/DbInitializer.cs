using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.EntityFrameworkSqlDataAccess.Repositories;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EntityFrameworkSqlDataAccess
{
    public static class DbInitializer
    {
        public static async Task Initialize(ProverDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
        }
    }
}
