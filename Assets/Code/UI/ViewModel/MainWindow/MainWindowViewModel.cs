using System;
using Core;
using UI.Infrastructure;
using UI.View;

namespace UI.ViewModel
{
    internal sealed class MainWindowViewModel : IMainWindowViewModel
    {
        public event Action<IWindowViewModel> OnClosed;
        public ISpaceWithNumbersViewModel MainSpaceNumbers { get; } 
        public IGameStateViewModel GameState { get; }
        public IPlayerContainerViewModel AIContainer { get; }
        public IPlayerContainerViewModel CharacterContainer { get; }

        public MainWindowViewModel(GameController gameController)
        {
            MainSpaceNumbers = new MainSpaceWithNumbersViewModel(gameController);
            GameState = new GameStateViewModel(gameController);
            AIContainer = new PlayerContainerViewModel(SpaceType.AI, gameController);
            CharacterContainer = new PlayerContainerViewModel(SpaceType.Character, gameController);
        }

        public void Close()
        {
            OnClosed?.Invoke(this);
        }

        public void Dispose()
        {
        }
    }
}
