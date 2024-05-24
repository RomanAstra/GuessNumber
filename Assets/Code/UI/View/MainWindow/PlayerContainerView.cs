using TMPro;
using UI.Infrastructure;
using UnityEngine;

namespace UI.View
{
    internal sealed class PlayerContainerView : MonoBehaviour, IView<IPlayerContainerViewModel>
    {
        public IPlayerContainerViewModel ViewModel => _viewModel;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private SpaceWithNumbersView _space;
        private IPlayerContainerViewModel _viewModel;

        public void Initialize(IPlayerContainerViewModel viewModel)
        {
            _viewModel = viewModel;
            _title.text = _viewModel.Title;
            _space.Initialize(_viewModel.SpaceNumbers);
        }

        public void Release()
        {
            _space.Release();
        }

        public void Dispose()
        {
            _space.Dispose();
            _viewModel.Dispose();
        }
    }
}