using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace EnigmaMM
{
    /// <summary>
    /// http://bea.stollnitz.com/blog/?p=34
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InvokeOC<T> : ObservableCollection<T> 
    {
        private Dispatcher dispatcherUIThread;

        private delegate void SetItemCallback(int index, T item);
        private delegate void RemoveItemCallback(int index);
        private delegate void ClearItemsCallback();
        private delegate void InsertItemCallback(int index, T item);
        private delegate void MoveItemCallback(int oldIndex, int newIndex);

        public InvokeOC(Dispatcher dispatcher)
        {
            this.dispatcherUIThread = dispatcher;
        }

        protected override void SetItem(int index, T item)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new SetItemCallback(SetItem), index, new object[] { item });
            }
        }

        protected override void RemoveItem(int index)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new RemoveItemCallback(RemoveItem), index);
            }
        }
        
        protected override void ClearItems()
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new ClearItemsCallback(ClearItems));
            }
        }

        protected override void InsertItem(int index, T item)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.InsertItem(index, item);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new InsertItemCallback(InsertItem), index, new object[] { item });
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.MoveItem(oldIndex, newIndex);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new MoveItemCallback(MoveItem), oldIndex, new object[] { newIndex });
            }
        }
    }
}
