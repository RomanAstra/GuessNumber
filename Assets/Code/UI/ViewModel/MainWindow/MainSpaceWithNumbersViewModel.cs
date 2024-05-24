using System;
using Core;
using UI.View;
using UniRx;

namespace UI.ViewModel
{
    internal sealed class MainSpaceWithNumbersViewModel : ISpaceWithNumbersViewModel
    {
        public IReadOnlyCovariantReactiveCollection<INumberViewModel> NumberViewModels => _numbers;

        private readonly CovariantReactiveCollection<NumberViewModel> _numbers = new();
        private readonly CompositeDisposable _disposable = new ();
        private readonly GameController _gameController;

        public MainSpaceWithNumbersViewModel(GameController gameController)
        {
            _gameController = gameController;
            
            for (int i = _gameController.MinNumber; i <= _gameController.MaxNumber; i++)
            {
                string title = i.ToString();
                var numberViewModel = new NumberViewModel(i, title, true);
                numberViewModel.SelectNumber.Subscribe(OnSelectNumber).AddTo(_disposable);
                _numbers.Add(numberViewModel);
            }
            gameController.AISelectNumber.
                SkipLatestValueOnSubscribe().
                Subscribe(OnSelectNumberAI).
                AddTo(_disposable);

            _gameController.Shuffle(_numbers);
        }

        private void OnSelectNumberAI(int number)
        {
            if (_gameController.State.Value != GameState.AIProcess)
            {
                return;
            }
            if (TryGetNumberAndRemove(number))
            {
                _gameController.Shuffle(_numbers);
            }
            else
            {
                throw new InvalidOperationException($"Number {number} not found for AI");
            }
        }

        private void OnSelectNumber(int number)
        {
            if (_gameController.State.Value != GameState.CharacterProcess)
            {
                return;
            }
            if (TryGetNumberAndRemove(number))
            {
                _gameController.SetCharacterNumber(number);
                _gameController.Shuffle(_numbers);
            }
            else
            {
                throw new InvalidOperationException($"Number {number} not found for Character");
            }
        }

        private bool TryGetNumberAndRemove(int foundNumber)
        {
            for (var index = 0; index < _numbers.Count; index++)
            {
                NumberViewModel viewModel = _numbers[index];
                int candidate = viewModel.Number;
                if (candidate.Equals(foundNumber))
                {
                    _numbers.Remove(viewModel);
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            _numbers.Clear();
            _disposable.Dispose();
        }
    }
}
