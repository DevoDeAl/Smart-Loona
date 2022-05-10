#region Библитеки
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#endregion
namespace DarkWowSoft
{
    /// <summary>
    /// Логика взаимодействия для linkWindow.xaml
    /// </summary>
    /// 
    public partial class sqlBox : Window
    {
        TextBox textBox = null;
        #region Констурктор
        public sqlBox(string sql)
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/MainIcon.png"));
            textBox = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5, 5, 5, 5),
                FontSize = 16,
                Text = sql,
                ContextMenu = new ContextMenu(),
                AcceptsReturn = true
            };
            MainRoot.Children.Add(textBox);
            #region Добавление ContextMenu
            MenuItem copyContextDGName = new MenuItem { Header = "Копировать", Tag = "DGname" };
            textBox.ContextMenu.Items.Add(copyContextDGName);
            copyContextDGName.Click += CopyContext_Click;
            #endregion
            Show();
        }

        private void CopyContext_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox.Text);
        }
        #endregion

    }
}
