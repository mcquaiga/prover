using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests.TestActions;

namespace Prover.Core.VerificationTests.PreTestActions
{
    public class DateTimeValidator : IPreTestValidation
    {
        private Logger _log = LogManager.GetCurrentClassLogger();

        public async Task Validate(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            var dtNow = DateTime.Now;

            if (!instrument.GetDateTime().Equals(dtNow))
            {
                _log.Info("Resetting instrument date/time...");
                await commClient.SetItemValue(203, instrument.GetTimeFormatted(dtNow));
                await commClient.SetItemValue(204, instrument.GetDateFormatted(dtNow));
            }
        }
    }
}
