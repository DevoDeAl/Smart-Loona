#region Библитеки
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#endregion
namespace DarkWowSoft
{
    /// <summary>
    /// Логика взаимодействия для linkWindow.xaml
    /// </summary>
    public partial class actionWindow : Window
    {
        #region Свойства для купации крестика
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion
        #region Основные данные
        System.Windows.Controls.DataGrid dataGrid = null;
        int result = 0;
        public bool selectionMode = false;
        public TextBox textBox = null;
        public IEnumerable<(string, int)> dataArray = null;
        #region События
        public delegate void СhoiceDone(object sender, EventArgs e);
        public event СhoiceDone СhoiceIsDone;
        public delegate void ClosedButton(object sender, EventArgs e);
        public event ClosedButton closedButton;
        #endregion
        #endregion
        #region Констурктор
        public actionWindow(IEnumerable<(string, int)> dataGridData, BitmapImage image, ref TextBox textBox, bool selectionMode = true, List<int> selectedList = null, string name = "Выбор")
        {
            InitializeComponent();
            this.Title = name;
            #region Иконка
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/MainIcon.png"));
            #endregion
            #region Присовение свойтсв
            this.selectionMode = selectionMode;
            this.textBox = textBox;
            this.dataArray = dataGridData;
            #endregion
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
                SelectionMode = (selectionMode) ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single,
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
            DataGridTextColumn idColumn = new DataGridTextColumn { Width = 45, Header = "ID", Binding = new System.Windows.Data.Binding("id"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(idColumn);
            DataGridTextColumn commentColumn = new DataGridTextColumn { Width = 229, Header = "Имя", Binding = new System.Windows.Data.Binding("comment"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(commentColumn);
            #endregion
            #region Заполнение DataGrid
            foreach ((string, int) data in dataGridData)
            {
                dataGrid.Items.Add(new DataGridItem() { id = data.Item2, comment = data.Item1 });
            }
            #endregion
            #region Выделение
            if (selectionMode)
            {
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
            }
            else
            {
                int selectedInt = selectedList[0];
                try
                {
                    foreach (DataGridItem i in dataGrid.Items)
                    {
                        if (selectedInt == i.id)
                        {
                            dataGrid.SelectedItem = i;
                            break;
                        }
                    }
                }
                catch { }
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
            this.Loaded += ActionWindow_Loaded;
            #endregion
        }

        #endregion
        #region Специальные функции
        /// <summary>
        /// Перезагрузка окна
        /// </summary>
        public void reset(List<int> selectedList = null)
        {
            result = 0;
            if (selectionMode)
            {
                dataGrid.SelectedItems.Clear();
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
            }
            else
            {
                dataGrid.SelectedItem = null;

                int selectedInt = selectedList[0];
                try
                {
                    foreach (DataGridItem i in dataGrid.Items)
                    {
                        if (selectedInt == i.id)
                        {
                            dataGrid.SelectedItem = i;
                            break;
                        }
                    }
                }
                catch { }
            }
        }
        /// <summary>
        /// Получение суммы ID
        /// </summary>
        public void GetIdsSum()
        {
            if (this.textBox != null) { textBox.Text = result.ToString(); }
        }
        #endregion
        #region События
        /// <summary>
        /// Событие при нажатии кнопки "Выбрать". Подсчет суммы ID
        /// </summary>
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (selectionMode)
            {
                DataGridItem[] items = dataGrid.SelectedItems.Cast<DataGridItem>().ToArray();
                if (items.Length != 0)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        DataGridItem item = items[i];
                        result += item.id;
                    }
                    #region Активация события
                    EventArgs args = new EventArgs();
                    СhoiceIsDone(this, args);
                    #endregion
                }
            }
            else
            {
                DataGridItem item = dataGrid.SelectedItem as DataGridItem;
                if (item != null)
                {
                    result = item.id;
                    #region Активация события
                    EventArgs args = new EventArgs();
                    СhoiceIsDone(this, args);
                    #endregion
                }
            }
        }
        /// <summary>
        /// Событие при нажатии кнопки "Отмена".
        /// </summary>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            #region Активация события
            EventArgs args = new EventArgs();
            closedButton(this, args);
            #endregion
        }
        #region Для купирования крестика
        private void ActionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion
        #endregion
        #region Вспомогательные классы
        /// <summary>
        /// Элемент сетки данных.
        /// </summary>
        public class DataGridItem
        {
            public int id { get; set; }
            public string comment { get; set; }
        }
        #endregion
    }

}
