using System;
using UniRx;

namespace Core
{
    public sealed class AIController : IDisposable
    {
        private readonly GameController _gameController;
        private readonly IDisposable _disposable;
        public AIController(GameController gameController)
        {
            _gameController = gameController;
            _disposable = _gameController.State.Subscribe(OnStateChange);
        }

        private void OnStateChange(GameState state)
        {
            if (state == GameState.AIProcess)
            {
                int random = _gameController.GetRandom();
                _gameController.SetAINumber(random);
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
