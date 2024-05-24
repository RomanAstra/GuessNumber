using System;
using System.Collections.Generic;

namespace UI.Infrastructure
{
    public sealed class ViewManager : IViewManager
    {
        private readonly Dictionary<Type, IWindowView> _viewMap = new (4);
        private readonly Stack<IWindowView> _viewsStack = new ();
        private readonly List<IWindowView> _outOfStackWindows = new ();
        private readonly Queue<Action> _modalWindowsCallsQueue = new ();
        private IWindowView _modalWindow;
        
        public void Register<T>(IWindowView<T> view) where T : class, IWindowViewModel
        {
            if (view == null)
            {
                throw new ArgumentNullException();
            }

            Type type = typeof(T);
            if (!_viewMap.ContainsKey(type))
            {
                _viewMap.Add(type, view);
                _viewMap[type] = view;
            }
            else
            {
                throw new ArgumentException($"There is another view of type {type}");
            }
        }

        public IWindowView<T> ShowWindow<T>(T viewModel) where T : class, IWindowViewModel
        {
            var view = GetView<T>();

            TryCloseViewIfItAlreadyShown(view);

            viewModel.OnClosed += Close;
            view.Initialize(viewModel);

            if (_viewsStack.Count > 0)
            {
                _viewsStack.Peek().Unfocus();
            }

            view.Show();
            view.Focus();

            _viewsStack.Push(view);
            return view;
        }

        public IWindowView<T> ShowWindowOutOfStack<T>(T viewModel) where T : class, IWindowViewModel
        {
            var view = GetView<T>();

            TryCloseViewIfItAlreadyShown(view);

            viewModel.OnClosed += Close;
            view.Initialize(viewModel);

            view.Show();
            view.Focus();

            _outOfStackWindows.Add(view);
            return view;
        }

        public void ShowModalWindow<T>(T viewModel) where T : class, IWindowViewModel
        {
            if (_modalWindow != null)
            {
                _modalWindowsCallsQueue.Enqueue(() => ShowModalWindow(viewModel));
                return;
            }

            _modalWindow = ShowWindow(viewModel);
        }

        private IWindowView<T> GetView<T>() where T : class, IWindowViewModel
        {
            Type type = typeof(T);
            if (!_viewMap.TryGetValue(type, out IWindowView view))
            {
                throw new ArgumentNullException($"View type of {type} not founded");
            }
            
            return view as IWindowView<T>;
        }

        private void Close(IWindowViewModel viewModel)
        {
            viewModel.OnClosed -= Close;
            if (_viewsStack.Count == 0)
            {
                throw new InvalidOperationException("Try to close not opened view model");
            }

            IWindowView currentView;
            int index = -1;
            for (int i = 0; i < _outOfStackWindows.Count; i++)
            {
                if (_outOfStackWindows[i].ViewModel == viewModel)
                {
                    index = i;
                }
            }

            if (index >= 0)
            {
                currentView = _outOfStackWindows[index];
                _outOfStackWindows.RemoveAt(index);
            }
            else
            {
                while (_viewsStack.Peek().ViewModel != viewModel)
                {
                    IWindowView view = _viewsStack.Peek();
                    view.ViewModel.Close();
                }

                currentView = _viewsStack.Pop();
            }

            currentView.Unfocus();
            currentView.Hide();

            if (_viewsStack.Count > 0)
            {
                IWindowView view = _viewsStack.Peek();
                view.Focus();
            }

            if (_modalWindow != null && _modalWindow.ViewModel == viewModel)
            {
                _modalWindow = null;
                if (_modalWindowsCallsQueue.Count > 0)
                {
                    _modalWindowsCallsQueue.Dequeue().Invoke();
                }
            }
        }

        private void TryCloseViewIfItAlreadyShown(IWindowView view)
        {
            if (_viewsStack.Contains(view) || _outOfStackWindows.Contains(view))
            {
                view.ViewModel.Close();
            }
        }
    }
}
