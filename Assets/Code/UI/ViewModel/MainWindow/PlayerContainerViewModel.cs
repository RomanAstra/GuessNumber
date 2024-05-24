using System;
using Core;
using UI.View;

namespace UI.ViewModel
{
    internal sealed class PlayerContainerViewModel : IPlayerContainerViewModel
    {
        public string Title { get; }
        public ISpaceWithNumbersViewModel SpaceNumbers { get; }

        public PlayerContainerViewModel(SpaceType spaceType, GameController gameController)
        {
            switch (spaceType)
            {
                case SpaceType.Character:
                    Title = "character space";
                    SpaceNumbers = new CharacterSpaceWithNumbersViewModel(gameController);
                    break;
                case SpaceType.AI:
                    Title = "enemy space";
                    SpaceNumbers = new AISpaceWithNumbersViewModel(gameController);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spaceType));
            }
        }
        
        public void Dispose()
        {
        }
    }
}
