using Core;
using UI.Infrastructure;
using UI.View;
using UnityEngine;

namespace UI.ViewModel
{
    public sealed class MainWindowPresenter
    {
        private readonly IViewManager _viewManager;
        private IMainWindowViewModel _viewModel;

        public MainWindowPresenter(IViewManager viewManager)
        {
            _viewManager = viewManager;
            var mainMenuView = Object.FindFirstObjectByType<MainWindowView>();
            _viewManager.Register(mainMenuView);
        }
        
        public void Show(GameController gameController)
        {
            _viewModel = new MainWindowViewModel(gameController);
            _viewManager.ShowWindow(_viewModel);
        }

        public void TryClose()
        {
            _viewModel?.Close();
        }
    }
}