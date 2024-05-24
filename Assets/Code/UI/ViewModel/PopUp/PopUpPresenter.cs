 using Core;
 using Cysharp.Threading.Tasks;
using UI.Infrastructure;
using UI.View;
using UnityEngine;

namespace UI.ViewModel
{
    public sealed class PopUpPresenter
    {
        private readonly IViewManager _viewManager;

        public PopUpPresenter(IViewManager viewManager)
        {
            _viewManager = viewManager;
            var view = Object.FindFirstObjectByType<PopUpView>(FindObjectsInactive.Include);
            _viewManager.Register(view);
        }
        
        public async UniTask ShowAsync(GameState gameState)
        {
            IPopUpViewModel viewModel = new PopUpViewModel(gameState);
            _viewManager.ShowWindow(viewModel);
            await viewModel.WaitForCloseAsync();
        }
    }
}
