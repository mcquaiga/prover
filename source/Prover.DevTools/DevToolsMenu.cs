using System.Windows.Input;

namespace Prover.DevTools
{
	public interface IDevToolsMenuItem
	{
		public string Description { get; set; }

		public ICommand Command { get; set; }
	}
}