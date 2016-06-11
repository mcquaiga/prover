using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class InstrumentInfoViewModel : ReactiveScreen, IHandle<InstrumentUpdateEvent>
    {
        private readonly IUnityContainer _container;

        public InstrumentInfoViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public InstrumentInfoViewModel(IUnityContainer container, Instrument instrument) : this(container)
        {
            Instrument = instrument;
        }

        public Instrument Instrument { get; set; }

        public string CorrectorType
        {
            get
            {
                switch (Instrument.CompositionType)
                {
                    case Core.Models.Instruments.CorrectorType.PTZ:
                        return "PTZ";
                    case Core.Models.Instruments.CorrectorType.P:
                        return "P";
                    default:
                        return "T";
                }
            }
        }

        public string BasePressure
            =>
                $"{Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}"
            ;

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public void Handle(InstrumentUpdateEvent message)
        {
            Instrument = message.InstrumentManager.Instrument;
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}