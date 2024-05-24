using UI.Infrastructure;

namespace UI.View
{
    public interface IMainWindowViewModel : IWindowViewModel
    {
        ISpaceWithNumbersViewModel MainSpaceNumbers { get; }
        IGameStateViewModel GameState { get; }
        IPlayerContainerViewModel AIContainer { get; }
        IPlayerContainerViewModel CharacterContainer { get; }
    }
}
