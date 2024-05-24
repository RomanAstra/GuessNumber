using Core;
using UI.View;
using UniRx;

namespace UI.ViewModel
{
    internal sealed class CharacterSpaceWithNumbersViewModel : ISpaceWithNumbersViewModel
    {
        public IReadOnlyCovariantReactiveCollection<INumberViewModel> NumberViewModels => _numbers;

        private readonly CovariantReactiveCollection<NumberViewModel> _numbers = new();
        private readonly CompositeDisposable _disposable = new ();

        public CharacterSpaceWithNumbersViewModel(GameController gameController)
        {
            gameController.CharacterSelectNumber.
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
