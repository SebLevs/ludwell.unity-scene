using System;
using System.Collections.Generic;

namespace Ludwell.Scene.Editor
{
    public class Disposer
    {
        private HashSet<IDisposable> _disposables = new();

        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void Remove(IDisposable disposable)
        {
            _disposables.Remove(disposable);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public void Clear()
        {
            _disposables.Clear();
        }
    }
}