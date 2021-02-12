using System.Threading.Tasks;

namespace Prover.Application.Interfaces {
	public interface IVolumeTestManager {
		Task CompleteVolumeVerification();
		Task BeginVolumeVerification();
		Task StartTest();
	}
}