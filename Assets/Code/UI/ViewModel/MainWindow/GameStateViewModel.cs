using Core;
using UI.View;
using UniRx;

namespace UI.ViewModel
{
    internal sealed class GameStateViewModel : IGameStateViewModel
    {
        public IReadOnlyReactiveProperty<string> Title => _title;
        private readonly StringReactiveProperty _title;
        private readonly CompositeDisposable _disposable = new();
        private readonly GameController _gameController;

        public GameStateViewModel(GameController gameController)
        {
            _gameController = gameController;
            _title = new StringReactiveProperty("Choose a number");
            _gameController.CharacterSelectNumber.
                SkipLatestValueOnSubscribe().
                Subscribe(OnSelectNumberChange).
                AddTo(_disposable);
            _gameController.AISelectNumber.
                SkipLatestValueOnSubscribe().
                Subscribe(OnSelectNumberChange).
                AddTo(_disposable);
        }

        private void OnSelectNumberChange(int selectNumber)
        {
            if (_gameController.HiddenNumber < selectNumber)
            {
                _title.Value = "You need to choose a smaller number";
            }
            else if (_gameController.HiddenNumber > selectNumber)
            {
                _title.Value = "You need to choose a larger number";
            }
            else
            {
                _title.Value = $"Everything is correct, the hidden number is {selectNumber}";
            }
        }

        public void Dispose()
        {
        }
    }
}