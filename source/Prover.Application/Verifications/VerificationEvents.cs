using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications.Events;
using Prover.Application.ViewModels;
using System.Reactive;
using Prover.Shared.Events;

namespace Prover.Application.Verifications
{
	public class VerificationEvents
	{
		public static EventHub<EvcVerificationViewModel, EvcVerificationViewModel>
				OnInitialize
		{ get; } = new EventHub<EvcVerificationViewModel, EvcVerificationViewModel>();

		public static EventHub<EvcVerificationTest, EvcVerificationTest>
				OnSubmit
		{ get; } = new EventHub<EvcVerificationTest, EvcVerificationTest>();

		public static EventHub<EvcVerificationViewModel, EvcVerificationViewModel>
				OnVerified
		{ get; } = new EventHub<EvcVerificationViewModel, EvcVerificationViewModel>();

		public static EventHub<EvcVerificationTest, EvcVerificationTest>
				OnSave
		{ get; } = new EventHub<EvcVerificationTest, EvcVerificationTest>();

		public static class TestEvents<TManager>
		{
			public static EventHub<TManager, Unit> OnStart { get; } = new EventHub<TManager, Unit>();
			public static EventHub<TManager, Unit> OnComplete { get; } = new EventHub<TManager, Unit>();
		}

		public static class CorrectionTests
		{
			public static EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>
					OnStart
			{ get; } = new EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>();

			public static EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>
					OnComplete
			{ get; } = new EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>();

			public static EventHub<StabilizeLiveReadItemsCoordinator, StabilizeLiveReadItemsCoordinator>
					OnLiveReadStart
			{ get; } = new EventHub<StabilizeLiveReadItemsCoordinator, StabilizeLiveReadItemsCoordinator>();

			public static EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>
					OnLiveReadComplete
			{ get; } = new EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>();

			public static EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>
					BeforeDownload
			{ get; } = new EventHub<VerificationTestPointViewModel, VerificationTestPointViewModel>();
		}

		public static void DefaultSubscribers(ILogger<VerificationEvents> logger = null)
		{
			logger = logger ?? NullLogger<VerificationEvents>.Instance;

			OnInitialize.Subscribe(e => SetResponse(e, e.Input, nameof(OnInitialize)));
			OnSubmit.Subscribe(e => SetResponse(e, e.Input, nameof(OnSubmit)));

			CorrectionTests.OnStart.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTests.OnStart)));
			CorrectionTests.OnComplete.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTests.OnComplete)));

			void SetResponse<TIn, TOut>(EventContext<TIn, TOut> context, TOut output, string name)
			{
				logger.LogDebug($"{name} Default event {name} was raised.");
				context.SetOutput(output);
			}
		}

	}
}