using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Application.ViewModels;
using Application.ViewModels.Services;
using Devices.Core.Items.ItemGroups;
using Devices.Honeywell.Core.Items;
using Devices.Honeywell.Core.Items.ItemGroups;
using Domain.EvcVerifications;
using Domain.EvcVerifications.Verifications;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Infrastructure.EntityFrameworkSqlDataAccess.Entities;
using Infrastructure.EntityFrameworkSqlDataAccess.Storage;
using Infrastructure.SampleData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.EntityFrameworkSqlDataAccess
{
    public class DbInitializer
    {
        private readonly EvcVerificationTestService _service;
        private readonly VerificationViewModelService _viewModelService;

        public DbInitializer(EvcVerificationTestService service, VerificationViewModelService viewModelService)
        {
            _service = service;
            _viewModelService = viewModelService;
        }

        public async Task Initialize(ProverDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            await Seed(context);
        }

        private async Task Seed(ProverDbContext context)
        {
            await context.EvcVerifications.LoadAsync();
            var evc = await context.EvcVerifications.ToListAsync();

            
            var repo = await Devices.Repository.GetAsync();

            var deviceType = repo.GetByName("Mini-Max");
            var device = deviceType.Factory.CreateInstance(ItemFiles.MiniMaxItemFile);

            var lowTemp = deviceType.ToItemValuesEnumerable(ItemFiles.TempLowItems);
            var ti = device.ItemGroup<TemperatureItems>(lowTemp);

            //CorrectionTest.Update<TemperatureItems>(tempTest, ti, ti.Factor)
            var tempVm = new TemperatureFactorViewModel(ti, 32);

            var testVm = await _viewModelService.CreateNewTest(device);
            var t = _viewModelService.ConvertViewModelToModel(testVm);

            var testRun = new EvcVerificationSql(device);

            testRun.AddTests(t.Tests);

            await context.EvcVerifications.AddAsync(testRun);
          
            await context.SaveChangesAsync();

            await context.EvcVerifications
                .Include(v => v.Tests)
                .ThenInclude(vt => (vt as VerificationTestPoint).Tests)
                .LoadAsync();

            evc = await context.EvcVerifications.ToListAsync();

            var myVm = await _viewModelService.CreateViewModelFromVerification(evc.First());


        }
    }
}
