namespace UI.Infrastructure
{
    public interface IViewManager
    {
        void Register<T>(IWindowView<T> view) where T : class, IWindowViewModel;
        IWindowView<T> ShowWindow<T>(T viewModel) where T : class, IWindowViewModel;
        void ShowModalWindow<T>(T viewModel) where T : class, IWindowViewModel;
        IWindowView<T> ShowWindowOutOfStack<T>(T viewModel) where T : class, IWindowViewModel;
    }
}