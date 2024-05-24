using System;
using Core;
using Cysharp.Threading.Tasks;
using UI.Infrastructure;
using UI.View;

namespace UI.ViewModel
{
    internal sealed class PopUpViewModel : IPopUpViewModel
    {
        private const string AGAIN = "Want to try again?";
        public event Action<IWindowViewModel> OnClosed;

        public string Title { get; }
        public string ButtonTitle { get; }
        
        private readonly UniTaskCompletionSource _closeCs = new();

        public PopUpViewModel(GameState gameState)
        {
            string title = gameState switch
            {
                GameState.CharacterWin => $"Character win. {AGAIN}",
                GameState.AIWin => $"Ai win. {AGAIN}",
                _ => AGAIN
            };

            Title = title;
            ButtonTitle = "Yes";
        }

        public void Close()
        {
            OnClosed?.Invoke(this);
            _closeCs.TrySetResult();
        }

        public void OnClick()
        {
            Close();
        }
        
        public async UniTask WaitForCloseAsync()
        {
            await _closeCs.Task;
        }

        public void Dispose()
        {
        }
    }
}
