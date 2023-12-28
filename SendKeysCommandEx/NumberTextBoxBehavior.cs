using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SendKeysCommandEx
{
    class NumberTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// 負数を入力可能にする場合、Trueを設定する。
        /// (デフォルト値 : False)
        /// </summary>
        public bool AllowNegativeValue
        {
            get { return (bool)GetValue(AllowNegativeValueProperty); }
            set { SetValue(AllowNegativeValueProperty, value); }
        }
        public static readonly DependencyProperty AllowNegativeValueProperty =
            DependencyProperty.Register("AllowNegativeValue", typeof(bool), typeof(NumberTextBoxBehavior), new UIPropertyMetadata(false));

        /// <summary>
        /// 空欄を許可する場合、Trueを設定する。
        /// (デフォルト値 : False)
        /// </summary>
        public bool AllowEmpty
        {
            get { return (bool)GetValue(AllowEmptyProperty); }
            set { SetValue(AllowEmptyProperty, value); }
        }
        public static readonly DependencyProperty AllowEmptyProperty =
            DependencyProperty.Register("AllowEmpty", typeof(bool), typeof(NumberTextBoxBehavior), new UIPropertyMetadata(false));

        /// <summary>
        /// 最小値を設定する。
        /// (デフォルト値 : 0)
        /// </summary>
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(NumberTextBoxBehavior), new UIPropertyMetadata(0));

        /// <summary>
        /// 最大値を設定する。
        /// (デフォルト値 : int.MaxValue)
        /// </summary>
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberTextBoxBehavior), new UIPropertyMetadata(int.MaxValue));

        public NumberTextBoxBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PreviewTextInput += intTextBox_PreviewTextInput;
            this.AssociatedObject.LostFocus += intTextBox_LostFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.PreviewTextInput += intTextBox_PreviewTextInput;
            this.AssociatedObject.LostFocus += intTextBox_LostFocus;
        }

        /// <summary>
        /// 数値専用テキストボックスの入力制限
        /// </summary>
        private void intTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox target = sender as TextBox;
            int value;
            if (int.TryParse(e.Text, out value) == false)
            {
                if (AllowNegativeValue)
                {
                    // 負数を許可する場合
                    if (string.IsNullOrEmpty(target.Text))
                    {
                        if ("-".Equals(e.Text) == false)
                        {
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    // 負数不許可の場合
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 数値専用テキストボックスの値を数値表示にする
        /// </summary>
        private void intTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox target = sender as TextBox;
            int value;
            if (int.TryParse(target.Text, out value))
            {
                if (value < MinValue)
                {
                    target.Text = MinValue.ToString();
                }
                else if (value > MaxValue)
                {
                    target.Text = MaxValue.ToString();
                }
                else
                {
                    target.Text = value.ToString();
                }
            }
            else
            {
                if (AllowEmpty)
                {
                    // 空欄を許可する場合
                    if (string.IsNullOrEmpty(target.Text) == false)
                    {
                        if (MinValue > 0)
                            target.Text = MinValue.ToString();
                        else
                            target.Text = "0";
                    }
                }
                else
                {
                    // 空欄を許可しない場合
                    if (MinValue > 0)
                        target.Text = MinValue.ToString();
                    else
                        target.Text = "0";
                }
            }
        }
    }
}
