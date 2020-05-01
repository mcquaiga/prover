using System.Windows.Input;

namespace Prover.Modules.DevTools
{
	public interface IDevToolsMenuItem
	{
		public string Description { get; set; }

		public ICommand Command { get; set; }
	}
}