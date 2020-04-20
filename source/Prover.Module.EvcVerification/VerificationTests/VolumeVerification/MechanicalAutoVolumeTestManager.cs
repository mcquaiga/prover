namespace Module.EvcVerification.VerificationTests.VolumeVerification
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class MechanicalAutoVolumeTestManager : AutoVolumeTestManager
    {
        private const long TachometerCount = 100;
        public MechanicalAutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm, ISettingsService settingsService) : base(eventAggregator, tachComm, settingsService)
        {
        }      

        protected override async Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct)
        {
            await Task.Run(() =>
            {
                var tachCount = 0;

                using (Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .Select(_ => Observable.FromAsync(async () => tachCount = await TachometerCommunicator.ReadTach()))
                        .Concat()
                        .Subscribe())
                {
                    while (tachCount < TachometerCount && !ct.IsCancellationRequested) { }
                }
            });                 
        }
    }
}
