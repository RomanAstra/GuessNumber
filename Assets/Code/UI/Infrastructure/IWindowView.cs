namespace UI.Infrastructure
{
    public interface IWindowView
    {
        IWindowViewModel ViewModel { get; }
        void Show();
        void Hide();
        void Unfocus();
        public void Focus();
    }
    
    public interface IWindowView<in T> : IWindowView where T : IWindowViewModel
    {
        void Initialize(T viewModel);
    }
}
