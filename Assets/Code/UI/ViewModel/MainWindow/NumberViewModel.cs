using Helpers;
using UI.View;

namespace UI.ViewModel
{
    internal sealed class NumberViewModel : INumberViewModel
    {
        internal EventAction<int> SelectNumber { get; }
        public int Number { get; }
        public string Title { get; }
        public bool IsActive { get; }
        
        public NumberViewModel(int number, string title, bool isActive)
        {
            Number = number;
            Title = title;
            IsActive = isActive;
            SelectNumber = new EventAction<int>();
        }
        
        public void OnClick()
        {
            if (IsActive)
            {
                SelectNumber.Invoke(Number);
            }
        }
        
        public void Dispose()
        {
            SelectNumber.Dispose();
        }
    }
}
