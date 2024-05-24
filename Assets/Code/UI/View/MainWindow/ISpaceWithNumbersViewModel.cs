using UI.Infrastructure;
using UniRx;

namespace UI.View
{
    public interface ISpaceWithNumbersViewModel : IViewModel
    {
        IReadOnlyCovariantReactiveCollection<INumberViewModel> NumberViewModels { get; }
    }
}
