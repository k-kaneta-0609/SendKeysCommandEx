using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SendKeysCommandEx
{
    class ItemMovableListBoxBehavior : Behavior<ListBox>
    {
        public ItemMovableListBoxBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.ToolTip = "[Ctrl]+[↑][↓]キー押下で上下に移動できます。";

            this.AssociatedObject.PreviewKeyDown += listBox_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PreviewKeyDown -= listBox_PreviewKeyDown;
        }

        private void listBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;
            if (listBox.Items.Count < 1) return;
            if (listBox.SelectedIndex < 0) return;

            int selectedItemIndex = listBox.SelectedIndex;

            if (ModifierKeys.Control.Equals(Keyboard.Modifiers))
            {
                int newSelectedIndex = -1;
                if (Key.Up.Equals(e.Key))
                {
                    // [Ctrl]+[↑]押下時
                    if (selectedItemIndex != 0)
                        newSelectedIndex = selectedItemIndex - 1;
                    e.Handled = true;
                }
                if (Key.Down.Equals(e.Key))
                {
                    // [Ctrl]+[↓]押下時
                    if (selectedItemIndex != listBox.Items.Count - 1)
                        newSelectedIndex = selectedItemIndex + 1;
                    e.Handled = true;
                }
                if (newSelectedIndex >= 0)
                {
                    listBoxMoveItemContent(listBox, selectedItemIndex, newSelectedIndex);
                }
            }
        }

        private void listBoxMoveItemContent(ListBox target, int fromItemIndex, int toItemIndex)
        {
            // コンテンツの入れ替え
            object currentItemContent =
                ((ListBoxItem)target.ItemContainerGenerator.ContainerFromIndex(fromItemIndex)).Content;
            target.Items.RemoveAt(fromItemIndex);
            target.Items.Insert(toItemIndex, currentItemContent);

            // 移動先アイテムを選択
            target.SelectedIndex = toItemIndex;

            // 移動先のListBoxItemを選択状態にしたいがその方法が分からない…
            //((ListBoxItem)target.ItemContainerGenerator.ContainerFromIndex(0)).Focus();
        }
    }
}
