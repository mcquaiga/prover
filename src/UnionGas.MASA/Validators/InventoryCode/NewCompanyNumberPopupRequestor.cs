using Caliburn.Micro;
using NLog;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.GUI.Common;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;
using LogManager = NLog.LogManager;

namespace UnionGas.MASA.Validators.InventoryCode
{
    public class NewCompanyNumberPopupRequestor : IGetValue
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ScreenManager _screenManager;

        public NewCompanyNumberPopupRequestor(ScreenManager screenManager, IEventAggregator eventAggregator)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
        }

        public string GetValue()
        {
            var dialog = _screenManager.ResolveViewModel<CompanyNumberDialogViewModel>();
            var result = _screenManager.ShowDialog(dialog);

            if (result.HasValue && result.Value)
            {
                _log.Debug($"New inventory code {dialog.CompanyNumber} was entered.");
                return dialog.CompanyNumber;
            }

            _log.Debug($"Skipping inventory code verification.");
            return null;
        }
    }
}