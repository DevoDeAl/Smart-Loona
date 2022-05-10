#region Библитеки
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#endregion
namespace DarkWowSoft
{
    /// <summary>
    /// Логика взаимодействия для linkWindow.xaml
    /// </summary>
    public partial class inheritWindow : Window
    {
        #region Основные данные
        System.Windows.Controls.DataGrid dataGrid = null;
        
        public bool selectionMode = false;
        public TextBox textBox = null;
        ComboBox comboBoxType = null;
        TextBox textBoxEntryId = null;
        MySqlConnection conn = null;
        public Dictionary<string, object> source = null;
        string[] smartScriptsColumnNames = new string[]
        {"entryorguid", "source_type",
        "id", "link",
        "event_type", "event_phase_mask",
        "event_chance", "event_flags",
        "event_param1", "event_param2",
        "event_param3", "event_param4",
        "event_param5", "action_type",
        "action_param1", "action_param2",
        "action_param3", "action_param4",
        "action_param5", "action_param6",
        "target_type", "target_param1",
        "target_param2", "target_param3",
        "target_param4", "target_x",
        "target_y", "target_z",
        "target_o", "comment" };
        public string inheritLastId = null;
        public int inheritLastST = -1;
        ObservableCollection<DataGridItem> dataGridItemsSource { get; set; }
        #region События
        public delegate void СhoiceDone(object sender, EventArgs e);
        public event СhoiceDone СhoiceIsDone;
        public delegate void ClosedButton(object sender, EventArgs e);
        public event ClosedButton closedButton;
        #endregion
        #endregion
        #region Констурктор
        public inheritWindow(BitmapImage image, ref DarkWowSoft.MainWindow.DataGridItem item, ref MySqlConnection conn, string inheritLastId = "0", int inheritLastST = -1)
        {
            InitializeComponent();
            this.conn = conn;
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/MainIcon.png"));
            this.inheritLastId = inheritLastId;
            this.inheritLastST = inheritLastST;

            #region Размер окна
            MaxHeight = MinHeight = 500; MaxWidth = MinWidth = 700;
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
            #region Создание границы
            Border border = new Border()
            {
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(2),
                Opacity = 0.90,
                Margin = new Thickness(5, 5, 5, 5),
                CornerRadius = new CornerRadius(10, 10, 10, 10)
            };
            #endregion
            #region Свиток
            StackPanel stackPanelType = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            TextBlock textType = new TextBlock()
            {
                Text = "Source Type",
                FontSize = 12,
                //Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 5, 0, 0),
                TextAlignment = TextAlignment.Left,
            };
            comboBoxType = new ComboBox()
            {
                Width = 105,
                Height = 27,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 5, 0, 0),
                FontWeight = FontWeights.SemiBold,
                FontStretch = new FontStretch(),
                Padding = new Thickness(10, 6.25, 0, 0),
                FontSize = 10
            }; /*comboBoxType.SelectedIndex = 0;*/ 
           (string, int)[] sourceItemArray = new (string, int)[3] { ("CREATURE", 0), ("GAMEOBJECT", 1),("ACTIONLIST", 9)  };
            foreach ((string, int) sourceItem in sourceItemArray)
            {
                comboBoxType.Items.Add(new ComboBoxItem { Content = sourceItem.Item1 + " - " + sourceItem.Item2.ToString(), Tag = sourceItem.Item2, FontWeight = FontWeights.SemiBold, FontSize = 12 });
            }
            comboBoxType.SelectedIndex = inheritLastST == -1 ? 0 : comboBoxType.Items.IndexOf(comboBoxType.Items.Cast<ComboBoxItem>().ToList().Find(x => ((int)x.Tag).Equals(this.inheritLastST)));
            #endregion
            #region Entry ID
            StackPanel stackPanelEntry = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(-345, 0.9, 0, 0),
            };
            TextBlock textEntry = new TextBlock()
            {
                Text = "Entry ID",
                FontSize = 12,
                //Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                TextAlignment = TextAlignment.Left,
            };
            textBoxEntryId = new TextBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                //Text = "localhost",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                FontSize = 16,
                Padding = new Thickness(1.5, 1.625, 0, 0),
                TextAlignment = TextAlignment.Center,
                Text = inheritLastId == null ? "0" : inheritLastId,
            };
            #endregion
            #region Создание DataGrid
            #region Стиль выделения в DataGrid
            Style style_dg = new Style(typeof(DataGridCell));
            Trigger trigger_dg = new Trigger() { Property = DataGridCell.IsSelectedProperty, Value = true };
            ////style_dg.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));
            //style_dg.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));
            trigger_dg.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.DodgerBlue)));
            trigger_dg.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.White)));
            trigger_dg.Setters.Add(new Setter(DataGridCell.FontWeightProperty, FontWeights.Bold));
            //style_dg.Triggers.Add(trigger_dg);
            #endregion
            dataGridItemsSource = new ObservableCollection<DataGridItem>();
            dataGrid = new System.Windows.Controls.DataGrid
            {
                //AutoGenerateColumns = false,
                Height = Height * 0.8,
                //MaxHeight = Height,
                Width = Width * 0.92, //wrapPanelDataGrid.Width / 1.5,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MinHeight = 1,
                MinWidth = 1,
                CanUserSortColumns = true,
                FontSize = 11,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                SelectionMode = DataGridSelectionMode.Single,
                IsReadOnly = false,
                //ContextMenu = new System.Windows.Controls.ContextMenu(),
                BorderThickness = new Thickness(2),
                Tag = "DG",
                Margin = new Thickness(10, 15, 10, 15),
                CellStyle = style_dg,
                Opacity = 0.98,
                ItemsSource = dataGridItemsSource,
                SelectionUnit = DataGridSelectionUnit.Cell,
                AlternatingRowBackground = new SolidColorBrush(Colors.AliceBlue),
            };
            #region Добавление заголовков к колонкам DataGrid
            Style style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            DataGridTextColumn entryColumn = new DataGridTextColumn { Width = 45, Header = "Entry", Binding = new System.Windows.Data.Binding("entry"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(entryColumn);
            DataGridTextColumn idColumn = new DataGridTextColumn { Width = 45, Header = "ID", Binding = new System.Windows.Data.Binding("id"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(idColumn);
            DataGridTextColumn commentColumn = new DataGridTextColumn { Width = 229, Header = "Имя", Binding = new System.Windows.Data.Binding("comment"), FontSize = 11, ElementStyle = style };
            dataGrid.Columns.Add(commentColumn);
            #endregion
            #endregion
            #region Создание кнопок
            Button buttonOk = new Button()
            {
                Content = "Выбрать",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = MaxWidth / 3 - 20,
                Height = 30,
                Margin = new Thickness(0, -10, -233, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Opacity = 0.98
            };
            Button buttonCancel = new Button()
            {
                Content = "Отмена",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = MaxWidth / 3 - 20,
                Height = 30,
                Margin = new Thickness(0, -10, -233, 0),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Opacity = 0.98
            };
            Button buttonFilter = new Button()
            {
                Content = "Поиск",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = MaxWidth / 4 - 20,
                Height = 25,
                Margin = new Thickness(-535, 13, 0, 0),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Opacity = 0.98
            };
            #endregion
            #region Сборка элементов
            MainRoot.Children.Add(background); Grid.SetRow(background, 0); Grid.SetRowSpan(background, 7); Grid.SetColumnSpan(background, 3);

            MainRoot.Children.Add(border); Grid.SetRow(border, 0); Grid.SetRowSpan(border, 6); Grid.SetColumnSpan(border, 3);

            stackPanelType.Children.Add(textType); stackPanelType.Children.Add(comboBoxType);
            MainRoot.Children.Add(stackPanelType); Grid.SetRow(stackPanelType, 0); Grid.SetColumn(stackPanelType, 0);

            stackPanelEntry.Children.Add(textEntry); stackPanelEntry.Children.Add(textBoxEntryId);
            MainRoot.Children.Add(stackPanelEntry); Grid.SetRow(stackPanelEntry, 0); Grid.SetColumn(stackPanelEntry, 1);

            MainRoot.Children.Add(buttonFilter); Grid.SetRow(buttonFilter, 0); Grid.SetColumn(buttonFilter, 2);

            MainRoot.Children.Add(dataGrid); Grid.SetRow(dataGrid, 1); Grid.SetRowSpan(dataGrid, 5); Grid.SetColumnSpan(dataGrid, 3);
            MainRoot.Children.Add(buttonOk); Grid.SetRow(buttonOk, 7); Grid.SetColumn(buttonOk, 0);
            MainRoot.Children.Add(buttonCancel); Grid.SetRow(buttonCancel, 7); Grid.SetColumn(buttonCancel, 1);
            #endregion
            #region Подписка на события
            buttonCancel.Click += ButtonCancel_Click;
            buttonOk.Click += ButtonOk_Click;
            buttonFilter.Click += ButtonFilter_Click;
            textBoxEntryId.PreviewTextInput += TextBoxId_PreviewTextInput;
            dataGrid.SizeChanged += DataGrid_SizeChanged;
            #endregion
            #region Автообновление значения
            if (inheritLastId != null && inheritLastId != "0")
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(buttonFilter);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();
            }
            #endregion
        }
        private void DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dataGrid.Columns[2].Width = dataGrid.ActualWidth - dataGrid.Columns[0].ActualWidth - dataGrid.Columns[1].ActualWidth;
        }
        #endregion
        #region Специальные функции
        #endregion
        #region События
        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            #region Очистка
            //dataGrid.Items.Clear();
            dataGridItemsSource.Clear();
            #endregion
            #region Исходные данные
            string type = ((ComboBoxItem)comboBoxType.SelectedItem).Tag.ToString();
            string entryId = textBoxEntryId.Text.ToString();
            inheritLastId = entryId;
            inheritLastST = (int)((ComboBoxItem)comboBoxType.SelectedItem).Tag;
            #endregion
            if (entryId != "0")
            {
                if (type != null && entryId != null)
                {
                    try
                    {
                        string sql = $"SELECT * FROM world.smart_scripts WHERE entryorguid = {entryId} AND source_type = {type}";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Dictionary<string, object> data = new Dictionary<string, object>();
                                foreach (string name in smartScriptsColumnNames) { data.Add(name, rdr[name]); }
                                //dataGrid.Items.Add(new DataGridItem() { entry = Convert.ToInt32(entryId), id = Convert.ToInt32(data["id"]), comment = data["comment"].ToString(), source = data });
                                dataGridItemsSource.Add(new DataGridItem() { entry = Convert.ToInt32(entryId), id = Convert.ToInt32(data["id"]), comment = data["comment"].ToString(), source = data });
                            }
                        }
                        else
                        {
                            MessageBox.Show("Smart_scripts для данного EntryID не найдены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        rdr.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
        }
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            //this.source = ((DataGridItem)dataGrid.SelectedItem).source;
            try
            {
                this.source = ((DataGridItem)dataGrid.SelectedCells[0].Item).source;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            #region Активация события
            EventArgs args = new EventArgs();
            СhoiceIsDone(this, args);
            #endregion
        }
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
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
            public int entry { get; set; }
            public int id { get; set; }
            public string comment { get; set; }
            public Dictionary<string, object> source { get; set; }
        }
        private void TextBoxId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string inputText = e.Text;
            if (tb.Text.Contains("-")) { tb.Text = null; }
            e.Handled = !IsTextAllowed(inputText);
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        #endregion
    }
}
