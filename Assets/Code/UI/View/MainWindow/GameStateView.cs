using TMPro;
using UI.Infrastructure;
using UniRx;
using UnityEngine;

namespace UI.View
{
    internal sealed class GameStateView : MonoBehaviour, IView<IGameStateViewModel>
    {
        public IGameStateViewModel ViewModel => _viewModel;
        [SerializeField] private TMP_Text _title;
        private IGameStateViewModel _viewModel;
        private CompositeDisposable _disposable = new();

        public void Initialize(IGameStateViewModel viewModel)
        {
            _disposable = new CompositeDisposable();
            _viewModel = viewModel;
            _viewModel.Title.Subscribe(OnTitleChange).AddTo(_disposable);
        }

        private void OnTitleChange(string title)
        {
            _title.text = title;
        }

        public void Release()
        {
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _viewModel.Dispose();
        }
    }
}