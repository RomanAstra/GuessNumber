using System.Collections.Generic;
using Helpers;
using UI.Infrastructure;
using UniRx;
using UnityEngine;

namespace UI.View
{
    internal sealed class SpaceWithNumbersView : MonoBehaviour, IView<ISpaceWithNumbersViewModel>
    {
        public ISpaceWithNumbersViewModel ViewModel => _viewModel;
        
        [SerializeField] private Transform _numbersContainer;
        [SerializeField] private NumberView _numberViewPrefab;
        private ISpaceWithNumbersViewModel _viewModel;
        private Pool<NumberView> _pool;
        private readonly List<NumberView> _numberViews = new();
        private CompositeDisposable _disposable = new();

        public void Initialize(ISpaceWithNumbersViewModel viewModel)
        {
            _viewModel = viewModel;
            _disposable = new CompositeDisposable();
            _pool = new Pool<NumberView>(_numberViewPrefab);
            IReadOnlyCovariantReactiveCollection<INumberViewModel> numbers = _viewModel.NumberViewModels;
            for (var index = 0; index < numbers.Count; index++)
            {
                INumberViewModel numberViewModel = numbers[index];
                AddNumberView(numberViewModel, index);
            }

            numbers.ObserveAdd().Subscribe(OnAddNumber).AddTo(_disposable);
            numbers.ObserveRemove().Subscribe(OnRemoveNumber).AddTo(_disposable);
            numbers.ObserveReplace().Subscribe(OnReplaceNumber).AddTo(_disposable);
        }

        private void AddNumberView(INumberViewModel numberViewModel, int index)
        {
            NumberView numberView = _pool.Get(Vector3.zero, Quaternion.identity, _numbersContainer);
            numberView.Initialize(numberViewModel);
            _numberViews.Insert(index, numberView);
        }

        private void RemoveNumberView(int index)
        {
            NumberView numberView = _numberViews[index];
            _numberViews.RemoveAt(index);
            _pool.Release(numberView);
        }

        private void OnAddNumber(ICovariantCollectionAddEvent<INumberViewModel> addEvent)
        {
            AddNumberView(addEvent.Value, addEvent.Index);
        }

        private void OnRemoveNumber(ICovariantCollectionRemoveEvent<INumberViewModel> removeEvent)
        {
            RemoveNumberView(removeEvent.Index);
        }

        private void OnReplaceNumber(ICovariantCollectionReplaceEvent<INumberViewModel> replaceEvent)
        {
            RemoveNumberView(replaceEvent.Index);
            AddNumberView(replaceEvent.NewValue, replaceEvent.Index);
        }

        public void Release()
        {
            for (var index = 0; index < _numberViews.Count; index++)
            {
                NumberView numberView = _numberViews[index];
                _pool.Release(numberView);
            }
            _numberViews.Clear();
        }

        public void Dispose()
        {
            Release();
            _viewModel.Dispose();
            _pool.Dispose();
            _disposable.Dispose();
        }
    }
}
