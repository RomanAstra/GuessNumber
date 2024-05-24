using UI.Infrastructure;

namespace UI.View
{
    public interface INumberViewModel : IViewModel
    {
        int Number { get; }
        string Title { get; }
        bool IsActive { get; }
        void OnClick();
    }
}
