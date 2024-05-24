using TMPro;
using UI.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public sealed class NumberView : MonoBehaviour, IView<INumberViewModel>
    {
        public INumberViewModel ViewModel => _viewModel;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Button _button;
        private INumberViewModel _viewModel; 

        public void Initialize(INumberViewModel viewModel)
        {
            _viewModel = viewModel;
            _title.text = _viewModel.Title;
            _button.interactable = _viewModel.IsActive;
            if (_viewModel.IsActive)
            {
                _button.onClick.AddListener(_viewModel.OnClick);
            }
        }

        public void Release()
        {
            if (_viewModel.IsActive)
            {
                _button.onClick.RemoveListener(_viewModel.OnClick);
            }
        }

        public void Dispose()
        {
            _viewModel.Dispose();
        }
    }
}
