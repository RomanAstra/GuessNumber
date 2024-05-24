using UI.Infrastructure;
using UniRx;

namespace UI.View
{
    public interface IGameStateViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<string> Title { get; }
    }
}