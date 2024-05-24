using Cysharp.Threading.Tasks;
using UI.Infrastructure;

namespace UI.View
{
    public interface IPopUpViewModel : IWindowViewModel
    {
        string Title { get; }
        string ButtonTitle { get; }
        void OnClick();
        UniTask WaitForCloseAsync();
    }
}
