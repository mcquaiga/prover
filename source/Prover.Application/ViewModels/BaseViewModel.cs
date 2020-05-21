using System;
using System.Windows.Input;

namespace Prover.Application.ViewModels
{
	public abstract class ViewModelWithIdBase : ViewModelBase
	{
		protected ViewModelWithIdBase() : this(Guid.NewGuid())
		{
		}

		protected ViewModelWithIdBase(Guid id) => Id = id;

		public Guid Id { get; set; }
	}
}