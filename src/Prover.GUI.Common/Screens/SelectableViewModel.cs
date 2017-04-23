using ReactiveUI;

namespace Prover.GUI.Common.Screens
{
    public class SelectableViewModel<T> : ViewModelBase
        where T : class
    {
        public SelectableViewModel(T selectableObject)
        {
            Object = selectableObject;
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        private T _object;
        public T Object
        {
            get { return _object; }
            set { this.RaiseAndSetIfChanged(ref _object, value); }
        }
    }
}
