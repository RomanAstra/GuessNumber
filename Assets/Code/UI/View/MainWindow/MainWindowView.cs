using UI.Infrastructure;
using UnityEngine;

namespace UI.View
{
    public sealed class MainWindowView : MonoBehaviour, IWindowView<IMainWindowViewModel>
    {
        public IWindowViewModel ViewModel => _viewModel;
        
        [SerializeField] private SpaceWithNumbersView _mainSpace;
        [SerializeField] private GameStateView _gameState;
        [SerializeField] private PlayerContainerView _aiView;
        [SerializeField] private PlayerContainerView _characterView; 
        private IMainWindowViewModel _viewModel;

        public void Initialize(IMainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _mainSpace.Initialize(_viewModel.MainSpaceNumbers);
            _gameState.Initialize(_viewModel.GameState);
            _aiView.Initialize(_viewModel.AIContainer);
            _characterView.Initialize(_viewModel.CharacterContainer);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _mainSpace.Dispose();
            _gameState.Dispose();
            _aiView.Dispose();
            _characterView.Dispose();
            _viewModel.Dispose();
        }

        public void Unfocus()
        {
        }

        public void Focus()
        {
        }
    }
}
