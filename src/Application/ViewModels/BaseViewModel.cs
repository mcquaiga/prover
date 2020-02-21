using System;
using ReactiveUI;

namespace Application.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject
    {
        protected BaseViewModel()
        {
            Id = Guid.NewGuid();
        }

        protected BaseViewModel(Guid id)
        {
            Id = id;
        }

        public virtual Guid Id { get; protected set; }
    }
}