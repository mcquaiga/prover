using System;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Models.EvcVerifications;

namespace Prover.UI.Desktop.Startup
{
	public static class DashboardServiceEx
	{
		public static DateTime ThisWeek = DateTime.Now.Subtract(TimeSpan.FromDays(7));

		public static Func<EvcVerificationTest, bool> Today => (v => v.TestDateTime.IsToday());
		public static Func<EvcVerificationTest, bool> Verified => (v => v.Verified);



		public static void AddValueDashboardItem(this IServiceCollection services, string title, Func<EvcVerificationTest, bool> predicate)
		{
			//services.AddSingleton<IDashboardValueViewModel>(c => 
			//        new ValueDashboardViewModel(c.GetRequiredService<IEntityDataCache<EvcVerificationTest>>(), title, predicate));
		}
	}
}