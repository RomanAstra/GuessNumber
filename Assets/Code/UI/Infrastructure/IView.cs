using Helpers;

namespace UI.Infrastructure
{
    public interface IView : IPoolObject
    {
    }
    
    public interface IView<T> : IView where T : IViewModel
    {
        T ViewModel { get; }
        void Initialize(T viewModel);
    }
}
