using TMPro;
using UI.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public sealed class PopUpView : MonoBehaviour, IWindowView<IPopUpViewModel>
    {
        public IWindowViewModel ViewModel => _viewModel;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _buttonTitle;
        [SerializeField] private Button _button;
        private IPopUpViewModel _viewModel;

        public void Initialize(IPopUpViewModel viewModel)
        {
            _viewModel = viewModel;
            _title.text = _viewModel.Title;
            _buttonTitle.text = _viewModel.ButtonTitle;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _button.onClick.AddListener(ButtonOnClick);
        }

        private void ButtonOnClick()
        {
            _viewModel.OnClick();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _button.onClick.RemoveListener(ButtonOnClick);
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
