using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DarkWowSoft
{

    public partial class CreatureTextWindow : Window
    {
        #region Свойства
        #region События
        //public delegate void СhoiceDone(object sender, EventArgs e);
        //public event СhoiceDone СhoiceIsDone;
        //public delegate void ClosedButton(object sender, EventArgs e);
        //public event ClosedButton closedButton;
        #endregion
        sqlBox sqlBox { get; set; }
        private MySqlConnection conn { get; set; }
        ObservableCollection<DataGridItem> dataGridItemsSource { get; set; }
        string[] tableNames = new string[13] { "CreatureID", "GroupID", "ID", "Text", "Type", "Language", "Probability", "Emote", "Duration", "Sound", "BroadcastTextid", "TextRange", "comment" };
        #endregion
        #region Конструктор
        public CreatureTextWindow(in MySqlConnection conn, string entryId = null)
        {
            InitializeComponent();
            #region Свойства класса
            this.conn = conn;
            dataGridItemsSource = new ObservableCollection<DataGridItem>();
            #endregion
            #region Сетка данных
            dataGrid.ItemsSource = dataGridItemsSource;
            dataGrid.CanUserAddRows = false;
            dataGrid.CanUserDeleteRows = false;
            #endregion
            #region Присвоение значений "по умолчанию"
            if (entryId != null) { this.entryId.Text = entryId; }
            #endregion
            #region Подписка на события
            search.Click += Search_Click;
            generateSQL.Click += GenerateSQL_Click;
            dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            this.SizeChanged += CreatureTextWindow_SizeChanged;
            #endregion
            #region Автообновление значения
            ButtonAutomationPeer peer = new ButtonAutomationPeer(search);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
            #endregion
        }



        #endregion
        #region Реализация событий
        private void CreatureTextWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = 0;
            for (int i = 0; i < 12; i++)
            {
                width += dataGrid.Columns[i].ActualWidth;
            }
            if (dataGrid.ActualWidth > width)
                dataGrid.Columns[12].Width = dataGrid.ActualWidth - width;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGridInner = sender as DataGrid;
            if (dataGridInner != null)
            {
                DataGridItem selectionItem = dataGridInner.SelectedItem as DataGridItem;
                if (selectionItem != null)
                {
                    creatureIdtext.Text = selectionItem.CreatureID;
                    groupIdtext.Text = Convert.ToString(selectionItem.GroupID);
                    idText.Text = Convert.ToString(selectionItem.ID);
                    textText.Text = selectionItem.Text;
                    typeText.Text = Convert.ToString(selectionItem.Type);
                    languageText.Text = Convert.ToString(selectionItem.Language);
                    probabilityText.Text = Convert.ToString(selectionItem.Probalility);
                    emoteText.Text = Convert.ToString(selectionItem.Emote);
                    durationText.Text = Convert.ToString(selectionItem.Duration);
                    soundText.Text = Convert.ToString(selectionItem.Sound);
                    broadcasttextidText.Text = Convert.ToString(selectionItem.BroadcastTextid);
                    textrangeText.Text = Convert.ToString(selectionItem.TextRange);
                    commentText.Text = selectionItem.comment;
                }
            }
        }
        private void GenerateSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Шаблоны регулярных выражений
                Regex pattern_1 = new Regex("[\"]");
                Regex pattern_2 = new Regex("[\']");
                #endregion
                string sql = $"SET @ENTRY := {Convert.ToInt32(entryId.Text)};\n" +
                             $"DELETE FROM `creature_text` WHERE `creatureID`=@ENTRY;\n" +
                             $"INSERT INTO `creature_text` ({tableNames.Aggregate((x, y) => x + ", " + y)}) VALUES\n";

                string sql_second = "";
                foreach (DataGridItem dataGridItem in dataGrid.Items)
                {
                    #region Для строки
                    string itemInfo = "";
                    int len = tableNames.Length;
                    for (int i = 1; i < len; i++)
                    {
                        try
                        {
                            if (i == tableNames.Length - 1)
                            {
                                itemInfo += "\'" + dataGridItem.source[tableNames[i]] + "\'";
                            }
                            else if (i == 3)
                            {
                                string str = dataGridItem.source[tableNames[i]] as string;
                                string str_1 = pattern_1.Replace(str, "\\\"");
                                string str_2 = pattern_2.Replace(str_1, "\\\'");
                                itemInfo += "\'" + str_2 + "\',";
                            }
                            else
                            {
                                itemInfo += dataGridItem.source[tableNames[i]] + ",";
                            }
                        }
                        catch { itemInfo += 0; }
                    }
                    #endregion
                    #region Для элемента
                    sql_second += $"(@ENTRY," +
                        $"{itemInfo}" + (dataGrid.Items.IndexOf(dataGridItem) == dataGrid.Items.Count - 1 ? ");" : "),\n");
                    #endregion
                }

                //Regex pattern = new Regex("[\"]|[\']");
                //string sql_secondReplaced = pattern.Replace(sql_second, "\\\"");

                sqlBox = new sqlBox(sql + sql_second);
                this.IsEnabled = false;
                sqlBox.Closed += SqlBox_Closed;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string id = entryId.Text;
            if (id != "0")
            {
                if (id != null)
                {
                    try
                    {
                        dataGridItemsSource.Clear();
                        string sql = $"SELECT * FROM world.creature_text WHERE CreatureID = {id}";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Dictionary<string, object> data = new Dictionary<string, object>();
                                foreach (string name in tableNames) { data.Add(name, rdr[name]); }
                                dataGridItemsSource.Add(new DataGridItem()
                                {
                                    CreatureID = Convert.ToString(data["CreatureID"]),
                                    GroupID = Convert.ToInt32(data["GroupID"]),
                                    ID = Convert.ToInt32(data["ID"]),
                                    Text = Convert.ToString(data["Text"]),
                                    Type = Convert.ToInt32(data["Type"]),
                                    Language = Convert.ToInt32(data["Language"]),
                                    Probalility = Convert.ToInt32(data["Probability"]),
                                    Emote = Convert.ToInt32(data["Emote"]),
                                    Duration = Convert.ToInt32(data["Duration"]),
                                    Sound = Convert.ToInt32(data["Sound"]),
                                    BroadcastTextid = Convert.ToInt32(data["BroadcastTextid"]),
                                    TextRange = Convert.ToInt32(data["TextRange"]),
                                    comment = Convert.ToString(data["comment"]),
                                    source = data
                                });
                            }
                            #region Выравнивание ширины колонок DataGrid
                            dataGrid.Columns[0].Width = 75;
                            dataGrid.Columns[1].Width = 60;
                            dataGrid.Columns[2].Width = 35;
                            dataGrid.Columns[3].Width = 300;
                            dataGrid.Columns[4].Width = 35;
                            dataGrid.Columns[5].Width = 65;
                            dataGrid.Columns[6].Width = 67;
                            dataGrid.Columns[7].Width = 55;
                            dataGrid.Columns[8].Width = 60;
                            dataGrid.Columns[9].Width = 55;
                            dataGrid.Columns[10].Width = 100;
                            dataGrid.Columns[11].Width = 67;
                            dataGrid.Columns[12].Width = 240;
                            #endregion
                        }
                        else
                        {
                            MessageBox.Show("Creature_text для данного EntryID не найдены!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information); rdr.Close();
                        }
                        rdr.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
            #region Скрытие ласт столбика
            dataGrid.Columns[13].Visibility = Visibility.Hidden;
            #endregion
            #region Корректировка размера окна
            this.Width += 1; this.Width -= 1;
            #endregion
        }
        private void SqlBox_Closed(object sender, EventArgs e)
        {
            sqlBox window = sender as DarkWowSoft.sqlBox;
            sqlBox = null;
            this.IsEnabled = true;
            window.Close();
        }
        private void creatureTextprimenit_Click(object sender, RoutedEventArgs e)
        {
            DataGridItem selectionItem = dataGrid.SelectedItem as DataGridItem;
            if (selectionItem != null)
            {
                #region Обновление элемента
                selectionItem.CreatureID = creatureIdtext.Text; selectionItem.source["CreatureID"] = creatureIdtext.Text;
                selectionItem.GroupID = Convert.ToInt32(groupIdtext.Text); selectionItem.source["GroupID"] = groupIdtext.Text;
                selectionItem.ID = Convert.ToInt32(idText.Text); selectionItem.source["ID"] = idText.Text;
                selectionItem.Text = textText.Text; selectionItem.source["Text"] = textText.Text;
                selectionItem.Type = Convert.ToInt32(typeText.Text); selectionItem.source["Type"] = typeText.Text;
                selectionItem.Language = Convert.ToInt32(languageText.Text); selectionItem.source["Language"] = languageText.Text;
                selectionItem.Probalility = Convert.ToInt32(probabilityText.Text); selectionItem.source["Probability"] = probabilityText.Text;
                selectionItem.Emote = Convert.ToInt32(emoteText.Text); selectionItem.source["Emote"] = emoteText.Text;
                selectionItem.Duration = Convert.ToInt32(durationText.Text); selectionItem.source["Duration"] = durationText.Text;
                selectionItem.Sound = Convert.ToInt32(soundText.Text); selectionItem.source["Sound"] = soundText.Text;
                selectionItem.BroadcastTextid = Convert.ToInt32(broadcasttextidText.Text); selectionItem.source["BroadcastTextid"] = broadcasttextidText.Text;
                selectionItem.TextRange = Convert.ToInt32(textrangeText.Text); selectionItem.source["TextRange"] = textrangeText.Text;
                selectionItem.comment = commentText.Text; selectionItem.source["comment"] = commentText.Text;
                #endregion
                #region Обновление DataSource
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = dataGridItemsSource;
                #region Выравнивание ширины колонок DataGrid
                dataGrid.Columns[0].Width = 75;
                dataGrid.Columns[1].Width = 60;
                dataGrid.Columns[2].Width = 35;
                dataGrid.Columns[3].Width = 300;
                dataGrid.Columns[4].Width = 35;
                dataGrid.Columns[5].Width = 65;
                dataGrid.Columns[6].Width = 67;
                dataGrid.Columns[7].Width = 55;
                dataGrid.Columns[8].Width = 60;
                dataGrid.Columns[9].Width = 55;
                dataGrid.Columns[10].Width = 100;
                dataGrid.Columns[11].Width = 67;
                dataGrid.Columns[12].Width = 240;
                #endregion
                #endregion
            }
        }

        private void creatureTextnew_Click(object sender, RoutedEventArgs e)
        {
            #region Получение следующего ID
            //int[] dataGridNumbers = new int[dataGrid.Items.Count];
            int[] dataGridGroupNumbers = new int[dataGrid.Items.Count];
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                //dataGridNumbers[i] = ((DataGridItem)dataGrid.Items[i]).ID;
                dataGridGroupNumbers[i] = ((DataGridItem)dataGrid.Items[i]).GroupID;
            }
            int newGroupNumber = MainWindow.getMissingNo(dataGridGroupNumbers);
            //int newNumber = MainWindow.getMissingNo(dataGridNumbers);
            #endregion
            #region Создание нового элемента
            DataGridItem dataGridItem = new DataGridItem()
            {
                CreatureID = entryId.Text,
                ID = 0,
                GroupID = newGroupNumber,
                Text = $"New Text({newGroupNumber})",
                Type = 0,
                Language = 0,
                Probalility = 0,
                Emote = 0,
                Duration = 0,
                Sound = 0,
                BroadcastTextid = 0,
                TextRange = 0,
                source = new Dictionary<string, object>(),
            };
            #endregion
            #region Обновление элемента
            dataGridItem.source["CreatureID"] = dataGridItem.CreatureID;
            dataGridItem.source["GroupID"] = dataGridItem.GroupID;
            dataGridItem.source["ID"] = dataGridItem.ID;
            dataGridItem.source["Text"] = dataGridItem.Text;
            dataGridItem.source["Type"] = dataGridItem.Type;
            dataGridItem.source["Language"] = dataGridItem.Language;
            dataGridItem.source["Probability"] = dataGridItem.Probalility;
            dataGridItem.source["Emote"] = dataGridItem.Emote;
            dataGridItem.source["Duration"] = dataGridItem.Duration;
            dataGridItem.source["Sound"] = dataGridItem.Sound;
            dataGridItem.source["BroadcastTextid"] = dataGridItem.BroadcastTextid;
            dataGridItem.source["TextRange"] = dataGridItem.TextRange;
            dataGridItem.source["comment"] = dataGridItem.comment;
            #endregion
            #region Обновление DataSource
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = dataGridItemsSource;
            #endregion
            #region Добавление нового элемента
            dataGridItemsSource.Add(dataGridItem);
            #endregion
            #region Выбор добавленного элемента
            dataGrid.SelectedIndex = dataGridItemsSource.IndexOf(dataGridItem);
            #endregion
            #region Скрытие ласт столбика
            dataGrid.Columns[13].Visibility = Visibility.Hidden;
            #endregion
            #region Корректировка размера окна
            this.Width += 1; this.Width -= 1;
            #endregion
            #region Выравнивание ширины колонок DataGrid
            dataGrid.Columns[0].Width = 75;
            dataGrid.Columns[1].Width = 60;
            dataGrid.Columns[2].Width = 35;
            dataGrid.Columns[3].Width = 300;
            dataGrid.Columns[4].Width = 35;
            dataGrid.Columns[5].Width = 65;
            dataGrid.Columns[6].Width = 67;
            dataGrid.Columns[7].Width = 55;
            dataGrid.Columns[8].Width = 60;
            dataGrid.Columns[9].Width = 55;
            dataGrid.Columns[10].Width = 100;
            dataGrid.Columns[11].Width = 67;
            dataGrid.Columns[12].Width = 240;
            #endregion

        }

        private void creatureTextcopy_Click(object sender, RoutedEventArgs e)
        {
            DataGridItem selectedItem = dataGrid.SelectedItem as DataGridItem;
            #region Получение номера недостающей GroupID
            int newIDNumber = 0;
            if (selectedItem != null)
            {
                int[] groupItemsIDs = dataGridItemsSource.ToList().FindAll(x => x.GroupID == selectedItem.GroupID).Select(x => x.ID).ToArray();
                if (groupItemsIDs.Length > 0)
                {
                    newIDNumber = MainWindow.getMissingNo(groupItemsIDs);
                }
                #region Добавление нового элемента
                DataGridItem dataGridItem = new DataGridItem()
                {
                    CreatureID = selectedItem.CreatureID,
                    ID = newIDNumber,
                    GroupID = selectedItem.GroupID,
                    Text = selectedItem.Text,
                    Type = selectedItem.Type,
                    Language = selectedItem.Language,
                    Probalility = selectedItem.Probalility,
                    Emote = selectedItem.Emote,
                    Duration = selectedItem.Duration,
                    Sound = selectedItem.Sound,
                    BroadcastTextid = selectedItem.BroadcastTextid,
                    TextRange = selectedItem.TextRange,
                    comment = selectedItem.comment,
                    source = new Dictionary<string, object>(),

                };

                #region Обновление элемента
                dataGridItem.source["CreatureID"] = dataGridItem.CreatureID;
                dataGridItem.source["GroupID"] = dataGridItem.GroupID;
                dataGridItem.source["ID"] = dataGridItem.ID;
                dataGridItem.source["Text"] = dataGridItem.Text;
                dataGridItem.source["Type"] = dataGridItem.Type;
                dataGridItem.source["Language"] = dataGridItem.Language;
                dataGridItem.source["Probability"] = dataGridItem.Probalility;
                dataGridItem.source["Emote"] = dataGridItem.Emote;
                dataGridItem.source["Duration"] = dataGridItem.Duration;
                dataGridItem.source["Sound"] = dataGridItem.Sound;
                dataGridItem.source["BroadcastTextid"] = dataGridItem.BroadcastTextid;
                dataGridItem.source["TextRange"] = dataGridItem.TextRange;
                dataGridItem.source["comment"] = dataGridItem.comment;
                #endregion
                #region Обновление DataSource
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = dataGridItemsSource;
                #endregion

                dataGridItemsSource.Add(dataGridItem);
                #endregion

                #region Выбор скопированного элемента
                dataGrid.SelectedIndex = dataGridItemsSource.IndexOf(dataGridItem);
                #endregion
                #region Скрытие ласт столбика
                dataGrid.Columns[13].Visibility = Visibility.Hidden;
                #endregion
                #region Корректировка размера окна
                this.Width += 1; this.Width -= 1;
                #endregion
                #region Выравнивание ширины колонок DataGrid
                dataGrid.Columns[0].Width = 75;
                dataGrid.Columns[1].Width = 60;
                dataGrid.Columns[2].Width = 35;
                dataGrid.Columns[3].Width = 300;
                dataGrid.Columns[4].Width = 35;
                dataGrid.Columns[5].Width = 65;
                dataGrid.Columns[6].Width = 67;
                dataGrid.Columns[7].Width = 55;
                dataGrid.Columns[8].Width = 60;
                dataGrid.Columns[9].Width = 55;
                dataGrid.Columns[10].Width = 100;
                dataGrid.Columns[11].Width = 67;
                dataGrid.Columns[12].Width = 240;
                #endregion


            }
            #endregion

        }

        private void creatureTextdelete_Click(object sender, RoutedEventArgs e)
        {
            DataGridItem selectedItem = dataGrid.SelectedItem as DataGridItem;
            dataGridItemsSource.Remove(selectedItem);
        }

        private void creatureTextsave_Click(object sender, RoutedEventArgs e)
        {

            //try
            //{
                #region Шаблоны регулярных выражений
                Regex pattern_1 = new Regex("[\"]");
                Regex pattern_2 = new Regex("[\']");
                #endregion
                int selectionIndex = dataGrid.SelectedIndex;
                //selIndex = selectionIndex;
                #region Запрос
                string sql_delete = $"DELETE FROM `creature_text` WHERE `creatureID`= {entryId.Text}";
                MySqlDataAdapter adapter_delete = new MySqlDataAdapter()
                {
                    InsertCommand = new MySqlCommand(sql_delete, conn),
                };
                adapter_delete.InsertCommand.ExecuteNonQuery();
                #endregion
                string sql_insert_1 = $"INSERT INTO `creature_text` ({tableNames.Aggregate((x, y) => x + ", " + y)}) VALUES\n";
                string sql_insert_2 = "";

                foreach (DataGridItem dataGridItem in dataGrid.Items)
                {
                    #region Для строки
                    string itemInfo = "";
                    int len = tableNames.Length;
                    for (int i = 1; i < len; i++)
                    {
                        try
                        {
                            if (i == tableNames.Length - 1)
                            {
                                itemInfo += "\'" + dataGridItem.source[tableNames[i]] + "\'";
                            }
                            else if (i == 3)
                            {
                                string str = dataGridItem.source[tableNames[i]] as string;
                            string str_1 = pattern_1.Replace(str, "\\\"");
                            string str_2 = pattern_2.Replace(str_1, "\\\'");
                            itemInfo += "\'" + str_2 + "\',";
                            //itemInfo = str;
                            }
                            else
                            {
                                itemInfo += Convert.ToString(dataGridItem.source[tableNames[i]]) + ",";
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); itemInfo += "0,"; }
                    }
                    #endregion
                    #region Для элемента
                    sql_insert_2 += $"({entryId.Text}," +
                        $"{itemInfo}" + (dataGrid.Items.IndexOf(dataGridItem) == dataGrid.Items.Count - 1 ? ");" : "),\n");
                    #endregion
                }

                MySqlDataAdapter adapter_insert = new MySqlDataAdapter()
                {
                    InsertCommand = new MySqlCommand(sql_insert_1 + sql_insert_2, conn),
                };
                adapter_insert.InsertCommand.ExecuteNonQuery();

                //ButtonAutomationPeer peer = new ButtonAutomationPeer(buttonRefreshEntryId);
                //IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                //invokeProv.Invoke();
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message + "\n\n" + ex.Source + "\n\n" + ex.HelpLink + "\n\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }


        }

        #endregion
        #region Вспомогательные классы
        private class DataGridItem
        {
            public string CreatureID { get; set; }
            public int GroupID { get; set; }
            public int ID { get; set; }
            public string Text { get; set; }
            public int Type { get; set; }
            public int Language { get; set; }
            public int Probalility { get; set; }
            public int Emote { get; set; }
            public int Duration { get; set; }
            public int Sound { get; set; }
            public int BroadcastTextid { get; set; }
            public int TextRange { get; set; }
            public string comment { get; set; }
            public Dictionary<string, object> source { get; set; }
        }
        #endregion


    }
}
