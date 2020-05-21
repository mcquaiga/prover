using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Verifications
{
	public class ManualTestManager : TestManagerBase, IRoutableViewModel
	{
		private readonly IScreenManager _screenManager;

		private readonly IVerificationService _verificationService;
		private readonly ILogger<ManualTestManager> _logger;

		public ManualTestManager(
				ILogger<ManualTestManager> logger,
				IScreenManager screenManager,
				IVerificationService verificationService,
				EvcVerificationViewModel verificationViewModel = null) : base(logger, screenManager, verificationService, verificationViewModel)
		{
			_logger = logger ?? NullLogger<ManualTestManager>.Instance;
			_screenManager = screenManager;
			_verificationService = verificationService;
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				TestViewModel?.Dispose();
			}

		}

		/// <inheritdoc />
		public string UrlPathSegment { get; }

		/// <inheritdoc />
		public IScreen HostScreen => _screenManager;
	}
}