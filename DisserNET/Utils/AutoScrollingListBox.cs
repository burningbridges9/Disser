using System.Windows.Controls;

namespace DisserNET.Utils
{
    public class AutoScrollingListBox : ListBox
    {
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int newItemCount = e.NewItems is null ? 0 : e.NewItems.Count;

            if (newItemCount > 0)
                this.ScrollIntoView(e.NewItems[newItemCount - 1]);

            base.OnItemsChanged(e);
        }
    }
}
