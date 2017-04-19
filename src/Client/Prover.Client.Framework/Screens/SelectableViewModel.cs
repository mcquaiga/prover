using ReactiveUI;

namespace Prover.Client.Framework.Screens
{
    public class SelectableViewModel<T> : ViewModelBase
        where T : class
    {
        private bool _isSelected;

        private T _object;

        public SelectableViewModel(T selectableObject)
        {
            Object = selectableObject;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public T Object
        {
            get { return _object; }
            set { this.RaiseAndSetIfChanged(ref _object, value); }
        }
    }
}