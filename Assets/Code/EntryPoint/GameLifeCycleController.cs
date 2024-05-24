using System;
using Core;
using Cysharp.Threading.Tasks;
using Helpers;
using UI.Infrastructure;
using UI.ViewModel;
using UniRx;

namespace EntryPoint
{
    internal sealed class GameLifeCycleController : IDisposable
    {
        private readonly IntRange _startRange;
        private readonly Randomizer _randomizer;
        private readonly MainWindowPresenter _mainWindowPresenter;
        private readonly PopUpPresenter _popUpPresenter;
        private readonly CompositeDisposable _disposable = new ();

        public GameLifeCycleController(IntRange startRange, Randomizer randomizer)
        {
            _startRange = startRange;
            _randomizer = randomizer;
            _mainWindowPresenter = new MainWindowPresenter(new ViewManager());
            _popUpPresenter = new PopUpPresenter(new ViewManager());
        }

        internal void StartGame()
        {
            var gameController = new GameController(_startRange, _randomizer);
            gameController.State.Subscribe(OnStateChange).AddTo(_disposable);
            new AIController(gameController).AddTo(_disposable);
            _mainWindowPresenter.Show(gameController);
        }

        private void OnStateChange(GameState state)
        {
            if (state is GameState.CharacterWin or GameState.AIWin)
            {
                RestartGameAsync(state).Forget();
            }
        }

        private async UniTaskVoid RestartGameAsync(GameState state)
        {
            _disposable.Clear();
            _mainWindowPresenter.TryClose();
            await _popUpPresenter.ShowAsync(state);
            StartGame();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
