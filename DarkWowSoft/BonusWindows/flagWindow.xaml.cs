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
    public partial class flagWindow : Window
    {
        #region Основные данные
        System.Windows.Controls.DataGrid dataGrid = null;
        int sum = 0;
        #region События
        public delegate void СhoiceDone(object sender, EventArgs e);
        public event СhoiceDone СhoiceIsDone;
        public delegate void ClosedButton(object sender, EventArgs e);
        public event ClosedButton closedButton;
        #endregion
        #endregion
        #region Констурктор
        public flagWindow(IEnumerable<(string, int)> dataGridData, BitmapImage image, List<int> selectedList = null)
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/MainIcon.png"));
            #region Размер окна
            MaxHeight = MinHeight = 350; MaxWidth = MinWidth = 300;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            #endregion
            #region Фон
            Rectangle background = new System.Windows.Shapes.Rectangle()
            {
                Fill = new ImageBrush
                {
                    ImageSource = image,
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(0, 0, image.Width / 2, image.Height / 2),
                    ViewportUnits = BrushMappingMode.Absolute
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            #endregion
            #region Создание DataGrid
            #region Стиль выделения в DataGrid
            Style style_dg = new Style(typeof(DataGridCell));
            Trigger trigger_dg = new Trigger() { Property = DataGridCell.IsSelectedProperty, Value = true };
            //style_dg.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));
            style_dg.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));
            trigger_dg.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.DodgerBlue)));
            trigger_dg.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.White)));
            trigger_dg.Setters.Add(new Setter(DataGridCell.FontWeightProperty, FontWeights.Bold));
            style_dg.Triggers.Add(trigger_dg);
            #endregion
            dataGrid = new System.Windows.Controls.DataGrid
            {
                Height = Height * 0.8,
                Width = Width * 0.92, //wrapPanelDataGrid.Width / 1.5,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MinHeight = 1,
                MinWidth = 1,
                CanUserSortColumns = true,
                FontSize = 11,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                SelectionMode = DataGridSelectionMode.Extended,
                IsReadOnly = true,
                //ContextMenu = new System.Windows.Controls.ContextMenu(),
                BorderThickness = new Thickness(2),
                Tag = "DG",
                Margin = new Thickness(0, 15, 0, 15),
                CellStyle = style_dg,
                Opacity = 0.98
            };
            #region Добавление заголовков к колонкам DataGrid
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            DataGridTextColumn idColumn = new DataGridTextColumn { Width = 25, Header = "ID", Binding = new System.Windows.Data.Binding("id"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(idColumn);
            DataGridTextColumn commentColumn = new DataGridTextColumn { Width = 249, Header = "Имя", Binding = new System.Windows.Data.Binding("comment"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(commentColumn);
            #endregion
            #region Заполнение DataGrid
            foreach ((string, int) data in dataGridData)
            {
                dataGrid.Items.Add(new DataGridItem() { id = data.Item2, comment = data.Item1 });
            }
            #endregion
            #region Выделение
            if (selectedList != null && selectedList.Count > 0)
            {
                foreach (DataGridItem i in dataGrid.Items)
                {
                    if (selectedList.Contains(i.id))
                    {
                        dataGrid.SelectedItems.Add(i);
                    }
                }
            }
            #endregion
            #endregion
            #region Создание кнопок
            Button buttonOk = new Button()
            {
                Content = "Выбрать",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = MaxWidth / 2 - 30,
                Height = 30,
                Margin = new Thickness(5, -10, 0, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Opacity = 0.98
            };
            Button buttonCancel = new Button()
            {
                Content = "Отмена",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = MaxWidth / 2 - 30,
                Height = 30,
                Margin = new Thickness(5, -10, 0, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Opacity = 0.98
            };
            #endregion
            #region Сборка элементов
            MainRoot.Children.Add(background); Grid.SetRow(background, 0); Grid.SetRowSpan(background, 7); Grid.SetColumnSpan(background, 2);
            MainRoot.Children.Add(dataGrid); Grid.SetRow(dataGrid, 0); Grid.SetRowSpan(dataGrid, 6); Grid.SetColumnSpan(dataGrid, 2);
            MainRoot.Children.Add(buttonOk); Grid.SetRow(buttonOk, 7); Grid.SetColumn(buttonOk, 0);
            MainRoot.Children.Add(buttonCancel); Grid.SetRow(buttonCancel, 7); Grid.SetColumn(buttonCancel, 1);
            #endregion
            #region Подписка на события
            buttonCancel.Click += ButtonCancel_Click;
            buttonOk.Click += ButtonOk_Click;
            #endregion
            Show();
        }
        #endregion
        #region Специальные функции
        public int GetIdsSum()
        {
            return sum;
        }
        #endregion
        #region События
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DataGridItem[] items = dataGrid.SelectedItems.Cast<DataGridItem>().ToArray();
            if (items.Length != 0)
            {
                for (int i = 0; i < items.Length; i++) 
                {
                    DataGridItem item = items[i];
                    sum += item.id;
                }
                #region Активация события
                EventArgs args = new EventArgs();
                СhoiceIsDone(this, args);
                #endregion
            }
        }
        private  void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            #region Активация события
            EventArgs args = new EventArgs();
            closedButton(this, args);
            #endregion
        }
        #endregion
        #region Вспомогательные классы
        public class DataGridItem
        {
            public int id { get; set; }
            public string comment { get; set; }
        }
        #endregion
    }
}
