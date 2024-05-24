using Core;
using UI.View;
using UniRx;

namespace UI.ViewModel
{
    internal sealed class AISpaceWithNumbersViewModel : ISpaceWithNumbersViewModel
    {
        public IReadOnlyCovariantReactiveCollection<INumberViewModel> NumberViewModels => _numbers;

        private readonly CovariantReactiveCollection<NumberViewModel> _numbers = new();
        private readonly CompositeDisposable _disposable = new ();

        public AISpaceWithNumbersViewModel(GameController gameController)
        {
            gameController.AISelectNumber.
                SkipLatestValueOnSubscribe().
                Subscribe(OnSelectNumber).
                AddTo(_disposable);
        }

        private void OnSelectNumber(int number)
        {
            string title = number.ToString();
            _numbers.Add(new NumberViewModel(number, title, false));
        }

        public void Dispose()
        {
            _numbers.Clear();
            _disposable.Dispose();
        }
    }
}
