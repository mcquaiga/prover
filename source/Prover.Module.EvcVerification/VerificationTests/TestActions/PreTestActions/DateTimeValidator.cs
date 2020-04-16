using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Module.EvcVerification.VerificationTests.TestActions.PreTestActions
{
    public class DateTimeValidator : IEvcDeviceValidationAction
    {        
        public VerificationStep VerificationStep => VerificationStep.PreVerification;            

        public async Task Execute(EvcCommunicationClient commClient, Instrument instrument, CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null)
        {
            var dtNow = DateTime.Now;

            if (!instrument.GetDateTime().Equals(dtNow))
            {             
                statusUpdates?.OnNext("Resetting instrument date/time...");
                await commClient.SetItemValue(203, instrument.GetTimeFormatted(dtNow));
                await commClient.SetItemValue(204, instrument.GetDateFormatted(dtNow));
            }
        }
    }
}
