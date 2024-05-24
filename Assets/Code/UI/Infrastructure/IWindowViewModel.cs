using System;

namespace UI.Infrastructure
{
    public interface IWindowViewModel : IDisposable
    {
        event Action<IWindowViewModel> OnClosed;

        void Close();
    }
    
    public interface IViewModel : IDisposable
    {
    }
}
