using UI.Infrastructure;

namespace UI.View
{
    public interface IPlayerContainerViewModel : IViewModel
    {
        string Title { get; }
        ISpaceWithNumbersViewModel SpaceNumbers { get; }
    }
}