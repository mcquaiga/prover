using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter.MeterDTODialog;
using Prover.Modules.UnionGas.MasaWebService;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Prover.Modules.UnionGas.Exporter.Views.TestsByJobNumber
{
    /// <summary>
    ///     Defines the <see cref="TestsByJobNumberViewModel" />
    /// </summary>
    public class TestsByJobNumberViewModel : ViewModelBase
    {
        private IList<MeterDTO> _meterDtoList = new List<MeterDTO>();


        public TestsByJobNumberViewModel(IMeterService<MeterDTO> meterService)
        {

            GetOpenJobNumbersCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                return Observable.Return(new List<string>());
                //return _testRunService.GetAllUnexported()
                //    .Where(i => !string.IsNullOrEmpty(i.JobId))
                //    .Select(i => i.JobId)
                //    .Distinct()
                //    .ToList();
            });

            GetOpenJobNumbersCommand
                .ToPropertyEx(this, x => x.JobNumbers, new List<string>());

            var canExecuteFetchTestsByJobNumber =
                this.WhenAnyValue(x => x.SelectedJobNumber, jobNumber => !string.IsNullOrEmpty(jobNumber));
            FetchTestsByJobNumberCommand = ReactiveCommand.CreateFromTask<string>(async (jobNumber) =>
            {
                if (!string.IsNullOrEmpty(SelectedJobNumber))
                {
                    var meterList = await meterService.GetOutstandingMeterTestsByJobNumber(int.Parse(jobNumber));

                    if (meterList.Any())
                    {
                        var mdViewModel = new MeterDtoListDialogViewModel(meterList);
                        await Messages.ShowDialog.Handle(mdViewModel);
                    }
                }

            }, canExecuteFetchTestsByJobNumber);

            FetchTestsByJobNumberCommand.ThrownExceptions
                .Subscribe(async ex => await Messages.ShowMessage.Handle(ex.Message));
        }


        public ReactiveCommand<string, Unit> FetchTestsByJobNumberCommand { get; }


        public ReactiveCommand<Unit, List<string>> GetOpenJobNumbersCommand { get; }


        public extern List<string> JobNumbers { [ObservableAsProperty] get; }


        [Reactive] public string SelectedJobNumber { get; set; }
    }
}