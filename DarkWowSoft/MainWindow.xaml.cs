#region Библотеки
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Windows.Automation.Provider;
using System.Data;
#endregion
namespace DarkWowSoft
{
    /// <summary>
    /// Основное окно
    /// </summary>
  
    public partial class MainWindow : Window
    {
        #region Элементы интерфейса (внешний контекст)
        #region Фон и картинки
        System.Windows.Shapes.Rectangle background; // Фон окна
        BitmapImage image = null;
        #endregion
        #region Текстовые поля
        TextBox textBoxEntryId = null;
        TextBox textBoxId = null;
        TextBox textBoxSelectLinkIndex = null;
        TextBox textBoxEvent = null;
        TextBox textBoxMask = null;
        TextBox textBoxChance = null;
        TextBox textBoxEventFlag = null;
        TextBox textBoxAction = null;
        TextBox textBoxTarget = null;
        TextBox textBoxComment = null;
        TextBox textBoxSelectLinkPreserved = null;
        TextBox[] textBoxes = new TextBox[4];
        PasswordBox password = null;
        #endregion
        #region "Последовательная панель"
        StackPanel[] EAT = new StackPanel[3];
        #endregion
        #region Свитки
        ComboBox comboBoxType = null;
        ComboBox comboBoxEvent = null;
        ComboBox comboBoxAction = null;
        ComboBox comboBoxTarget = null;
        #endregion
        #region Текст
        TextBlock textDGName = null;
        TextBlock textDGNameLocal = null;
        TextBlock textDGSmartAI = null;
        TextBlock textBlockComment = null;
        #endregion
        #region Передаточные панели
        WrapPanel wrapPanelMain = null;
        WrapPanel wrapPanelDataGrid = null;
        WrapPanel wrapPanelEditor = null;
        WrapPanel wrapPanelParameters = null;
        WrapPanel wrapPanelScriptDescription = null;
        #endregion
        #region Кнопки
        Button buttonSQL = null;
        Button buttonCreate = null;
        Button buttonSwitchSmart = null;
        Button inheritBehaviorButton = null;
        Button linkButton = null;
        Button buttonCopy = null;
        Button buttonNew = null;
        Button buttonDelete = null;
        Button buttonSave = null;
        Button buttonUpdateTempBase = null;
        Button buttonRefreshEntryId = null;
        Button creatureTextButton = null;
        #endregion
        #region Сетка данных
        System.Windows.Controls.DataGrid dataGrid = null;
        DataGridTextColumn commentColumn = null;
        #endregion
        #region Чек бокс
        CheckBox checkBoxId = null;
        #endregion
        #region Дополнительные окна
        linkWindow linkWindow = null;
        phaseWindow phaseWindow = null;
        flagWindow flagWindow = null;
        sqlBox sqlBox = null;
        inheritWindow inheritWindow = null;
        List<(int, actionWindow)> actionWindow = new List<(int, actionWindow)>();
        List<(int, actionWindow)> eventWindow = new List<(int, actionWindow)>();
        CreatureTextWindow creatureTextWindow = null;
        #endregion
        #region Ползунок
        Slider chanceSlider = null;
        #endregion
        #region Контроллер вкладок
        TabControl tabControl = null;
        #endregion
        #region Массивы данных
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
        (string, int)[] phases = new (string, int)[10]
        { ("PHASE_ALWAYS",0), ("PHASE_1",1),
        ("PHASE_2",2),("PHASE_3",4),
        ("PHASE_4",8),("PHASE_5",16),
        ("PHASE_6",32),("PHASE_7",64),
        ("PHASE_8",128),("PHASE_9",256) };
        (string, int)[] flags = new (string, int)[11]
        { ("NONE",0), ("NOT_REPEATABLE",1),
        ("DIFFICULTY_0_NORMAL_DUNGEON",2), ("DIFFICULTY_1_HEROIC_DUNGEON",4),
        ("DIFFICULTY_2_NORMAL_RAID",8), ("DIFFICULTY_3_HEROIC_RAID",16),
        ("RESERVED_5",32), ("RESERVED_6",64),
        ("DEBUG_ONLY",128), ("DONT_RESET",256),
        ("WHILE_CHARMED",512) };
        (int, string[])[] eventArray = new (int, string[])[83]
        { (-1, new string[5] { "InitialMin", "InitialMax", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "InitialMin", "InitialMax", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "HPMin%", "HPMax%", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "ManaMin%", "ManaMax%", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "CooldownMin", "CooldownMax", "Player only (0/1)", "Creature entry (if param3 is 0)", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (2, new string[5] { "SpellID", "School", "CooldownMin", "CooldownMax", null } ),
        (-1, new string[5] { "MinDist", "MaxDist", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "No Hostile (0/1)", "MaxRange", "CooldownMin", "CooldownMax", "Player Only 0/1" } ),
        (-1, new string[5] { "Type (None= 0, Map = 1, Area = 2)", "Map ID", "Zone ID", null, null } ),
        (-1, new string[5] { "HPMin%", "HPMax%", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "RepeatMin", "RepeatMax", "Spell ID (0 any)", null, null } ),
        (-1, new string[5] { "HP Deficit", "Radius", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "Radius", "RepeatMin", "RepeatMax", null, null } ),
        (-1, new string[5] { "Spell ID", "Radius", "RepeatMin", "RepeatMin", null } ),
        (-1, new string[5] { "Creature ID (0 any)", "CooldownMin", "CooldownMax", null, null } ),
        (-1, new string[5] { "ManaMin%", "ManaMax%", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "QuestID (0 any)", "RepeatMin", "RepeatMax", null, null } ),
        (-1, new string[5] { "QuestID (0 any)", "RepeatMin", "RepeatMax", null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Emote ID", "CooldownMin", "CooldownMax", null, null } ),
        (-1, new string[5] { "Spell ID", "Stacks", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "Spell ID", "Stacks", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "NoHostile", "MaxRange", "CooldownMin", "CooldownMax", "Player Only 0/1" } ),
        (-1, new string[5] { "CooldownMin", "CooldownMax", null, null, null } ),
        (-1, new string[5] { "CooldownMin", "CooldownMax", null, null, null } ),
        (-1, new string[5] { "0(on charm apply)/1(on charm remove)", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (2, new string[5] { "Spell ID", "School", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "MinDmg", "MaxDmg", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { "MinDmg", "MaxDmg", "RepeatMin", "RepeatMax", null } ),
        (1, new string[5] { "MovementType (0 any)", "PointID", null, null, null } ),
        (-1, new string[5] { "Entry", "CooldownMin", "CooldownMax", null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] {  "Field", "Value", "CooldownMin", "CooldownMax", null  } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Entry (0 any)", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Point ID", null, null, null, null } ),
        (-1, new string[5] { "Team (0 any)", "CooldownMin", "CooldownMax", null, null } ),
        (-1, new string[5] { "Trigger ID (0 any)", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Group ID (from creature_text)", "Creature ID (0 any)", null, null, null } ),
        (-1, new string[5] { "MinHeal", "MaxHeal", "CooldownMin", "CooldownMax", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { "Point ID (0 any)", "PathId (0 any)", null, null, null } ),
        (-1, new string[5] { "Id", null, null, null, null } ),
        (-1, new string[5] { "InitialMin", "InitialMax", "RepeatMin", "RepeatMax", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Menu ID", "ID", null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "0/1/2", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Event phase mask", null, null, null, null } ),
        (-1, new string[5] { "CooldownMin", "CooldownMax", null, null, null } ),
        (-1, new string[5] { "game_event.event Entry", null, null, null, null } ),
        (-1, new string[5] { "game_event.event Entry", null, null, null, null } ),
        (-1, new string[5] { "State(0-NotReady,1-Ready,2-Activacted,3-JustDeactivated)", null, null, null, null } ),
        (-1, new string[5] { "EventId", null, null, null, null } ),
        (-1, new string[5] { "EventId", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "MinHP%", "MaxHP%", "RepeatMin", "RepeatMax", "Radius" } ),
        (-1, new string[5] { "Database GUID", "Database ENTRY", "Distance", "Repeat interval(ms)", null } ),
        (-1, new string[5] { "Database GUID", "Database ENTRY", "Distance", "Repeat interval(ms)", null } ),
        (-1, new string[5] { "CounterID", "Value", "CooldownMin", "CooldownMax", null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { "Param_string: triggerName", null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ),
        (-1, new string[5] { null, null, null, null, null } ) };
        (int, string[])[] actionArray = new (int, string[])[147]
        { (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "Creature_text.groupid", "Wait before SMART_EVENT_TEXT_OVER", "0 talk of the target/1 target as talk target" , null, null, null } ),
        (-1, new string[6] { "Faction ID", null, null , null, null, null } ),
        (-1, new string[6] { "Creature_template.entry", "Creature_template.modelID", null , null, null, null } ),
        (-1, new string[6] { "Sound ID", "OnlySelf (0/1)", "Distant Sound (0/1)" , null, null, null } ),
        (-1, new string[6] { "Emote ID", null, null , null, null, null } ),
        (-1, new string[6] { "Quest ID", null, null , null, null, null } ),
        (-1, new string[6] { "Quest ID", "DirectAdd (0/1)", null , null, null, null } ),
        (1, new string[6] { "State", null , null, null, null, null } ),
        (-1, new string[6] {  null,  null, null , null, null, null } ),
        (-1, new string[6] { "Emote ID-1", "Emote ID-2", "Emote ID-3" , "Emote ID-4", "Emote ID-5", "Emote ID-6" } ),
        (23, new string[6] { "Spell ID", "CastFlags", "TriggeredFlags" , null, null, null } ),
        (2, new string[6] { "Creature_template.entry", "Summon type", "Duration (in ms)" , "Attack Invoker (0/1)", "SmartActionSummonCreatureFlags (1/2)", null } ),
        (-1, new string[6] { "Threat% +", "Threat% -", null , null, null, null } ),
        (-1, new string[6] { "Threat% +", "Threat% -", null , null, null, null } ),
        (-1, new string[6] { "Quest ID", null, null , null, null, null } ),
        (-1, new string[6] { "Phase ID", "Apply/Remove (1/0)", null , null, null, null } ),
        (-1, new string[6] { "Emote ID", null, null , null, null, null } ),
        (-1, new string[6] { "Unit_flags", "False(UnitFlag1)/True(UnitFlag2)", null , null, null, null } ),
        (-1, new string[6] { "Unit_flags", "False(UnitFlag1)/True(UnitFlag2)", null , null, null, null } ),
        (-1, new string[6] { "Allow Attack State (0/1)", null, null , null, null, null } ),
        (-1, new string[6] { "Allow Combat Movement (0/1)", null, null , null, null, null } ),
        (-1, new string[6] { "smart_scripts.event_phase_mask", null, null , null, null, null } ),
        (-1, new string[6] { "Increment +", "Decrement -", null , null, null, null } ),
        (-1, new string[6] { null, null, null , null, null, null } ),
        (-1, new string[6] { "Flee Text Emote 0/1", null, null , null, null, null } ),
        (-1, new string[6] { "Quest ID", null, null , null, null, null } ),
        (-1, new string[6] { null, null, null , null, null, null } ),
        (-1, new string[6] { "Spell ID", "Only Owned Auras 0/1", null , null, null, null } ),
        (-1, new string[6] { "Distance (0 = Default value)", "Angle (0 = Default value)", "End creature_template.entry" , "Credit", "CreditType(0monsterkill,1event)", null } ),
        (-1, new string[6] { "Phase_mask 1", "Phase_mask 2", "Phase_mask 3" , "Phase_mask 4", "Phase_mask 5", "Phase_mask 6" } ),
        (-1, new string[6] { "Phase_mask minimum", "Phase_mask maximum", null , null, null, null } ),
        (-1, new string[6] { null, null, null , null, null, null } ),
        (-1, new string[6] { "Creature_template.entry", null, null , null, null, null } ),
        (-1, new string[6] { "Field", "Data", "Type(0=SetData,1=SetBossState)" , null, null, null } ),
        (-1, new string[6] { "Field", null, null, null, null, null } ),
        (-1, new string[6] { "Creature_template.entry", "Update Level", null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "Radius", "Call for help Text Emote(0/1)", null, null, null, null } ),
        (-1, new string[6] { "Sheath(0-unarmed,1-melee,2-ranged)", null, null, null, null, null } ),
        (-1, new string[6] { "Despawn timer ms", "Respawn timer sec", null, null, null, null } ),
        (-1, new string[6] { "HP value number", "HP value %", null, null, null, null } ),
        (-1, new string[6] { "creature_template.entry", "creature_template.modelID", null, null, null, null } ),
        (-1, new string[6] { "creature.phasemask", null, null, null, null, null } ),
        (-1, new string[6] { "Field", "Data", null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "0/1", null, null, null, null, null } ),
        (-1, new string[6] { "0/1", null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "gameobject_template.entry", "Despawn time in seconds", "0-despawn when summoner despawn/1-despawn when time runs out", null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "Taxi ID", null, null, null, null, null } ),
        (6, new string[6] { "0 = walk / 1 = run", "waypoints.entry", "Can Repeat (0/1)", "quest_template.id", "Despawn time", "React State" } ),
        (-1, new string[6] { "Time (in ms)", null, null, null, null, null } ),
        (-1, new string[6] { "Despawn Time", "quest_template.id", "Fail (0/1)", null, null, null } ),
        (-1, new string[6] { "item_template.entry", "count", null, null, null, null } ),
        (-1, new string[6] { "item_template.entry", "count", null, null, null, null } ),
        (1, new string[6] { "TemplateAI-ID", "(ID-1 or 2)Spellid", "(ID-1 or 2)RepeatMin", "(ID-1 or 2)RepeatMax", "(ID-1 or 2)Range", "(ID-1 or 2)Mana%" } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ),
        (-1, new string[6] { "0 = gravity On / 1 = gravity Off", null, null, null, null, null } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ),
        (-1, new string[6] { "Map ID", null, null, null, null, null } ),
        (-1, new string[6] { "Counter ID", "Value", "Reset (0/1)", null, null, null } ),
        (-1, new string[6] { "varID", null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "0=North,West=1.5,South=3,East=4.5", null, null, null, null, null } ),
        (-1, new string[6] { "ID", "InitialMin", "InitialMax", "RepeatMin(only if it repeats)", "RepeatMax(only if it repeats)", "Chance" } ),
        (-1, new string[6] { "Entry", null, null, null, null, null } ),
        (-1, new string[6] { "Point ID", "Is Transport (0/1)", "Disable Pathfinding (0/1)", "Contact Distance", null, null } ),
        (-1, new string[6] { "Respawntime in seconds", null, null, null, null, null } ),
        (-1, new string[6] { "creature_equip_template.CreatureID", "Slotmask", "Slot1 (item_template.entry)", "Slot2 (item_template.entry)", "Slot3 (item_template.entry)", null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "id(>1)", null, null, null, null, null } ),
        (-1, new string[6] { "id(>1)", null, null, null, null, null } ),
        (-1, new string[6] { "Spell ID", null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "Attack Distance", "Attack Angle", null, null, null, null } ),
        (-1, new string[6] { "Timed ActionList ID", "0-OOC, 1-IC, 2-ALWAYS", "Can stop an ActionList and start another(0/1)", null, null, null } ),
        (-1, new string[6] { "Creature_template.npcflag", null, null, null, null, null } ),
        (-1, new string[6] { "Creature_template.npcflag", null, null, null, null, null } ),
        (-1, new string[6] { "Creature_template.npcflag", null, null, null, null, null } ),
        (-1, new string[6] { "Creature_text.groupid", null, null, null, null, null } ),
        (23, new string[6] { "Spell ID", "CastFlags", "TriggeredFlags", null, null, null } ),
        (2, new string[6] { "Spell ID", "CastFlags", "CasterTargetType", "CasterTarget(target_param1)", "CasterTarget target_param2)", "CasterTarget(target_param3)" } ),
        (-1, new string[6] { "Timed ActionList ID-1", "Timed ActionList ID-2", "Timed ActionList ID-3", "Timed ActionList ID-4", "Timed ActionList ID-5", "Timed ActionList ID-6" } ),
        (-1, new string[6] { "Timed ActionList ID-Min", "Timed ActionList ID-Max", null, null, null, null } ),
        (-1, new string[6] { "Radius", null, null, null, null, null } ),
        (-1, new string[6] { "Value", "Type", null, null, null, null } ),
        (-1, new string[6] { "Value", "Type", null, null, null, null } ),
        (-1, new string[6] { "With delay (0/1)", "Spell ID", "Instant (0/1)", null, null, null } ),
        (-1, new string[6] { "Animprogress (0-255)", null, null, null, null, null } ),
        (-1, new string[6] { "creature.dynamicflags", null, null, null, null, null } ),
        (-1, new string[6] { "creature.dynamicflags", null, null, null, null, null } ),
        (-1, new string[6] { "creature.dynamicflags", null, null, null, null, null } ),
        (-1, new string[6] { "Speed XY", "Speed Z", null, null, null, null } ),
        (-1, new string[6] { "gossip_menu.entry", "gossip_menu.text_id(npc_text.ID)", null, null, null, null } ),
        (-1, new string[6] { "0-Not ready,1-Ready,2-Activated,3-Just deactivated", null, null, null, null, null } ),
        (-1, new string[6] { "ID", null, null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "0/1", null, null, null, null, null } ),
        (-1, new string[6] { "0/1", null, null, null, null, null } ),
        (-1, new string[6] { "gameobject_template_addon.flags", null, null, null, null, null } ),
        (-1, new string[6] { "gameobject_template_addon.flags", null, null, null, null, null } ),
        (-1, new string[6] { "gameobject_template_addon.flags", null, null, null, null, null } ),
        (-1, new string[6] { "creature_summon_groups.groupId", "Attack invoker (0/1)", null, null, null, null } ),
        (-1, new string[6] { "Power type", "New power", null, null, null, null } ),
        (-1, new string[6] { "Power type", "Power to add", null, null, null, null } ),
        (-1, new string[6] { "Power type", "Power to remove", null, null, null, null } ),
        (-1, new string[6] { "GameEvent ID", null, null, null, null, null } ),
        (-1, new string[6] { "GameEvent ID", null, null, null, null, null } ),
        (-1, new string[6] { "Waypoint 1", "Waypoint 2", "Waypoint 3", "Waypoint 4", "Waypoint 5", "Waypoint 6" } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "Sound ID 1", "Sound ID 2", "Sound ID 3", "Sound ID 4", "OnlySelf (0/1)", "Distant Sound (0/1)" } ),
        (-1, new string[6] { "Timer", null, null, null, null, null } ),
        (-1, new string[6] { "Disable evade (1) / re-enable (0)", null, null, null, null, null } ),
        (-1, new string[6] { "State", null, null, null, null, null } ),
        (-1, new string[6] { "0/1", null, null, null, null, null } ),
        (-1, new string[6] { "Type", null, null, null, null, null } ),
        (-1, new string[6] { "Sight Distance", null, null, null, null, null } ),
        (-1, new string[6] { "Flee Time", null, null, null, null, null } ),
        (-1, new string[6] { "Threat +", "Threat -", null, null, null, null } ),
        (-1, new string[6] { "ID", null, null, null, null, null } ),
        (-1, new string[6] { "Id min range", "Id max range", null, null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "MovementSlot(default = 0,active = 1,controlled = 2)", "PauseTime (ms)", "Forced", null, null, null } ),
        (-1, new string[6] { "AnimKit ID", "Type(0/1/2/3)", null, null, null, null } ),
        (-1, new string[6] { "Scene ID", null, null, null, null, null } ),
        (-1, new string[6] { "Scene ID", null, null, null, null, null } ),
        (-1, new string[6] { "Group ID", "minDelay", "maxDelay", "Spawnflags", null, null } ),
        (-1, new string[6] { "Group ID", "minDelay", "maxDelay", "Spawnflags", null, null } ),
        (-1, new string[6] { "spawnType (0 npc/ 1 gob)", "SpawnId (DB Guid)", "maxDelay", "Spawnflags", null, null } ),
        (23, new string[6] { "Spell ID", "castFlags", "triggeredFlags", null, null, null } ),
        (-1, new string[6] { "Entry", "Cinematic", null, null, null, null } ),
        (-1, new string[6] { "MovementType", "SpeedInteger", "SpeedFraction", null, null, null } ),
        (-1, new string[6] { "SpellVisualKitId", null, null, null, null, null } ),
        (-1, new string[6] { "ZoneId", "AreaLightId", "LightId(overrideLightId)", "FadeInTime(transition Milliseconds)", null, null } ),
        (-1, new string[6] { "ZoneId", "WeatherId", "WeatherGrade (intensity)", null, null, null } ),
        (-1, new string[6] { null, null, null, null, null, null } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ),
        (-1, new string[6] { "HP%", null, null, null, null, null } ),
        (-1, new string[6] { "conversation_template.id", null, null, null, null, null } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ),
        (-1, new string[6] { "0 = Off / 1 = On", null, null, null, null, null } ) };
        string[,] targetArray = new string[31, 8] {
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, "(For Move-Action)Target_X", "(For Move-Action)Target_Y", "(For Move-Action)Target_Z", "(For Move-Action)Target_O"},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, "Target_X", "Target_Y", "Target_Z", "Target_O"},
        { "Creature Entry (0 any)", "MinDist", "MaxDist", "Number of target(0=All)", null, null, null, null},
        { "GUID", "ENTRY", null, null, null, null, null, null},
        { "Creature Entry (0 any)", "MaxDist", "Number of target(0=All)", null, null, null, null, null},
        { "varID", null, null, null, null, null, null, null},
        { "Gameobject Entry (0 any)", "MinDist", "MaxDist", "Number of target(0=All)", null, null, null, null},
        { "GUID", "ENTRY", null, null, null, null, null, null},
        { "Gameobject Entry (0 any)", "MaxDist", "Number of target(0=All)", null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { "MinDist", "MaxDist", null, null, null, null, null, null},
        { "MaxDist", null, null, null, null, null, null, null},
        { "Creature Entry (0 any)", "MaxDist (Can be from 0-100 yards)", "Dead? (0/1)", null, null, null, null, null},
        { "Gameobject Entry (0 any)", "MaxDist (Can be from 0-100 yards)", null, null, null, null, null, null},
        { "MaxDist", null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { "MaxDist", "PlayerOnly (0/1)", null, null, null, null, null, null},
        { "MaxDist", "PlayerOnly (0/1)", null, null, null, null, null, null},
        { null, null, null, null, null, null, null, null},
        { "MaxDist", "PlayerOnly (0/1)", "Is in Los (0/1)", null, null, null, null, null},
        { "Seat", null, null, null, null, null, null, null},
        { "Gameobject Entry (0 any)", "MaxDist (Can be from 0-100 yards)", null, null, null, null, null, null}
        };
        (int, (int, (string, int)[])[])[] actionButtonArray = new (int, (int, (string, int)[])[])[8]
        {
                (11, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                    ("NORMAL", 0),  ("INTERRUPT_PREVIOUS", 1), ("TRIGGERED", 2), ("FORCE_CAST", 4), ("NO_MELEE_IF_OOM", 8), ("FORCE_TARGET_SELF", 16), ("CAST_AURA_NOT_PRESENT", 32), ("COMBAT_MOVE", 64)
                }
                ),
                  (3, new (string, int)[]
                {
                    ("NORMAL", 0),  ("IGNORE_GCD", 1), ("IGNORE_SPELL_AND_CATEGORY_CD", 2), ("IGNORE_POWER_AND_REAGENT_COST", 4), ("IGNORE_CAST_ITEM", 8), ("IGNORE_AURA_SCALING", 16), ("IGNORE_CAST_IN_PROGRESS", 32), ("IGNORE_COMBO_POINTS", 64), ("CAST_DIRECTLY", 128), ("IGNORE_AURA_INTERRUPT_FLAGS", 512), ("IGNORE_SET_FACING", 1024), ("IGNORE_SHAPESHIFT", 2048), ("IGNORE_CASTER_AURASTATE", 4096), ("DISALLOW_PROC_EVENTS", 8192), ("IGNORE_CASTER_MOUNTED_OR_ON_VEHICLE", 16384), ("IGNORE_BLOCKED_SPELL_FAMILY", 32768), ("IGNORE_CASTER_AURA", 131072), ("DONT_RESET_PERIODIC_TIMER", 262144), ("DONT_REPORT_CAST_ERROR", 524288), ("FULL_MASK", 1048576), ("IGNORE_EQUIPPED_ITEM_REQUIREMENT", 2097152), ("IGNORE_TARGET_CHECK", 4194304), ("FULL_DEBUG_MASK", 8388608)

                }
                )
                }
                ),
                (12, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                     ("NONE", 0), ("TIMED_OR_DEAD_DESPAWN", 1),  ("TIMED_OR_CORPSE_DESPAWN", 2),   ("TIMED_DESPAWN", 3), ("TIMED_DESPAWN_OUT_OF_COMBAT", 4), ("CORPSE_DESPAWN", 5), ("CORPSE_TIMED_DESPAWN", 6), ("DEAD_DESPAWN", 7),  ("MANUAL_DESPAWN", 8)
                }
                )
                }
                ),
                (85, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                    ("NORMAL", 0),  ("INTERRUPT_PREVIOUS", 1), ("TRIGGERED", 2), ("FORCE_CAST", 4), ("NO_MELEE_IF_OOM", 8), ("FORCE_TARGET_SELF", 16), ("CAST_AURA_NOT_PRESENT", 32), ("COMBAT_MOVE", 64)

                }
                ),
                  (3, new (string, int)[]
                {
                    ("NORMAL", 0),  ("IGNORE_GCD", 1), ("IGNORE_SPELL_AND_CATEGORY_CD", 2), ("IGNORE_POWER_AND_REAGENT_COST", 4), ("IGNORE_CAST_ITEM", 8), ("IGNORE_AURA_SCALING", 16), ("IGNORE_CAST_IN_PROGRESS", 32), ("IGNORE_COMBO_POINTS", 64), ("CAST_DIRECTLY", 128), ("IGNORE_AURA_INTERRUPT_FLAGS", 512), ("IGNORE_SET_FACING", 1024), ("IGNORE_SHAPESHIFT", 2048), ("IGNORE_CASTER_AURASTATE", 4096), ("DISALLOW_PROC_EVENTS", 8192), ("IGNORE_CASTER_MOUNTED_OR_ON_VEHICLE", 16384), ("IGNORE_BLOCKED_SPELL_FAMILY", 32768), ("IGNORE_CASTER_AURA", 131072), ("DONT_RESET_PERIODIC_TIMER", 262144), ("DONT_REPORT_CAST_ERROR", 524288), ("FULL_MASK", 1048576), ("IGNORE_EQUIPPED_ITEM_REQUIREMENT", 2097152), ("IGNORE_TARGET_CHECK", 4194304), ("FULL_DEBUG_MASK", 8388608)
                }
                )
                }
                ),
                (86, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                  ("NORMAL", 0),  ("INTERRUPT_PREVIOUS", 1), ("TRIGGERED", 2), ("FORCE_CAST", 4), ("NO_MELEE_IF_OOM", 8), ("FORCE_TARGET_SELF", 16), ("CAST_AURA_NOT_PRESENT", 32), ("COMBAT_MOVE", 64)
                }
                )
                }
                ),
                (134, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                  ("NORMAL", 0),  ("INTERRUPT_PREVIOUS", 1), ("TRIGGERED", 2), ("FORCE_CAST", 4), ("NO_MELEE_IF_OOM", 8), ("FORCE_TARGET_SELF", 16), ("CAST_AURA_NOT_PRESENT", 32), ("COMBAT_MOVE", 64)
                }
                ),
                  (3, new (string, int)[]
                {
                  ("NORMAL", 0),  ("IGNORE_GCD", 1), ("IGNORE_SPELL_AND_CATEGORY_CD", 2), ("IGNORE_POWER_AND_REAGENT_COST", 4), ("IGNORE_CAST_ITEM", 8), ("IGNORE_AURA_SCALING", 16), ("IGNORE_CAST_IN_PROGRESS", 32), ("IGNORE_COMBO_POINTS", 64), ("CAST_DIRECTLY", 128), ("IGNORE_AURA_INTERRUPT_FLAGS", 512), ("IGNORE_SET_FACING", 1024), ("IGNORE_SHAPESHIFT", 2048), ("IGNORE_CASTER_AURASTATE", 4096), ("DISALLOW_PROC_EVENTS", 8192), ("IGNORE_CASTER_MOUNTED_OR_ON_VEHICLE", 16384), ("IGNORE_BLOCKED_SPELL_FAMILY", 32768), ("IGNORE_CASTER_AURA", 131072), ("DONT_RESET_PERIODIC_TIMER", 262144), ("DONT_REPORT_CAST_ERROR", 524288), ("FULL_MASK", 1048576), ("IGNORE_EQUIPPED_ITEM_REQUIREMENT", 2097152), ("IGNORE_TARGET_CHECK", 4194304), ("FULL_DEBUG_MASK", 8388608)

                }
                )
                }
                ),
                (58, new (int, (string, int)[])[]
                { (1, new (string, int)[]
                {
                  ("SMARTAI_TEMPLATE_BASIC", 0),  ("SMARTAI_TEMPLATE_CASTER", 1), ("SMARTAI_TEMPLATE_TURRET", 2), ("SMARTAI_TEMPLATE_PASSIVE", 3), ("SMARTAI_TEMPLATE_CAGED_GO_PART", 4), ("SMARTAI_TEMPLATE_CAGED_NPC_PART", 5)


                }
                )
                }
                ),
                (53, new (int, (string, int)[])[]
                { (6, new (string, int)[]
                {
                  ("REACT_PASSIVE", 0),  ("REACT_DEFENSIVE", 1), ("REACT_AGGRESSIVE", 2), ("REACT_ASSIST", 3)


                }
                )
                }
                ),
                (8, new (int, (string, int)[])[]
                { (1, new (string, int)[]
                {
                    ("REACT_PASSIVE", 0),  ("REACT_DEFENSIVE", 1), ("REACT_AGGRESSIVE", 2), ("REACT_ASSIST", 3)

                }
                )
                }
                )
        };
        (int, (int, (string, int)[])[])[] eventButtonArray = new (int, (int, (string, int)[])[])[3]
{
                (8, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                    ("HOLY", 2),  ("FIRE", 4), ("NATURE", 8), ("FROST", 16), ("SHADOW", 32), ("ARCANE", 64)
                }
                )
                }
                ),

                (31, new (int, (string, int)[])[]
                { (2, new (string, int)[]
                {
                    ("HOLY", 2),  ("FIRE", 4), ("NATURE", 8), ("FROST", 16), ("SHADOW", 32), ("ARCANE", 64)
                }
                )
                }
                ),

                (34, new (int, (string, int)[])[]
                { (1, new (string, int)[]
                {
                ("IDLE_MOTION_TYPE", 0),  ("RANDOM_MOTION_TYPE", 1), ("WAYPOINT_MOTION_TYPE", 2), ("MAX_DB_MOTION_TYPE", 3), ("CONFUSED_MOTION_TYPE", 4), ("CHASE_MOTION_TYPE", 5), ("HOME_MOTION_TYPE", 6), ("FLIGHT_MOTION_TYPE", 7), ("POINT_MOTION_TYPE", 8), ("FLEEING_MOTION_TYPE", 9), ("DISTRACT_MOTION_TYPE", 10), ("ASSISTANCE_MOTION_TYPE", 11), ("ASSISTANCE_DISTRACT_MOTION_TYPE", 12), ("TIMED_FLEEING_MOTION_TYPE", 13), ("FOLLOW_MOTION_TYPE", 14), ("ROTATE_MOTION_TYPE", 15), ("EFFECT_MOTION_TYPE", 16), ("SPLINE_CHAIN_MOTION_TYPE", 17), ("FORMATION_MOTION_TYPE ", 18)
                }
                )
                }
                )

        };
        #endregion
        #region Индексы
        int selIndex = -1;
        int lastId = -1;
        int linkIndex = 0;
        #endregion
        #region Соединение
        MySqlConnection conn = null;
        #endregion
        #region Сохранения для окна "позаимстовать поведение"
        string inheritLastId = null;
        int inheritLastST = -1;
        #endregion
        #endregion
        #region Конструктор
        public MainWindow()
        {
            InitializeComponent();
            #region Создание иконки
            ResizeMode = ResizeMode.NoResize;
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/MainIcon.png"));
            #endregion
            #region Добавление фона
            try
            {
                BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/LunaStart.png"));
                background = new System.Windows.Shapes.Rectangle()
                {
                    Fill = new ImageBrush
                    {
                        ImageSource = image,
                        TileMode = TileMode.Tile,
                        Viewport = new Rect(0, 0, image.Width, image.Height),
                        ViewportUnits = BrushMappingMode.Absolute
                    },
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
                this.Height = image.Height; this.Width = image.Width;
            }
            catch { }
            #endregion
            #region Создание передаточных панелей
            WrapPanel wrapPanelHost = new WrapPanel()
            {
                Height = double.NaN,
                MinWidth = this.Width,
                MaxWidth = this.Width,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0.5 }
            };
            WrapPanel wrapPanelDB = new WrapPanel()
            {
                Height = double.NaN,
                MinWidth = this.Width,
                MaxWidth = this.Width,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0.5 }
            };
            WrapPanel wrapPanelName = new WrapPanel()
            {
                Height = double.NaN,
                MinWidth = this.Width,
                MaxWidth = this.Width,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0.5 }
            };
            WrapPanel wrapPanelPassword = new WrapPanel()
            {
                Height = double.NaN,
                MinWidth = this.Width,
                MaxWidth = this.Width,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0.5 }
            };
            #endregion
            #region Поля и текст
            TextBlock textBlockHost = new TextBlock()
            {
                Text = "HostName",
                FontSize = 18,
                Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.DarkBlue),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 00, 25, 00),
            };
            TextBox textBoxHost = new TextBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(15, 00, 25, 00),
                Text = "localhost",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.IndianRed),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                FontSize = 18,
            }; textBoxes[0] = textBoxHost;
            TextBox textBoxPort = new TextBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(15, 00, 25, 00),
                Text = "3306",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.IndianRed),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                FontSize = 18,
            }; textBoxes[1] = textBoxPort;
            TextBlock textBlockDB = new TextBlock()
            {
                Text = "World DB  ",
                FontSize = 18,
                Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.DarkBlue),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 00, 25, 00),
            };
            TextBox textBoxDB = new TextBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(15, 00, 25, 00),
                Text = "world",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.IndianRed),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                FontSize = 18,
            }; textBoxes[2] = textBoxDB;
            TextBlock textBlockName = new TextBlock()
            {
                Text = "Username  ",
                FontSize = 18,
                Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Green),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 00, 25, 00),
            };
            TextBox textBoxName = new TextBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(15, 00, 25, 00),
                Text = "root",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.IndianRed),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                FontSize = 18,
            }; textBoxes[3] = textBoxName;
            TextBlock textBlockPassword = new TextBlock()
            {
                Text = "Password   ",
                FontSize = 18,
                Background = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Green),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 00, 25, 00),
            };
            PasswordBox textBoxPassword = new PasswordBox()
            {
                Width = 100,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(15, 00, 25, 00),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.IndianRed),
                BorderThickness = new Thickness(2),
                BorderBrush = new SolidColorBrush(Colors.DarkBlue),
                FontSize = 18,
            }; password = textBoxPassword;
            #endregion
            #region Создание кнопки
            buttonCreate = new Button()
            {
                Name = "Соединение",
                Content = "Соединение",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 300,
                Height = 50,
                Margin = new Thickness(00, 00, 00, 10),
                Foreground = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.DarkBlue),
                FontWeight = FontWeights.Bold,
                FontSize = 18,
            };
            #endregion
            #region Сборка элементов
            #region Добавление фона
            MainRoot.Children.Add(background); Grid.SetRowSpan(background, 7); Grid.SetColumnSpan(background, 4);
            #endregion
            #region Имя хоста
            wrapPanelHost.Children.Add(textBlockHost);
            wrapPanelHost.Children.Add(textBoxHost);
            wrapPanelHost.Children.Add(textBoxPort);
            #endregion
            #region База данных
            wrapPanelDB.Children.Add(textBlockDB);
            wrapPanelDB.Children.Add(textBoxDB);
            #endregion
            #region Имя пользователя
            wrapPanelName.Children.Add(textBlockName);
            wrapPanelName.Children.Add(textBoxName);
            #endregion
            #region Пароль
            wrapPanelPassword.Children.Add(textBlockPassword);
            wrapPanelPassword.Children.Add(textBoxPassword);
            #endregion
            #region Добавление всех элементов в основную разметку
            MainRoot.Children.Add(wrapPanelHost); Grid.SetRow(wrapPanelHost, 1); Grid.SetColumnSpan(wrapPanelHost, 4);
            MainRoot.Children.Add(wrapPanelDB); Grid.SetRow(wrapPanelDB, 2); Grid.SetColumnSpan(wrapPanelDB, 4);
            MainRoot.Children.Add(wrapPanelName); Grid.SetRow(wrapPanelName, 3); Grid.SetColumnSpan(wrapPanelName, 4);
            MainRoot.Children.Add(wrapPanelPassword); Grid.SetRow(wrapPanelPassword, 4); Grid.SetColumnSpan(wrapPanelPassword, 4);
            MainRoot.Children.Add(buttonCreate); Grid.SetRow(buttonCreate, 6); Grid.SetColumnSpan(buttonCreate, 4);
            #endregion
            #endregion
            #region Подписка на события
            buttonCreate.Click += ConnectButtomClick_Click;
            buttonCreate.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
            buttonCreate.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
            password.KeyUp += Password_KeyUp;
            #endregion
        }
        #endregion
        #region Реализация событий до входа
        /// <summary>
        /// Соединение при нажатии кнопки Enter. (симуляция нажатия кнопки "соединение")
        /// </summary>
        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(buttonCreate);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();
            }
        }
        /// <summary>
        /// Соединение и "развертывание" основного окна программы.
        /// </summary>
        private void ConnectButtomClick_Click(object sender, RoutedEventArgs e)
        {
            #region Данные соединение и создание самого соединения
            string connStr = "server=" + textBoxes[0].Text + ";user=" + textBoxes[3].Text + ";database=" + textBoxes[2].Text + ";port=" + textBoxes[1].Text + ";password=" + password.Password.ToString();
            conn = new MySqlConnection(connStr);
            #endregion
            try
            {
                #region Соединение
                conn.Open();
                #endregion
                #region Изменение графики
                #region Фон и размеры
                MainRoot.Children.Clear();
                ResizeMode = ResizeMode.CanResize;
                try
                {
                    image = GetImageMultiextension("Background/Background");
                    background = new System.Windows.Shapes.Rectangle()
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
                }
                catch
                {
                    try
                    {
                        image = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/luna_background.png"));
                        background = new System.Windows.Shapes.Rectangle()
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
                    }
                    catch { }
                }
                #endregion
                #region Создание границы
                Border border = new Border()
                {
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Opacity = 0.90,
                    Margin = new Thickness(5, 25, 5, 25),
                    CornerRadius = new CornerRadius(10, 10, 10, 10)
                };
                #endregion
                #region Создание передаточных панелей
                wrapPanelMain = new WrapPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Margin = new Thickness(0, 0, 0, -30)
                };
                wrapPanelDataGrid = new WrapPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Margin = new Thickness(0, 0, 0, -30),
                    IsEnabled = false
            };
                wrapPanelScriptDescription = new WrapPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    IsEnabled = false
                };
                wrapPanelEditor = new WrapPanel()
                {
                    //Height = double.NaN,
                    //Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    IsEnabled = false
                    //Background = Brushes.Red
                };
                wrapPanelParameters = new WrapPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    IsEnabled = false
                };
                #endregion
                #region Основная панель
                Border mainBorder = new Border()
                {
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Opacity = 0.90,
                    CornerRadius = new CornerRadius(10, 10, 10, 10),
                    Margin = new Thickness(0, 0, 0, -30)
                };
                StackPanel stackPanelMainId = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                };
                TextBlock textMainMain = new TextBlock()
                {
                    Text = "Основная информация",
                    FontSize = 16,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 0, 25, 0),
                    TextAlignment = TextAlignment.Left,
                };
                TextBlock textIdMain = new TextBlock()
                {
                    Text = "1.Введите EntryID",
                    FontSize = 12,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.DodgerBlue),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 00, 25, 00),
                    TextAlignment = TextAlignment.Left,
                };
                StackPanel stackPanelEntryId = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal
                };
                textBoxEntryId = new TextBox()
                {
                    Width = 100,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    //Text = "localhost",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                    Padding = new Thickness(5, 3.25, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                BitmapImage refImage = new BitmapImage(new Uri("pack://application:,,,/DarkWowSoft;component/Resources/refresh.png"));
                buttonRefreshEntryId = new Button()
                {
                    //Content = refImage,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    Foreground = new SolidColorBrush(Colors.White),
                    //Background = new SolidColorBrush(Colors.DodgerBlue),
                    Background = new ImageBrush() { ImageSource = refImage },
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                };
                TextBlock textSourceType = new TextBlock()
                {
                    Text = "2.Выберите тип данных",
                    FontSize = 12,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.DodgerBlue),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 00, 25, 00),
                    TextAlignment = TextAlignment.Left,
                    Padding = new Thickness(5, 7.5, 0, 0)
                };
                comboBoxType = new ComboBox()
                {
                    Width = 200,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontStretch = new FontStretch(),
                    Padding = new Thickness(5, 7.5, 0, 0),
                    FontSize = 10
                }; comboBoxType.SelectedIndex = 0;
                (string, int)[] sourceItemArray = new (string, int)[13] { ("CREATURE", 0), ("GAMEOBJECT", 1), ("AREATRIGGER", 2), ("EVENT", 3), ("GOSSIP", 4), ("QUEST", 5), ("SPELL", 6), ("TRANSPORT", 7), ("INSTANCE", 8), ("TIMED ACTIONLIST", 9), ("SCENE", 10), ("AREATRIGGER ENTITY", 11), ("AREATRIGGER ENTITY SERVERSIDE", 12) };
                foreach ((string, int) sourceItem in sourceItemArray)
                {
                    comboBoxType.Items.Add(new ComboBoxItem { Content = sourceItem.Item1 + " - " + sourceItem.Item2.ToString(), Tag = sourceItem.Item2, FontWeight = FontWeights.SemiBold, FontSize = 12 });
                }
                StackPanel stackPanelLockId = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(15, 10, 0, 0),
                };
                TextBlock textId = new TextBlock()
                {
                    Text = "ID",
                    FontSize = 16,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxId = new TextBox()
                {
                    Width = 50,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    //Text = "localhost",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                    Padding = new Thickness(0, 3.25, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    IsEnabled = false,
                };
                TextBlock textLock = new TextBlock()
                {
                    Text = "Блок:",
                    FontSize = 16,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(5, 5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                    IsEnabled = false
                };
                checkBoxId = new CheckBox()
                {
                    Margin = new Thickness(5, 7.5, 0, 0),
                    LayoutTransform = new ScaleTransform(1.3, 1.3),
                    IsChecked = true,
                    IsEnabled = false
                };
                StackPanel stackPanelCopyNew = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(15, 10, 0, 0),
                };
                buttonCopy = new Button()
                {
                    Content = "Копия",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    IsEnabled = false
                };
                buttonNew = new Button()
                {
                    Content = "Новый",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    IsEnabled = false
                };
                StackPanel stackPanelDeleteSave = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(15, 10, 0, 0),
                };
                buttonDelete = new Button()
                {
                    Content = "Удалить",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    IsEnabled = false
                };
                buttonSave = new Button()
                {
                    Content = "Сохранить",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    IsEnabled = false
                };
                buttonUpdateTempBase = new Button()
                {
                    Content = "Применить",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 205,
                    Height = 40,
                    Margin = new Thickness(15, 15, 0, 0),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Colors.ForestGreen),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    IsEnabled = false,
                };
                #endregion
                #region Панель "Data Grid"
                Border dgBorder = new Border()
                {
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Opacity = 0.90,
                    CornerRadius = new CornerRadius(10, 10, 10, 10),
                    Margin = new Thickness(0, 0, 0, -30)
                };
                StackPanel stackPanelDG = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(10, 10, 0, 0),
                };
                StackPanel stackPanelDGUp = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                StackPanel stackPanelNameSmart = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                textDGName = new TextBlock()
                {
                    Text = "Имя",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.DodgerBlue),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 5, 0),
                    TextAlignment = TextAlignment.Left,
                    ContextMenu = new ContextMenu()
                };
                textDGNameLocal = new TextBlock()
                {
                    Text = "[ Пер.имя ]",
                    FontSize = 12,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.DodgerBlue),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 5, 0),
                    TextAlignment = TextAlignment.Left,
                    ContextMenu = new ContextMenu()
                };
                textDGSmartAI = new TextBlock()
                {
                    Text = "[ SmartAI ]",
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.ForestGreen),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 5, 0),
                    TextAlignment = TextAlignment.Left,
                };
                #region Добавление ContextMenu
                MenuItem copyContextDGName = new MenuItem { Header = "Копировать", Tag = "DGname" };
                textDGName.ContextMenu.Items.Add(copyContextDGName);
                MenuItem copyContextDGNameLocal = new MenuItem { Header = "Копировать", Tag = "DGnameLocal" };
                textDGNameLocal.ContextMenu.Items.Add(copyContextDGNameLocal);
                #endregion
                buttonSQL = new Button()
                {
                    Content = "Сгенерировать SQL",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 180,
                    Height = 40,
                    Margin = new Thickness(5, 0, 0, 0),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Colors.ForestGreen),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                };
                creatureTextButton = new Button()
                {
                    Content = "Creature Text",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 180,
                    Height = 40,
                    Margin = new Thickness(5, 0, 0, 0),
                    //Foreground = new SolidColorBrush(Colors.White),
                    //Background = new SolidColorBrush(Colors.),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                };
                buttonSwitchSmart = new Button()
                {
                    Content = "Включить SmartAI",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 150,
                    Height = 40,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                };
                #region Стиль выделения в DataGrid
                Style style_dg = new Style(typeof(DataGridCell));
                Trigger trigger_dg = new Trigger() { Property = DataGridCell.IsSelectedProperty, Value = true };
                style_dg.Setters.Add(new Setter(DataGridCell.FocusableProperty, false));
                trigger_dg.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.DodgerBlue)));
                trigger_dg.Setters.Add(new Setter(DataGridCell.ForegroundProperty, new SolidColorBrush(Colors.White)));
                trigger_dg.Setters.Add(new Setter(DataGridCell.FontWeightProperty, FontWeights.Bold));
                style_dg.Triggers.Add(trigger_dg);
                #endregion
                dataGrid = new System.Windows.Controls.DataGrid
                {
                    Height = Height * 0.6,
                    Width = Width * 0.92,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    MinHeight = 1,
                    MinWidth = 1,
                    CanUserSortColumns = true,
                    FontSize = 11,
                    CanUserAddRows = false,
                    CanUserDeleteRows = false,
                    SelectionMode = DataGridSelectionMode.Single,
                    IsReadOnly = true,
                    BorderThickness = new Thickness(2),
                    Tag = "DG",
                    Margin = new Thickness(0, 15, 0, 15),
                    CellStyle = style_dg,
                    EnableRowVirtualization = false
                };
                #region Добавление заголовков к колонкам DataGrid
                Style style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                DataGridTextColumn idColumn = new DataGridTextColumn { Width = 25, Header = "ID", Binding = new System.Windows.Data.Binding("id"), FontSize = 11, ElementStyle = style };
                dataGrid.Columns.Add(idColumn);
                DataGridTextColumn phaseColumn = new DataGridTextColumn { Width = 50, Header = "Phase", Binding = new System.Windows.Data.Binding("phase"), FontSize = 11, ElementStyle = style };
                dataGrid.Columns.Add(phaseColumn);
                DataGridTextColumn linkColumn = new DataGridTextColumn { Width = 35, Header = "Link", Binding = new System.Windows.Data.Binding("link"), FontSize = 11, ElementStyle = style };
                dataGrid.Columns.Add(linkColumn);
                commentColumn = new DataGridTextColumn { Width = 250, Header = "Комментарий", Binding = new System.Windows.Data.Binding("comment"), FontSize = 11, ElementStyle = style };
                dataGrid.Columns.Add(commentColumn);
                #endregion
                #endregion
                #region Панель "описание скрипта"
                textBlockComment = new TextBlock()
                {
                    Text = "Комментарий:",
                    FontSize = 18,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 0, 25, 0),
                    TextAlignment = TextAlignment.Center,
                };
                textBoxComment = new TextBox()
                {
                    Width = 400,
                    Height = 28,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 0),
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                };
                #endregion
                #region Панель "Редактор"
                #region Общие
                Border borderEditor = new Border()
                {
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Opacity = 0.90,
                    Margin = new Thickness(0, -30, 0, 0),
                    CornerRadius = new CornerRadius(10, 10, 10, 10)
                };
                StackPanel stackPanelEditor = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, -30, 0, 0),
                };
                TextBlock textEditor = new TextBlock()
                {
                    Text = "Поведение",
                    FontSize = 16,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 0, 25, 0),
                    TextAlignment = TextAlignment.Left,
                };
                inheritBehaviorButton = new Button()
                {
                    Content = "Позаимствовать поведение",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = Width * 0.6,
                    Height = 25,
                    Margin = new Thickness(5, 10, 0, 0),
                    // Foreground = new SolidColorBrush(Colors.White),
                    // Background = new SolidColorBrush(Colors.LightGray),
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                };
                #endregion
                #region Link панель
                StackPanel stackPanelSelectLink = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textLinkToScript = new TextBlock()
                {
                    Text = "Link to script:",
                    FontSize = 12,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 2.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxSelectLinkIndex = new TextBox()
                {
                    Width = 50,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    //Text = "localhost",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 1.5, 0, 0),
                    TextAlignment = TextAlignment.Center
                };
                linkButton = new Button()
                {
                    Content = "Выбрать",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 100,
                    Height = 20,
                    Margin = new Thickness(10, 2.5, 0, 0),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                };
                TextBlock textLinkToScriptPreserved = new TextBlock()
                {
                    Text = "Link from:",
                    FontSize = 12,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 2.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxSelectLinkPreserved = new TextBox()
                {
                    Width = 50,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    //Text = "localhost",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 1.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    IsEnabled = false,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    Text = "None"
                };
                #endregion
                #region Event
                StackPanel stackPanelEvent = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textLinkToOnEvent = new TextBlock()
                {
                    Text = "On Event",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                comboBoxEvent = new ComboBox()
                {
                    Name = "Event",
                    Width = 250,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontStretch = new FontStretch(),
                    Padding = new Thickness(5, 3.25, 0, 0),
                    FontSize = 10
                }; comboBoxEvent.SelectedIndex = 0;
                (string, int)[] sourceEventArray = new (string, int)[83] { ("UPDATE_IC (Or ActionList)", 0), ("UPDATE_OOC", 1),
                 ("HEALTH%", 2),  ("MANA%", 3),
                 ("AGGRO", 4),  ("KILL", 5),
                 ("DEATH", 6),  ("EVADE", 7),
                 ("SPELLHIT", 8),  ("RANGE", 9),
                 ("OOC_LOS", 10),  ("RESPAWN", 11),
                 ("TARGET_HEALTH%", 12),  ("TARGET_CASTING", 13),
                 ("FRIENDLY_HEALTH", 14),  ("FRIENDLY_IS_CC", 15),
                 ("FRIENDLY_MISSING_BUFF", 16),  ("SUMMONED_UNIT", 17),
                 ("TARGET_MANA%", 18),  ("ACCEPTED_QUEST", 19),
                 ("REWARD_QUEST", 20),  ("REACHED_HOME", 21),
                 ("RECEIVE_EMOTE", 22),  ("HAS_AURA", 23),
                 ("TARGET_BUFFED", 24),  ("RESET", 25),
                 ("IN_COMBAT_LOS", 26),  ("PASSENGER_BOARDED", 27),
                 ("PASSENGER_REMOVED", 28),  ("CHARMED", 29),
                 ("CHARMED_TARGET", 30),  ("SPELLHIT_TARGET", 31),
                 ("DAMAGED", 32),  ("DAMAGED_TARGET", 33),
                 ("MOVEMENT_IN_FORM", 34),  ("SUMMON_DESPAWNED", 35),
                 ("CORPSE_REMOVED", 36),  ("AI_INIT", 37),
                 ("DATA_SET", 38),  ("WAYPOINT_START", 39),
                 ("WAYPOINT_REACHED", 40),  ("TRANSPORT_ADDPLAYER", 41),
                 ("TRANSPORT_ADDCREATURE", 42),  ("TRANSPORT_REMOVE_PLAYER", 43),
                 ("TRANSPORT_RELOCATE", 44),  ("INSTANCE_PLAYER_ENTER", 45),
                 ("AREATRIGGER_ONTRIGGER", 46),  ("QUEST_ACCEPTED", 47),
                 ("QUEST_OBJ_COMPLETION", 48),  ("QUEST_COMPLETION", 49),
                 ("QUEST_REWARDED", 50),  ("QUEST_FAIL", 51),
                 ("TEXT_OVER", 52),  ("RECEIVE_HEAL", 53),
                 ("JUST_SUMMONED", 54),  ("WAYPOINT_PAUSED", 55),
                 ("WAYPOINT_RESUMED", 56),  ("WAYPOINT_STOPPED", 57),
                 ("WAYPOINT_ENDED", 58),  ("TIMED_EVENT_TRIGGERED", 59),
                 ("UPDATE_IC_OOC", 60),  ("LINK", 61),
                 ("GOSSIP_SELECT", 62),  ("JUST_CREATED", 63),
                 ("GOSSIP_HELLO", 64),  ("FOLLOW_COMPLETED", 65),
                 ("PHASE_CHANGE", 66),  ("IS_BEHIND_TARGET", 67),
                 ("GAME_EVENT_START", 68),  ("GAME_EVENT_END", 69),
                 ("GO_LOOT_STATE_CHANGED", 70),  ("GO_EVENT_INFORM", 71),
                 ("ACTION_DONE", 72),  ("ON_SPELLCLICK", 73),
                 ("FRIENDLY_HEALTH%", 74),  ("DISTANCE_CREATURE", 75),
                 ("DISTANCE_GAMEOBJECT", 76),  ("COUNTER_SET", 77),
                 ("SCENE_START", 78),  ("SCENE_TRIGGER", 79),
                 ("SCENE_CANCEL", 80),  ("SCENE_COMPLETE", 81),
                 ("SUMMONED_UNIT_DIES", 82) };
                foreach ((string, int) sourceItem in sourceEventArray)
                {
                    comboBoxEvent.Items.Add(new ComboBoxItem { Content = sourceItem.Item1 + " - " + sourceItem.Item2.ToString(), Tag = sourceItem.Item2, FontWeight = FontWeights.SemiBold, FontSize = 12 });
                }
                textBoxEvent = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                #endregion
                #region Action
                StackPanel stackPanelAction = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textLinkToOnAction = new TextBlock()
                {
                    Text = "Do Action",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                comboBoxAction = new ComboBox()
                {
                    Name = "Action",
                    Width = 250,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontStretch = new FontStretch(),
                    Padding = new Thickness(5, 3.25, 0, 0),
                    FontSize = 10
                }; comboBoxAction.SelectedIndex = 0;
                (string, int)[] sourceActionArray = new (string, int)[147] { ("NONE", 0), ("TALK", 1),
                ("SET_FACTION", 2), ("MORPH_TO_ENTRY_OR_MODEL", 3),
                ("PLAY_SOUND", 4), ("PLAY_EMOTE", 5),
                ("FAIL_QUEST", 6), ("OFFER_QUEST", 7),
                ("SET_REACT_STATE", 8), ("ACTIVATE_OBJECT", 9),
                ("RANDOM_EMOTE", 10), ("CAST", 11),
                ("SUMMON_CREATURE", 12), ("THREAT_SINGLE%", 13),
                ("THREAT_ALL%", 14), ("CALL_AREA_EXPLORED_OR_EVENT_HAPPENS", 15),
                ("SET_INGAME_PHASE_ID", 16), ("SET_EMOTE_STATE", 17),
                ("SET_UNIT_FLAG", 18), ("REMOVE_UNIT_FLAG", 19),
                ("AUTO_ATTACK", 20), ("ALLOW_COMBAT_MOVEMENT", 21),
                ("SET_EVENT_PHASE", 22), ("INC_EVENT_PHASE", 23),
                ("EVADE", 24), ("FLEE_FOR_ASSIST", 25),
                ("CALL_GROUP_EVENT_HAPPENS", 26), ("COMBAT_STOP", 27),
                ("REMOVE_AURAS_FROM_SPELL", 28), ("FOLLOW", 29),
                ("RANDOM_PHASE", 30), ("RANDOM_PHASE_RANGE", 31),
                ("RESET_GOBJECT", 32), ("CREDIT_KILLED_MONSTER", 33),
                ("SET_INST_DATA", 34), ("SET_INST_DATA64", 35),
                ("UPDATE_TEMPLATE", 36), ("DIE", 37),
                ("SET_IN_COMBAT_WITH_ZONE", 38), ("CALL_FOR_HELP", 39),
                ("SELECT_WEAPON_SHOW_TYPE", 40), ("FORCE_DESPAWN", 41),
                ("SET_UNPUTABLE_HP_LEVEL", 42), ("MOUNT_TO_ENTRY_OR_MODEL", 43),
                ("SET_INGAME_PHASE_MASK", 44), ("SET_DATA", 45),
                ("STOP_ATTACK", 46), ("SET_VISIBILITY", 47),
                ("SET_ACTIVE", 48), ("START_ATTACK", 49),
                ("SUMMON_GAMEOBJECT", 50), ("KILL_UNIT", 51),
                ("ACTIVATE_TAXI", 52), ("WAYPOINT_START", 53),
                ("WAYPOINT_PAUSE", 54), ("WAYPOINT_STOP", 55),
                ("ADD_ITEM", 56), ("REMOVE_ITEM", 57),
                ("INSTALL_AI_TEMPLATE", 58), ("SET_RUN", 59),
                ("SET_DISABLE_GRAVITY", 60), ("SET_SWIM", 61),
                ("TELEPORT", 62), ("SET_COUNTER", 63),
                ("STORE_TARGET_LIST", 64), ("WAYPOINT_CONTINUE", 65),
                ("SET_ORIENTATION", 66), ("CREATE_TIMED_EVENT", 67),
                ("PLAYMOVIE", 68), ("MOVE_TO_POSITION", 69),
                ("ENABLE_TEMP_GOBJECT", 70), ("EQUIP_WEAPON", 71),
                ("CLOSE_GOSSIP", 72), ("TRIGGER_TIMED_EVENT", 73),
                ("REMOVE_TIMED_EVENT", 74), ("ADD_AURA", 75),
                ("OVERRIDE_SCRIPT_BASE_OBJECT.!WARNING!", 76), ("RESET_SCRIPT_BASE_OBJECT", 77),
                ("CALL_SCRIPT_RESET", 78), ("SET_RANGED_MOVEMENT", 79),
                ("CALL_TIMED_ACTIONLIST", 80), ("SET_NPC_FLAG", 81),
                ("ADD_NPC_FLAG", 82), ("REMOVE_NPC_FLAG", 83),
                ("SIMPLE_TALK", 84), ("SELF_CAST", 85),
                ("CROSS_CAST", 86), ("CALL_RANDOM_TIMED_ACTIONLIST", 87),
                ("CALL_RANDOM_RANGE_TIMED_ACTIONLIST", 88), ("RANDOM_MOVE", 89),
                ("SET_UNIT_FIELD_BYTES_1", 90), ("REMOVE_UNIT_FIELD_BYTES_1", 91),
                ("INTERRUPT_SPELL", 92), ("SEND_GO_CUSTOM_ANIM", 93),
                ("SET_DYNAMIC_FLAG", 94), ("ADD_DYNAMIC_FLAG", 95),
                ("REMOVE_DYNAMIC_FLAG", 96), ("JUMP_TO_POS", 97),
                ("SEND_GOSSIP_MENU", 98), ("GOBJECT_SET_LOOT_STATE", 99),
                ("SEND_TARGET_TO_TARGET", 100), ("SET_HOME_POS", 101),
                ("SET_HEALTH_REGEN", 102), ("SET_ROOT", 103),
                ("SET_GOBJECT_FLAG", 104), ("ADD_GOBJECT_FLAG", 105),
                ("REMOVE_GOBJECT_FLAG", 106), ("SUMMON_CREATURE_GROUP", 107),
                ("SET_POWER", 108), ("ADD_POWER", 109),
                ("REMOVE_POWER", 110), ("GAME_EVENT_STOP", 111),
                ("GAME_EVENT_START", 112), ("START_CLOSEST_WAYPOINT", 113),
                ("MOVE_OFFSET", 114), ("PLAY_RANDOM_SOUND", 115),
                ("SET_CORPSE_DELAY", 116), ("DISABLE_EVADE", 117),
                ("GOBJECT_SET_GOBJECT_STATE", 118), ("SET_CAN_FLY", 119),
                ("REMOVE_AURAS_BY_TYPE", 120), ("SET_SIGHT_DISTANCE", 121),
                ("FLEE", 122), ("ADD_THREAT", 123),
                ("LOAD_EQUIPMENT", 124), ("TRIGGER_RANDOM_TIMED_EVENT", 125),
                ("REMOVE_ALL_GAMEOBJECTS", 126), ("PAUSE_MOVEMENT", 127),
                ("PLAY_ANIM_KIT", 128), ("SCENE_PLAY", 129),
                ("SCENE_CANCEL", 130), ("SPAWN_SPAWNGROUP", 131),
                ("DESPAWN_SPAWNGROUP", 132), ("RESPAWN_BY_SPAWN_ID", 133),
                ("INVOKER_CAST", 134), ("PLAY_CINEMATIC", 135),
                ("SET_MOVEMENT_SPEED", 136), ("PLAY_SPELL_VISUAL_KIT", 137),
                ("OVERRIDE_LIGHT", 138), ("OVERRIDE_WEATHER", 139),
                ("SET_AI_ANIM_KIT", 140), ("SET_HOVER", 141),
                ("SET_HEALTH%", 142), ("CREATE_CONVERSATION", 143),
                ("SET_IMMUNE_PC", 144), ("SET_IMMUNE_NPC", 145),
                ("SET_UNINTERACTIBLE", 146) };
                foreach ((string, int) sourceItem in sourceActionArray)
                {
                    comboBoxAction.Items.Add(new ComboBoxItem { Content = sourceItem.Item1 + " - " + sourceItem.Item2.ToString(), Tag = sourceItem.Item2, FontWeight = FontWeights.SemiBold, FontSize = 12 });
                }
                textBoxAction = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                #endregion
                #region Target
                StackPanel stackPanelTarget = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textLinkToAtTarget = new TextBlock()
                {
                    Text = "At Target",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                comboBoxTarget = new ComboBox()
                {
                    Name = "Target",
                    Width = 250,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontStretch = new FontStretch(),
                    Padding = new Thickness(5, 3.25, 0, 0),
                    FontSize = 10
                }; comboBoxTarget.SelectedIndex = 0;
                (string, int)[] sourceTargetArray = new (string, int)[31] { ("NONE", 0), ("SELF", 1),
                ("VICTIM", 2), ("HOSTILE_SECOND_AGGRO", 3),
                ("HOSTILE_LAST_AGGRO", 4), ("RANDOM", 5),
                ("RANDOM_NOT_TOP", 6), ("ACTION_INVOKER", 7),
                ("POSITION", 8), ("CREATURE_RANGE", 9),
                ("CREATURE_GUID", 10), ("CREATURE_DISTANCE", 11),
                ("TARGET_STORED", 12), ("GAMEOBJECT_RANGE", 13),
                ("GAMEOBJECT_GUID", 14), ("GAMEOBJECT_DISTANCE", 15),
                ("INVOKER_PARTY", 16), ("PLAYER_RANGE", 17),
                ("PLAYER_DISTANCE", 18), ("CLOSEST_CREATURE", 19),
                ("CLOSEST_GAMEOBJECT", 20), ("CLOSEST_PLAYER", 21),
                ("ACTION_INVOKER_UNIT", 22), ("OWNER_OR_SUMMONER", 23),
                ("THREAT_LIST", 24), ("RANDOM_HOSTILE_TARGET", 25),
                ("RANDOM_FRENDLY_TARGET", 26), ("LOOT_RECIPIENTS", 27),
                ("FARTHEST_TARGET", 28), ("VEHICLE_ACCESSORY", 29),
                ("CLOSEST_UNSPAWNED_GAMEOBJECT", 30) };
                foreach ((string, int) sourceItem in sourceTargetArray)
                {
                    comboBoxTarget.Items.Add(new ComboBoxItem { Content = sourceItem.Item1 + " - " + sourceItem.Item2.ToString(), Tag = sourceItem.Item2, FontWeight = FontWeights.SemiBold, FontSize = 12 });
                }
                textBoxTarget = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                #endregion
                #region Chance
                StackPanel stackPanelChance = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textChance = new TextBlock()
                {
                    Text = "Chance",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxChance = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "100"
                };
                chanceSlider = new Slider()
                {
                    Width = 250,
                    Height = 20,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    Interval = 1,
                    Maximum = 100
                };
                #endregion
                #region PhaseMask
                StackPanel stackPanelPhaseMask = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textMask = new TextBlock()
                {
                    Text = "Phase Mask",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxMask = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                Button selectMaskButton = new Button()
                {
                    Content = "Select Mask",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 250,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                };
                #endregion
                #region Flag
                StackPanel stackPanelEventFlag = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 10, 0, 0),
                };
                TextBlock textFlag = new TextBlock()
                {
                    Text = "Event Flag",
                    Width = 65,
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Left,
                };
                textBoxEventFlag = new TextBox()
                {
                    Width = 30,
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    Padding = new Thickness(0, 0.5, 0, 0),
                    TextAlignment = TextAlignment.Center,
                    Text = "0"
                };
                Button selectFlagButton = new Button()
                {
                    Content = "Select Flag",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 250,
                    Height = 20,
                    Margin = new Thickness(5, 0, 0, 0),
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 12,
                };
                #endregion
                #endregion
                #region Панель "Параметры"
                Border borderParam = new Border()
                {
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Opacity = 0.90,
                    Margin = new Thickness(0, -30, 0, 0),
                    CornerRadius = new CornerRadius(10, 10, 10, 10)
                };
                StackPanel stackPanelParam = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, -30, 0, 0),
                };
                TextBlock textParam = new TextBlock()
                {
                    Text = "Параметры поведения",
                    FontSize = 16,
                    //Background = new SolidColorBrush(Colors.White),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(15, 0, 25, 0),
                    TextAlignment = TextAlignment.Left,
                };
                tabControl = new TabControl()
                {
                    Background = new SolidColorBrush { Opacity = 0.5 },
                    Width = this.Width / 1.2,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                TabItem tabItemEvent = new TabItem
                {
                    Header = "Event",
                    FontWeight = FontWeights.SemiBold,
                    Width = 100,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                TabItem tabItemAction = new TabItem
                {
                    Header = "Action",
                    FontWeight = FontWeights.SemiBold,
                    Width = 100,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                TabItem tabItemTarget = new TabItem
                {
                    Header = "Target",
                    FontWeight = FontWeights.SemiBold,
                    Width = 100,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                #region Субпанель Event
                StackPanel stackPanelSubEvent = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 0),
                }; EAT[0] = stackPanelSubEvent;
                for (int i = 0; i < 5; i++)
                {
                    StackPanel stackPanelInner = new StackPanel()
                    {
                        Height = double.NaN,
                        Width = double.NaN,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = new SolidColorBrush { Opacity = 0.0 },
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 10, 0, 0),
                    };
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = "-UNUSED PARAMETR-",
                        Width = 250,
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(10, 0.5, 0, 0),
                        TextAlignment = TextAlignment.Left,
                    };
                    TextBox textBox = new TextBox()
                    {
                        Width = 80,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0),
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        FontSize = 12,
                        Padding = new Thickness(0, 1.5, 0, 0),
                        TextAlignment = TextAlignment.Center,
                        //IsEnabled = false,
                        Background = new SolidColorBrush(Colors.DarkGray),
                        Text = "0"
                    };
                    textBox.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                    stackPanelInner.Children.Add(textBlock); stackPanelInner.Children.Add(textBox);
                    stackPanelSubEvent.Children.Add(stackPanelInner);
                }
                #endregion
                #region Субпанель Action
                StackPanel stackPanelSubAction = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 0),
                }; EAT[1] = stackPanelSubAction;
                for (int i = 0; i < 6; i++)
                {
                    StackPanel stackPanelInner = new StackPanel()
                    {
                        Height = double.NaN,
                        Width = double.NaN,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = new SolidColorBrush { Opacity = 0.0 },
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 10, 0, 0),
                    };
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = "-UNUSED PARAMETR-",
                        Width = 250,
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(10, 0.5, 0, 0),
                        TextAlignment = TextAlignment.Left,
                    };
                    TextBox textBox = new TextBox()
                    {
                        Width = 80,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0),
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        FontSize = 12,
                        Padding = new Thickness(0, 1.5, 0, 0),
                        TextAlignment = TextAlignment.Center,
                        //IsEnabled = false,
                        Background = new SolidColorBrush(Colors.DarkGray),
                        Text = "0"
                    };
                    textBox.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                    stackPanelInner.Children.Add(textBlock); stackPanelInner.Children.Add(textBox);
                    stackPanelSubAction.Children.Add(stackPanelInner);
                }
                #endregion
                #region Субпанель Target
                StackPanel stackPanelSubTarget = new StackPanel()
                {
                    Height = double.NaN,
                    Width = double.NaN,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush { Opacity = 0.0 },
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 0),
                }; EAT[2] = stackPanelSubTarget;
                for (int i = 0; i < 8; i++)
                {
                    StackPanel stackPanelInner = new StackPanel()
                    {
                        Height = double.NaN,
                        Width = double.NaN,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = new SolidColorBrush { Opacity = 0.0 },
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 10, 0, 0),
                    };
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = "-UNUSED PARAMETR-",
                        Width = 250,
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush(Colors.Gray),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(10, 0.5, 0, 0),
                        TextAlignment = TextAlignment.Left,
                    };
                    TextBox textBox = new TextBox()
                    {
                        Width = 80,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 0),
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        FontSize = 12,
                        Padding = new Thickness(0, 1.5, 0, 0),
                        TextAlignment = TextAlignment.Center,
                        //IsEnabled = false,
                        Background = new SolidColorBrush(Colors.DarkGray),
                        Text = "0"
                    };
                    textBox.PreviewTextInput += TextBoxId_PreviewTextInputWithMinus;
                    stackPanelInner.Children.Add(textBlock); stackPanelInner.Children.Add(textBox);
                    stackPanelSubTarget.Children.Add(stackPanelInner);
                }
                #endregion
                #endregion
                #region Сборка элементов
                #region Фон
                MainRoot.Children.Add(background); Grid.SetRowSpan(background, 7); Grid.SetColumnSpan(background, 4);
                #endregion
                #region Основная панель
                MainRoot.Children.Add(mainBorder); Grid.SetRowSpan(mainBorder, 3); Grid.SetColumnSpan(mainBorder, 1);
                stackPanelMainId.Children.Add(textMainMain);
                stackPanelMainId.Children.Add(textIdMain);
                stackPanelEntryId.Children.Add(textBoxEntryId); stackPanelEntryId.Children.Add(buttonRefreshEntryId);
                stackPanelMainId.Children.Add(stackPanelEntryId);
                stackPanelMainId.Children.Add(textSourceType);
                stackPanelMainId.Children.Add(comboBoxType);
                stackPanelLockId.Children.Add(textId); stackPanelLockId.Children.Add(textBoxId); stackPanelLockId.Children.Add(textLock); stackPanelLockId.Children.Add(checkBoxId);
                stackPanelMainId.Children.Add(stackPanelLockId);
                stackPanelCopyNew.Children.Add(buttonCopy); stackPanelCopyNew.Children.Add(buttonNew);
                stackPanelMainId.Children.Add(stackPanelCopyNew);
                stackPanelDeleteSave.Children.Add(buttonDelete); stackPanelDeleteSave.Children.Add(buttonSave);
                stackPanelMainId.Children.Add(stackPanelDeleteSave);
                stackPanelMainId.Children.Add(buttonUpdateTempBase);
                wrapPanelMain.Children.Add(stackPanelMainId);
                MainRoot.Children.Add(wrapPanelMain); Grid.SetRow(wrapPanelMain, 0); Grid.SetRowSpan(wrapPanelMain, 3); Grid.SetColumnSpan(wrapPanelMain, 1);
                #endregion
                #region Панель DataGrid
                MainRoot.Children.Add(dgBorder); Grid.SetRow(dgBorder, 0); Grid.SetColumn(dgBorder, 1); Grid.SetRowSpan(dgBorder, 3); Grid.SetColumnSpan(dgBorder, 3);
                stackPanelNameSmart.Children.Add(textDGName); stackPanelNameSmart.Children.Add(textDGNameLocal); stackPanelNameSmart.Children.Add(textDGSmartAI);
                stackPanelDGUp.Children.Add(stackPanelNameSmart);
                stackPanelDGUp.Children.Add(buttonSQL); stackPanelDGUp.Children.Add(buttonSwitchSmart); stackPanelDGUp.Children.Add(creatureTextButton);
                stackPanelDG.Children.Add(stackPanelDGUp); stackPanelDG.Children.Add(dataGrid);
                wrapPanelDataGrid.Children.Add(stackPanelDG);
                MainRoot.Children.Add(wrapPanelDataGrid); Grid.SetRow(wrapPanelDataGrid, 0); Grid.SetColumn(wrapPanelDataGrid, 1); Grid.SetRowSpan(wrapPanelDataGrid, 3); Grid.SetColumnSpan(wrapPanelDataGrid, 3);
                #endregion
                #region Название скрипта
                border.MaxHeight = textBoxComment.Height + 15;
                MainRoot.Children.Add(border); Grid.SetRow(border, 3); Grid.SetRowSpan(border, 1); Grid.SetColumnSpan(border, 4);
                wrapPanelScriptDescription.Children.Add(textBlockComment);
                wrapPanelScriptDescription.Children.Add(textBoxComment);
                MainRoot.Children.Add(wrapPanelScriptDescription); Grid.SetRow(wrapPanelScriptDescription, 3); Grid.SetRowSpan(wrapPanelScriptDescription, 1); Grid.SetColumnSpan(wrapPanelScriptDescription, 4);
                #endregion
                #region Панель редактор
                stackPanelEditor.Children.Add(textEditor); stackPanelEditor.Children.Add(inheritBehaviorButton);
                stackPanelSelectLink.Children.Add(textLinkToScript); stackPanelSelectLink.Children.Add(textBoxSelectLinkIndex);
                stackPanelSelectLink.Children.Add(linkButton);
                stackPanelSelectLink.Children.Add(textLinkToScriptPreserved); stackPanelSelectLink.Children.Add(textBoxSelectLinkPreserved);
                stackPanelEditor.Children.Add(stackPanelSelectLink);
                stackPanelEvent.Children.Add(textLinkToOnEvent); stackPanelEvent.Children.Add(comboBoxEvent); stackPanelEvent.Children.Add(textBoxEvent);
                stackPanelEditor.Children.Add(stackPanelEvent);
                stackPanelAction.Children.Add(textLinkToOnAction); stackPanelAction.Children.Add(comboBoxAction); stackPanelAction.Children.Add(textBoxAction);
                stackPanelEditor.Children.Add(stackPanelAction);
                stackPanelTarget.Children.Add(textLinkToAtTarget); stackPanelTarget.Children.Add(comboBoxTarget); stackPanelTarget.Children.Add(textBoxTarget);
                stackPanelEditor.Children.Add(stackPanelTarget);
                stackPanelChance.Children.Add(textChance); stackPanelChance.Children.Add(textBoxChance); stackPanelChance.Children.Add(chanceSlider);
                stackPanelEditor.Children.Add(stackPanelChance);
                stackPanelPhaseMask.Children.Add(textMask); stackPanelPhaseMask.Children.Add(textBoxMask); stackPanelPhaseMask.Children.Add(selectMaskButton);
                stackPanelEditor.Children.Add(stackPanelPhaseMask);
                stackPanelEventFlag.Children.Add(textFlag); stackPanelEventFlag.Children.Add(textBoxEventFlag); stackPanelEventFlag.Children.Add(selectFlagButton);
                stackPanelEditor.Children.Add(stackPanelEventFlag);

                wrapPanelEditor.Children.Add(stackPanelEditor);
                MainRoot.Children.Add(borderEditor); Grid.SetRow(borderEditor, 4); Grid.SetRowSpan(borderEditor, 3); Grid.SetColumnSpan(borderEditor, 2);
                MainRoot.Children.Add(wrapPanelEditor); Grid.SetRow(wrapPanelEditor, 4); Grid.SetRowSpan(wrapPanelEditor, 3); Grid.SetColumnSpan(wrapPanelEditor, 2);
                #endregion
                #region Панель "параметры"
                stackPanelParam.Children.Add(textParam);
                tabControl.Items.Add(tabItemEvent);
                tabControl.Items.Add(tabItemAction);
                tabControl.Items.Add(tabItemTarget);
                stackPanelParam.Children.Add(tabControl);
                wrapPanelParameters.Children.Add(stackPanelParam);
                tabItemEvent.Content = stackPanelSubEvent;
                tabItemAction.Content = stackPanelSubAction;
                tabItemTarget.Content = stackPanelSubTarget;
                MainRoot.Children.Add(borderParam); Grid.SetRow(borderParam, 4); Grid.SetColumn(borderParam, 2); Grid.SetRowSpan(borderParam, 3); Grid.SetColumnSpan(borderParam, 2);
                MainRoot.Children.Add(wrapPanelParameters); Grid.SetRow(wrapPanelParameters, 4); Grid.SetColumn(wrapPanelParameters, 2); Grid.SetRowSpan(wrapPanelParameters, 3); Grid.SetColumnSpan(wrapPanelParameters, 2);
                #endregion
                #endregion
                #endregion
                #region Подписка на события
                this.SizeChanged += MainWindow_SizeChanged;
                checkBoxId.Click += CheckBoxId_Checked;
                textBoxId.PreviewTextInput += TextBoxId_PreviewTextInput;
                textBoxEntryId.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxAction.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxAction.TextChanged += TextBoxAction_TextChanged;
                textBoxEvent.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxEvent.TextChanged += TextBoxEvent_TextChanged;
                textBoxTarget.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxTarget.TextChanged += TextBoxTarget_TextChanged;
                textBoxChance.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxChance.TextChanged += TextBoxChance_TextChanged;
                textBoxEventFlag.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                textBoxMask.PreviewTextInput += TextBoxId_PreviewTextInput_1;
                buttonRefreshEntryId.Click += ButtonRefreshEntryId_Click;
                copyContextDGName.Click += CopyContext_Click;
                copyContextDGNameLocal.Click += CopyContext_Click;
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                dataGrid.LoadingRow += DataGrid_LoadingRow;
                buttonSwitchSmart.Click += ButtonSwitchSmart_Click;
                linkButton.Click += LinkButton_Click;
                comboBoxEvent.SelectionChanged += ComboBox_SelectionChanged;
                comboBoxAction.SelectionChanged += ComboBox_SelectionChanged;
                comboBoxTarget.SelectionChanged += ComboBox_SelectionChanged;
                this.PreviewKeyUp += DataGrid_PreviewKeyDown;
                chanceSlider.ValueChanged += ChanceSlider_ValueChanged;
                selectMaskButton.Click += SelectMaskButton_Click;
                selectFlagButton.Click += SelectFlagButton_Click;
                buttonNew.Click += ButtonNew_Click;
                buttonNew.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonNew.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonSave.Click += ButtonSave_Click;
                buttonSave.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonSave.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonUpdateTempBase.Click += ButtonUpdateTempBase_Click;
                buttonUpdateTempBase.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonUpdateTempBase.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonCopy.Click += ButtonCopy_Click;
                buttonCopy.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonCopy.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonDelete.Click += ButtonDelete_Click;
                buttonDelete.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonDelete.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonSwitchSmart.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonSwitchSmart.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonSQL.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonSQL.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                buttonRefreshEntryId.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                buttonRefreshEntryId.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                inheritBehaviorButton.Click += InheritBehaviorButton_Click;
                buttonSQL.Click += ButtonSQL_Click;
                this.Closed += MainWindow_Closed;
                creatureTextButton.Click += CreatureTextButton_Click;
                creatureTextButton.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
                creatureTextButton.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp;
                #endregion
                #region Размеры окна
                MinWidth = Width = 1000;
                MinHeight = Height = 750;
                Width++;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Реализация событий после входа
        #region Основные кнопки
        /// <summary>
        /// Сохранение элементов из временной базы данных в постоянную
        /// </summary>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectionIndex = dataGrid.SelectedIndex;
                selIndex = selectionIndex;
                string id = textBoxEntryId.Text.ToString();
                string type = comboBoxType.SelectedIndex.ToString();
                #region Запрос
                string sql_delete = $"DELETE FROM `smart_scripts` WHERE `entryorguid`= {id} AND `source_type`= {type}";
                MySqlDataAdapter adapter_delete = new MySqlDataAdapter()
                {
                    InsertCommand = new MySqlCommand(sql_delete, conn),
                };
                adapter_delete.InsertCommand.ExecuteNonQuery();
                #endregion
                foreach (DataGridItem dataGridItem in dataGrid.Items)
                {
                    string sql_insert = $"INSERT INTO `smart_scripts` (`entryorguid`,`source_type`,`id`,`link`,`event_type`,`event_phase_mask`,`event_chance`,`event_flags`,`event_param1`,`event_param2`,`event_param3`,`event_param4`,`event_param5`,`action_type`,`action_param1`,`action_param2`,`action_param3`,`action_param4`,`action_param5`,`action_param6`,`target_type`,`target_param1`,`target_param2`,`target_param3`,`target_param4`,`target_x`,`target_y`,`target_z`,`target_o`,`comment`) VALUES  " +
                    $"({dataGridItem.source["entryorguid"]}, {dataGridItem.source["source_type"]},{dataGridItem.source["id"]}, {dataGridItem.source["link"]}, " +
                    $"{dataGridItem.source["event_type"]}, {dataGridItem.source["event_phase_mask"]}, {dataGridItem.source["event_chance"]}, {dataGridItem.source["event_flags"]}, " +
                    $"{dataGridItem.source["event_param1"]},{dataGridItem.source["event_param2"]}, {dataGridItem.source["event_param3"]}, {dataGridItem.source["event_param4"]},{dataGridItem.source["event_param5"]}, " +
                    $"{dataGridItem.source["action_type"]}, {dataGridItem.source["action_param1"]}, {dataGridItem.source["action_param2"]}, {dataGridItem.source["action_param3"]}, {dataGridItem.source["action_param4"]}, {dataGridItem.source["action_param5"]}, {dataGridItem.source["action_param6"]}, " +
                    $"{dataGridItem.source["target_type"]}, {dataGridItem.source["target_param1"]}, {dataGridItem.source["target_param2"]}, {dataGridItem.source["target_param3"]}, {dataGridItem.source["target_param4"]}, {dataGridItem.source["target_x"].ToString().Replace(',', '.')}, {dataGridItem.source["target_y"].ToString().Replace(',', '.')}, {dataGridItem.source["target_z"].ToString().Replace(',', '.')},  {dataGridItem.source["target_o"].ToString().Replace(',', '.')}, " +
                    $"\"{dataGridItem.source["comment"]}\")";
                    MySqlDataAdapter adapter_insert = new MySqlDataAdapter()
                    {
                        InsertCommand = new MySqlCommand(sql_insert, conn),
                    };
                    adapter_insert.InsertCommand.ExecuteNonQuery();
                }
                ButtonAutomationPeer peer = new ButtonAutomationPeer(buttonRefreshEntryId);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n\n" + ex.Source + "\n\n" + ex.HelpLink + "\n\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }

        }
        /// <summary>
        /// Создание нового элемента во временной базе данных при нажатии кнопки "Новый"
        /// </summary>
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int entryId = Convert.ToInt32(textBoxEntryId.Text);
                int type = comboBoxType.SelectedIndex;
                Dictionary<string, object> data = new Dictionary<string, object>();
                foreach (string name in smartScriptsColumnNames)
                {
                    data.Add(name, 0);
                }
                //int newId = dataGrid.Items.Cast<DataGridItem>().OrderBy(x => x.id).Last().id + 1;
                int[] array = dataGrid.Items.Cast<DataGridItem>().Select(x => x.id).OrderBy(x => x).ToArray();
                int newId = getMissingNo(array);
                data["comment"] = ("Script description ID - " + newId).ToString();
                data["id"] = Convert.ToInt32(newId);
                data["entryorguid"] = entryId;
                data["source_type"] = type;
                data["event_chance"] = 100;
                dataGrid.Items.Add(new DataGridItem() { id = Convert.ToInt32(data["id"]), link = Convert.ToInt32(data["link"]), phase = Convert.ToInt32(data["event_phase_mask"]), comment = data["comment"].ToString(), source = data });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        /// <summary>
        /// Срздание копии элемента при нажатии кнокпи "Копировать"
        /// </summary>
        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            int entryId = Convert.ToInt32(textBoxEntryId.Text);
            int type = comboBoxType.SelectedIndex;
            DataGridItem selItem = dataGrid.SelectedItem as DataGridItem;
            Dictionary<string, object> newSource = new Dictionary<string, object>(selItem.source);
            int[] array = dataGrid.Items.Cast<DataGridItem>().Select(x => x.id).OrderBy(x => x).ToArray();
            int newId = getMissingNo(array);
            newSource["comment"] = selItem.source["comment"] + " -- copy".ToString();
            newSource["id"] = Convert.ToInt32(newId);
            dataGrid.Items.Add(new DataGridItem() { id = Convert.ToInt32(newSource["id"]), link = Convert.ToInt32(newSource["link"]), phase = Convert.ToInt32(newSource["event_phase_mask"]), comment = newSource["comment"].ToString(), source = newSource });
        }
        /// <summary>
        /// Удалние элемента из временной базы при нажатии кнопки "Удалить"
        /// </summary>
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridItem item = dataGrid.SelectedItem as DataGridItem;
                int Id = item.id;
                dataGrid.Items.Remove(dataGrid.SelectedItem);
                if (dataGrid.Items.Count > 0)
                {
                    dataGrid.SelectedItem = dataGrid.Items[dataGrid.Items.Count - 1];
                }
                else { buttonSave.IsEnabled = true; lastId = Id; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        /// <summary>
        /// Формирование запрооса в текстовом формате при нажатии кнопки "Сгенерировать SQL"
        /// </summary>
        private void ButtonSQL_Click(object sender, RoutedEventArgs e)
        {
            if (sqlBox == null)
            {
                int sourceType = Convert.ToInt32(comboBoxType.SelectedIndex);
                string sourceString = "";
                switch (sourceType)
                {
                    case 0:
                        sourceString = $"UPDATE creature_template SET AIName=\"SmartAI\" WHERE entry=@ENTRY;\n";
                        break;
                    case 1:
                        sourceString = $"UPDATE gameobject_template SET AIName=\"SmartGameObjectAI\" WHERE entry=@ENTRY;\n";
                        break;
                    case 2:
                        sourceString = $"UPDATE areatrigger_scripts SET ScriptName= \"SmartTrigger\" WHERE entry=@ENTRY;\n";
                        break;
                    default:
                        sourceString = "";
                        break;
                }
                string comment = textBoxComment.Text.ToString();
                string sql = $"-- {textDGName.Text}\n" +
                             $"SET @ENTRY := {Convert.ToInt32(textBoxEntryId.Text)};\n" +
                             $"SET @SOURCETYPE := {sourceType};\n" +
                             sourceString + /*"\n" +*/
                             $"DELETE FROM `smart_scripts` WHERE `entryorguid`=@ENTRY AND `source_type`=@SOURCETYPE;\n" +
                             $"INSERT INTO `smart_scripts` (`entryorguid`,`source_type`,`id`,`link`,`event_type`,`event_phase_mask`,`event_chance`,`event_flags`,`event_param1`,`event_param2`,`event_param3`,`event_param4`,`event_param5`,`action_type`,`action_param1`,`action_param2`,`action_param3`,`action_param4`,`action_param5`,`action_param6`,`target_type`,`target_param1`,`target_param2`,`target_param3`,`target_param4`,`target_x`,`target_y`,`target_z`,`target_o`,`comment`) VALUES\n";

                string sql_second = "";
                foreach (DataGridItem dataGridItem in dataGrid.Items)
                {
                    sql_second += $"(@ENTRY,@SOURCETYPE,{ dataGridItem.source["id"]}," +
                    $"{ dataGridItem.source["link"] }," +
                    $"{ dataGridItem.source["event_type"] }," +
                    $"{ dataGridItem.source["event_phase_mask"] }," +
                    $"{ dataGridItem.source["event_chance"] }," +
                    $"{ dataGridItem.source["event_flags"] }," +
                    $"{ dataGridItem.source["event_param1"] }," +
                    $"{ dataGridItem.source["event_param2"] }," +
                    $"{ dataGridItem.source["event_param3"] }," +
                    $"{ dataGridItem.source["event_param4"]  }," +
                    $"{ dataGridItem.source["event_param5"] }," +
                    $"{ dataGridItem.source["action_type"]}," +
                    $"{ dataGridItem.source["action_param1"]}," +
                    $"{ dataGridItem.source["action_param2"]}," +
                    $"{ dataGridItem.source["action_param3"]}," +
                    $"{ dataGridItem.source["action_param4"] }," +
                    $"{ dataGridItem.source["action_param5"] }," +
                    $"{ dataGridItem.source["action_param6"] }," +
                    $"{ dataGridItem.source["target_type"] }," +
                    $"{ dataGridItem.source["target_param1"] }," +
                    $"{ dataGridItem.source["target_param2"]}," +
                    $"{ dataGridItem.source["target_param3"]}," +
                    $"{ dataGridItem.source["target_param4"] }," +
                    $"{ dataGridItem.source["target_x"].ToString().Replace(',', '.')  }," +
                    $"{ dataGridItem.source["target_y"].ToString().Replace(',', '.')  }," +
                    $"{ dataGridItem.source["target_z"].ToString().Replace(',', '.')  }," +
                    $"{ dataGridItem.source["target_o"].ToString().Replace(',', '.') }," +
                    $"\"{ dataGridItem.source["comment"]}\"" + (dataGrid.Items.IndexOf(dataGridItem) == dataGrid.Items.Count - 1 ? ");" : "),\n");
                }
                sqlBox = new sqlBox(sql + sql_second);
                sqlBox.Closed += SqlBox_Closed;
            }
        }
        /// <summary>
        /// Обновление временной базы элементов сетки данных
        /// </summary>
        private void ButtonUpdateTempBase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectionIndex = dataGrid.SelectedIndex;
                if (selectionIndex != -1)
                {
                    DataGridItem dataGridItem = dataGrid.SelectedItem as DataGridItem;
                    if (dataGridItem != null)
                    {
                        dataGridItem.source["entryorguid"] = Convert.ToInt32(textBoxEntryId.Text);
                        dataGridItem.source["source_type"] = Convert.ToInt32(comboBoxType.SelectedIndex);
                        dataGridItem.source["id"] = Convert.ToInt32(textBoxId.Text);
                        dataGridItem.source["link"] = Convert.ToInt32(textBoxSelectLinkIndex.Text);
                        dataGridItem.source["event_type"] = Convert.ToInt32(textBoxEvent.Text);
                        dataGridItem.source["event_phase_mask"] = Convert.ToInt32(textBoxMask.Text);
                        dataGridItem.source["event_chance"] = Convert.ToInt32(textBoxChance.Text);
                        dataGridItem.source["event_flags"] = Convert.ToInt32(textBoxEventFlag.Text);
                        dataGridItem.source["event_param1"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[0].Children[0]).Children[1]).Text);
                        dataGridItem.source["event_param2"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[0].Children[1]).Children[1]).Text);
                        dataGridItem.source["event_param3"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[0].Children[2]).Children[1]).Text);
                        dataGridItem.source["event_param4"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[0].Children[3]).Children[1]).Text);
                        dataGridItem.source["event_param5"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[0].Children[4]).Children[1]).Text);
                        dataGridItem.source["action_type"] = Convert.ToInt32(textBoxAction.Text);
                        dataGridItem.source["action_param1"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[0]).Children[1]).Text);
                        dataGridItem.source["action_param2"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[1]).Children[1]).Text);
                        dataGridItem.source["action_param3"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[2]).Children[1]).Text);
                        dataGridItem.source["action_param4"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[3]).Children[1]).Text);
                        dataGridItem.source["action_param5"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[4]).Children[1]).Text);
                        dataGridItem.source["action_param6"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[1].Children[5]).Children[1]).Text);
                        dataGridItem.source["target_type"] = Convert.ToInt32(textBoxTarget.Text);
                        dataGridItem.source["target_param1"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[2].Children[0]).Children[1]).Text);
                        dataGridItem.source["target_param2"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[2].Children[1]).Children[1]).Text);
                        dataGridItem.source["target_param3"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[2].Children[2]).Children[1]).Text);
                        dataGridItem.source["target_param4"] = Convert.ToInt32(((TextBox)((StackPanel)EAT[2].Children[3]).Children[1]).Text);
                        dataGridItem.source["target_x"] = (float)Convert.ToDouble(((TextBox)((StackPanel)EAT[2].Children[4]).Children[1]).Text.Replace(".", ","));
                        dataGridItem.source["target_y"] = (float)Convert.ToDouble(((TextBox)((StackPanel)EAT[2].Children[5]).Children[1]).Text.Replace(".", ","));
                        dataGridItem.source["target_z"] = (float)Convert.ToDouble(((TextBox)((StackPanel)EAT[2].Children[6]).Children[1]).Text.Replace(".", ","));
                        dataGridItem.source["target_o"] = (float)Convert.ToDouble(((TextBox)((StackPanel)EAT[2].Children[7]).Children[1]).Text.Replace(".", ","));
                        dataGridItem.source["comment"] = textBoxComment.Text.ToString();
                    }
                    #region Обновление DataGrid
                    dataGridItem.id = (int)dataGridItem.source["id"];
                    dataGridItem.link = (int)dataGridItem.source["link"];
                    dataGridItem.phase = (int)dataGridItem.source["event_phase_mask"];
                    dataGridItem.comment = (string)dataGridItem.source["comment"];
                    DataGridItem refItem = new DataGridItem() { id = (int)dataGridItem.source["id"], link = (int)dataGridItem.source["link"], phase = (int)dataGridItem.source["event_phase_mask"], comment = (string)dataGridItem.source["comment"], source = dataGridItem.source };
                    int index = dataGrid.SelectedIndex;
                    dataGrid.Items.RemoveAt(index);
                    dataGrid.Items.Insert(index, refItem);
                    #endregion
                    dataGrid.SelectedIndex = selectionIndex;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        #region Кнопка Creature Text
        private void CreatureTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (creatureTextWindow == null)
            {
                string entryId = textBoxEntryId.Text.ToString();
                string type = comboBoxType.SelectedIndex.ToString();
                if (type == "0")
                {
                    creatureTextWindow = new CreatureTextWindow(in conn, entryId);
                    creatureTextWindow.Closed += CreatureTextWindow_Closed;
                    creatureTextWindow.Show();
                    this.IsEnabled = false;
                }
                else
                {
                    creatureTextWindow = new CreatureTextWindow(in conn);
                    creatureTextWindow.Closed += CreatureTextWindow_Closed;
                    creatureTextWindow.Show();
                    this.IsEnabled = false;
                }
            }
        }
        private void CreatureTextWindow_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = true;
            creatureTextWindow = null;
        }
        #endregion
        #endregion
        #region Эффекты нажатия кнопок
        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            button.Effect = null;
        }
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            button.Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                BlurRadius = 20,
                ShadowDepth = 0,
            };
        }
        #endregion
        private void SqlBox_Closed(object sender, EventArgs e)
        {
            sqlBox window = sender as DarkWowSoft.sqlBox;
            sqlBox = null;
            window.Close();
        }
        private void TextBoxTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int number = Convert.ToInt32(tb.Text);
                comboBoxTarget.SelectedIndex = number;
                if (number > 30 || tb.Text.Length > 2)
                {
                    tb.Text = null;
                    comboBoxTarget.SelectedIndex = 0;
                }
            }
            catch { comboBoxTarget.SelectedIndex = 0; }
        }
        private void TextBoxEvent_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int number = Convert.ToInt32(tb.Text);
                comboBoxEvent.SelectedIndex = number;
                if (number > 82 || tb.Text.Length > 2)
                {
                    tb.Text = null;
                    comboBoxEvent.SelectedIndex = 0;
                }
            }
            catch { comboBoxEvent.SelectedIndex = 0; }
        }
        private void TextBoxAction_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int number = Convert.ToInt32(tb.Text);
                comboBoxAction.SelectedIndex = number;
                if (number > 146 || tb.Text.Length > 3)
                {
                    tb.Text = null;
                    comboBoxAction.SelectedIndex = 0;
                }
            }
            catch { comboBoxAction.SelectedIndex = 0; }
        }
        #region Собютия окна Flags
        private void SelectFlagButton_Click(object sender, RoutedEventArgs e)
        {
            if (flagWindow == null)
            {
                List<int> list = null;
                try
                {
                    int[] intArray = flags.Select(x => x.Item2).ToArray();
                    list = IntSeparator(Convert.ToInt32(textBoxEventFlag.Text), intArray);
                }
                catch { }

                flagWindow = new flagWindow(flags, image, list);
                flagWindow.СhoiceIsDone += FlagWindow_СhoiceIsDone;
                flagWindow.closedButton += FlagWindow_closedButton;
                flagWindow.Closed += FlagWindow_Closed;
            }
        }
        private void FlagWindow_Closed(object sender, EventArgs e)
        {
            flagWindow = null;
        }
        private void FlagWindow_closedButton(object sender, EventArgs e)
        {
            flagWindow.Close();
        }
        private void FlagWindow_СhoiceIsDone(object sender, EventArgs e)
        {
            textBoxEventFlag.Text = this.flagWindow.GetIdsSum().ToString();
            flagWindow.Close();
        }
        #endregion
        #region События окна Link
        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            int selected = dataGrid.SelectedIndex;
            if (selected != -1 && linkWindow == null)
            {
                int count = dataGrid.Items.Count;
                List<(int, string)> dataList = new List<(int, string)>();
                int j = 0;
                for (int i = 0; i < count; i++)
                {
                    if (i == selected) { continue; }
                    DataGridItem item = (DataGridItem)dataGrid.Items[i];
                    int id = item.id;
                    if (id == 0) { continue; }
                    dataList.Add((id, item.comment));
                    j++;
                }
                linkWindow = new linkWindow(dataList, image);
                linkWindow.СhoiceIsDone += LinkWindow_СhoiceIsDone;
                linkWindow.closedButton += LinkWindow_closedButton;
                linkWindow.Closed += LinkWindow_Closed;
            }
        }
        private void LinkWindow_Closed(object sender, EventArgs e)
        {
            linkWindow = null;
        }
        private void LinkWindow_closedButton(object sender, EventArgs e)
        {
            linkWindow.Close();
        }
        private void LinkWindow_СhoiceIsDone(object sender, EventArgs e)
        {
            linkIndex = this.linkWindow.GetSelectedId();
            textBoxSelectLinkIndex.Text = linkIndex.ToString();
            linkWindow.Close();
        }
        #endregion
        #region События окна Phase
        private void SelectMaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (phaseWindow == null)
            {
                List<int> list = null;
                try
                {
                    int[] intArray = flags.Select(x => x.Item2).ToArray();
                    list = IntSeparator(Convert.ToInt32(textBoxMask.Text), intArray);
                }
                catch { }
                phaseWindow = new phaseWindow(phases, image, list);
                phaseWindow.СhoiceIsDone += PhaseWindow_СhoiceIsDone;
                phaseWindow.closedButton += PhaseWindow_closedButton;
                phaseWindow.Closed += PhaseWindow_Closed;
            }
        }
        private void PhaseWindow_Closed(object sender, EventArgs e)
        {
            phaseWindow = null;
        }
        private void PhaseWindow_closedButton(object sender, EventArgs e)
        {
            phaseWindow.Close();
        }
        private void PhaseWindow_СhoiceIsDone(object sender, EventArgs e)
        {
            textBoxMask.Text = this.phaseWindow.GetIdsSum().ToString();
            phaseWindow.Close();
        }
        #endregion
        #region События окон Action
        private void ButtonEvent_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Button button = sender as Button;
                actionWindow window = eventWindow.Find(x => x.Item1.Equals((int)button.Tag)).Item2;
                window.СhoiceIsDone += Window_СhoiceIsDone;
                window.closedButton += Window_closedButton;
                if (window.IsLoaded)
                {
                    int[] intArray = window.dataArray.Select(x => x.Item2).ToArray();
                    int selInt = Convert.ToInt32(window.textBox.Text);
                    List<int> list = null;
                    if (window.selectionMode)
                    {
                        list = IntSeparator(selInt, intArray);
                    }
                    else
                    {
                        list = new List<int> { selInt };
                    }
                    window.reset(list);
                    window.Visibility = Visibility.Visible;
                }
                else { window.Show(); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Button button = sender as Button;
                actionWindow window = actionWindow.Find(x => x.Item1.Equals((int)button.Tag)).Item2;
                window.СhoiceIsDone += Window_СhoiceIsDone;
                window.closedButton += Window_closedButton;
                if (window.IsLoaded)
                {
                    int[] intArray = window.dataArray.Select(x => x.Item2).ToArray();
                    int selInt = Convert.ToInt32(window.textBox.Text);
                    List<int> list = null;
                    if (window.selectionMode)
                    {
                        list = IntSeparator(selInt, intArray);
                    }
                    else
                    {
                        list = new List<int> { selInt };
                    }
                    window.reset(list);
                    window.Visibility = Visibility.Visible;
                }
                else { window.Show(); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        private void Window_closedButton(object sender, EventArgs e)
        {
            actionWindow window = sender as actionWindow;
            window.Visibility = Visibility.Hidden;
        }
        private void Window_СhoiceIsDone(object sender, EventArgs e)
        {
            actionWindow window = sender as actionWindow;
            window.GetIdsSum();
            window.Visibility = Visibility.Hidden;
        }
        #endregion
        #region Кнопка "Позаимствовать поведение"
        private void InheritBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridItem item = dataGrid.SelectedItem as DataGridItem;
            if (item != null)
            {
                try
                {
                    this.IsEnabled = false;
                    inheritWindow = new inheritWindow(image, ref item, ref conn, inheritLastId, inheritLastST);
                    inheritWindow.Show();
                    inheritWindow.closedButton += InheritWindow_closedButton;
                    inheritWindow.СhoiceIsDone += InheritWindow_СhoiceIsDone;
                    inheritWindow.Closed += InheritWindow_Closed;
                }
                catch (Exception ex) { MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
        private void InheritWindow_closedButton(object sender, EventArgs e)
        {
            inheritWindow.Close();
        }
        private void InheritWindow_Closed(object sender, EventArgs e)
        {
            inheritLastId = inheritWindow.inheritLastId;
            inheritLastST = inheritWindow.inheritLastST;
            inheritWindow = null;
            this.IsEnabled = true;
        }
        private void InheritWindow_СhoiceIsDone(object sender, EventArgs e)
        {
            try
            {
                DataGridItem dataGridItem = dataGrid.SelectedItem as DataGridItem;
                Dictionary<string, object> source = inheritWindow.source;
                if (source != null)
                {
                    #region Обновление DataGrid
                    DataGridItem refItem = new DataGridItem() { id = dataGridItem.id, link = Convert.ToInt32(source["link"]), phase = Convert.ToInt32(source["event_phase_mask"]), comment = dataGridItem.comment, source = source };
                    int index = dataGrid.SelectedIndex;
                    dataGrid.Items.RemoveAt(index);
                    dataGrid.Items.Insert(index, refItem);
                    #endregion
                    dataGrid.SelectedIndex = index;
                    inheritWindow.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        #endregion
        private void TextBoxChance_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                int number = Convert.ToInt32(tb.Text);
                if (number <= 100 || number < 0)
                { chanceSlider.Value = Convert.ToInt32(tb.Text); }
                else { chanceSlider.Value = 0; }
            }
            catch { chanceSlider.Value = 0; }
        }
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Down)
            {
                if (dataGrid.SelectedIndex < dataGrid.Items.Count - 1)
                { dataGrid.SelectedIndex += 1; }
            }
            else if (e.Key == System.Windows.Input.Key.Up)
            {
                if (dataGrid.SelectedIndex != 0)
                { dataGrid.SelectedIndex -= 1; }
            }
        }
        private void ChanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            textBoxChance.Text = (Math.Round(slider.Value)).ToString();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            int index = comboBox.SelectedIndex;
            DataGridItem dataGridItem = dataGrid.SelectedItem as DataGridItem;
            if (dataGridItem != null)
            {
                #region Данные из базы
                Dictionary<string, object> source = dataGridItem.source;
                #endregion
                try
                {
                    switch (comboBox.Name)
                    {
                        case "Event":
                            #region Очитка кнопок и окон 
                            #region Закрытеи окон
                            foreach ((int, actionWindow) actionWindow in eventWindow)
                            {
                                try { actionWindow.Item2.Close(); } catch { }
                            }
                            #endregion
                            for (int i = 0; i < 5; i++)
                            {
                                try { ((StackPanel)EAT[0].Children[i]).Children.RemoveAt(2); } catch { }
                            }
                            eventWindow.Clear();
                            #endregion
                            textBoxEvent.Text = item.Tag.ToString();
                            #region Данные из базы
                            if (source != null)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    double data = -1;
                                    try { data = data = Math.Round(Convert.ToDouble(source["event_param" + (i + 1).ToString()]), 3); } catch { }
                                    TextBox textBox = (TextBox)((StackPanel)EAT[0].Children[i]).Children[1];
                                    textBox.Text = data.ToString();
                                    #region Данные имен
                                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[0].Children[i]).Children[0];
                                    string str = eventArray[index].Item2[i];
                                    if (str != null)
                                    {
                                        textBlock.Text = str;
                                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                                        //textBox.IsEnabled = true;
                                        textBox.Background = new SolidColorBrush(Colors.White);
                                    }
                                    else
                                    {
                                        textBlock.Text = "-UNUSED PARAMETR-";
                                        textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                                        //textBox.IsEnabled = false;
                                        textBox.Background = new SolidColorBrush(Colors.DarkGray);
                                    }
                                    #endregion
                                    #region Создание кнопкм
                                    #region Для кнопок
                                    int num = eventArray[index].Item1;
                                    IEnumerable<int> indexes = null;
                                    if (num != -1)
                                    {
                                        indexes = num.ToString().Select(x => int.Parse(x.ToString()));
                                    }
                                    #endregion
                                    if (indexes != null)
                                    {
                                        foreach (int indx in indexes)
                                        {
                                            if (i.Equals(indx - 1))
                                            {
                                                (string, int)[] dataArray = null;
                                                try { dataArray = eventButtonArray.ToList().Find(x => x.Item1.Equals(index)).Item2.ToList().Find(x => x.Item1.Equals(indx)).Item2; } catch { }
                                                if (dataArray != null)
                                                {
                                                    Button button = new Button()
                                                    {
                                                        Content = "...",
                                                        VerticalAlignment = VerticalAlignment.Center,
                                                        HorizontalAlignment = HorizontalAlignment.Left,
                                                        Width = 25,
                                                        Height = 20,
                                                        Margin = new Thickness(10, 0, 0, 0),
                                                        FontWeight = FontWeights.Bold,
                                                        FontSize = 12,
                                                        Tag = indx,
                                                    };
                                                    StackPanel stackPanel = (StackPanel)EAT[0].Children[i];
                                                    if (index != 8 && index != 31 && index != 34)
                                                    {
     
                                                    }
                                                    else
                                                    {
                                                        eventWindow.Add((indx, new DarkWowSoft.actionWindow(dataArray, image, ref textBox, false, new List<int> { Convert.ToInt32(textBox.Text) }, textBlock.Text)));
                                                    }
                                                    stackPanel.Children.Add(button);
                                                    button.Click += ButtonEvent_Click;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                            break;
                        case "Action":
                            #region Очитка кнопок и окон
                            foreach ((int, actionWindow) eventWindow in actionWindow)
                            {
                                try { eventWindow.Item2.Close(); } catch { }
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                try { ((StackPanel)EAT[1].Children[i]).Children.RemoveAt(2); } catch { }
                            }
                            actionWindow.Clear();
                            #endregion
                            #region Данные из базы
                            if (source != null)
                            {
                                #region Для кнопок
                                int num = actionArray[index].Item1;
                                IEnumerable<int> indexes = null;
                                if (num != -1)
                                {
                                    indexes = num.ToString().Select(x => int.Parse(x.ToString()));
                                }
                                #endregion
                                for (int i = 0; i < 6; i++)
                                {
                                    double data = -1;
                                    try { data = Math.Round(Convert.ToDouble(source["action_param" + (i + 1).ToString()]), 3); } catch { }
                                    TextBox textBox = (TextBox)((StackPanel)EAT[1].Children[i]).Children[1];
                                    textBox.Text = data.ToString();
                                    #region Данные имен
                                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[1].Children[i]).Children[0];
                                    string str = actionArray[index].Item2[i];
                                    if (str != null)
                                    {
                                        textBlock.Text = str;
                                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                                        textBox.Background = new SolidColorBrush(Colors.White);
                                    }
                                    else
                                    {
                                        textBlock.Text = "-UNUSED PARAMETR-";
                                        textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                                        textBox.Background = new SolidColorBrush(Colors.DarkGray);
                                    }
                                    #endregion
                                    #region Создание кнопкм
                                    if (indexes != null)
                                    {
                                        foreach (int indx in indexes)
                                        {
                                            if (i.Equals(indx - 1))
                                            {
                                                (string, int)[] dataArray = null;
                                                try { dataArray = actionButtonArray.ToList().Find(x => x.Item1.Equals(index)).Item2.ToList().Find(x => x.Item1.Equals(indx)).Item2; } catch { }
                                                if (dataArray != null)
                                                {
                                                    Button button = new Button()
                                                    {
                                                        Content = "...",
                                                        VerticalAlignment = VerticalAlignment.Center,
                                                        HorizontalAlignment = HorizontalAlignment.Left,
                                                        Width = 25,
                                                        Height = 20,
                                                        Margin = new Thickness(10, 0, 0, 0),
                                                        FontWeight = FontWeights.Bold,
                                                        FontSize = 12,
                                                        Tag = indx,
                                                    };
                                                    StackPanel stackPanel = (StackPanel)EAT[1].Children[i];
                                                    if (index != 12 && index != 58 && index != 8)
                                                    {
                                                        int[] intArray = dataArray.Select(x => x.Item2).ToArray();
                                                        int selInt = Convert.ToInt32(textBox.Text);
                                                        List<int> list = IntSeparator(selInt, intArray);
                                                        actionWindow.Add((indx, new DarkWowSoft.actionWindow(dataArray, image, ref textBox, true, list, textBlock.Text)));
                                                    }
                                                    else
                                                    {
                                                        actionWindow.Add((indx, new DarkWowSoft.actionWindow(dataArray, image, ref textBox, false, new List<int> { Convert.ToInt32(textBox.Text)}, textBlock.Text)));
                                                    }
                                                    stackPanel.Children.Add(button);
                                                    button.Click += ButtonAction_Click;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                            textBoxAction.Text = item.Tag.ToString();
                            break;
                        case "Target":
                            #region Данные из базы
                            if (source != null)
                            {
                                string[] names = new string[8] { "target_param1", "target_param2", "target_param3", "target_param4", "target_x", "target_y", "target_z", "target_o" };
                                for (int i = 0; i < 8; i++)
                                {
                                    double data = -1;
                                    try { data = Math.Round(Convert.ToDouble(source[names[i]]), 3); } catch { }
                                    TextBox textBox = (TextBox)((StackPanel)EAT[2].Children[i]).Children[1];
                                    textBox.Text = data.ToString().Replace(",", ".");
                                    textBox.Tag = data;

                                    #region Данные имен
                                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[2].Children[i]).Children[0];
                                    string str = targetArray[index, i];
                                    if (str != null)
                                    {
                                        textBlock.Text = str;
                                        textBlock.Foreground = new SolidColorBrush(Colors.Black);
                                        textBox.Background = new SolidColorBrush(Colors.White);
                                    }
                                    else
                                    {
                                        textBlock.Text = "-UNUSED PARAMETR-";
                                        textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                                        textBox.Background = new SolidColorBrush(Colors.DarkGray);
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                            textBoxTarget.Text = item.Tag.ToString();
                            break;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); comboBox.SelectedIndex = 0; }
            }
        }
        private void ButtonSwitchSmart_Click(object sender, RoutedEventArgs e)
        {
            int index = comboBoxType.SelectedIndex;
            string type = index.ToString();
            string entryId = textBoxEntryId.Text.ToString();
            if (type != null && entryId != null)
            {
                switch (type.ToString())
                {
                    case "0":
                        #region Запрос
                        string sql = "UPDATE creature_template SET AIName = 'SmartAI' WHERE entry = " + entryId;
                        MySqlDataAdapter adapter = new MySqlDataAdapter()
                        {
                            InsertCommand = new MySqlCommand(sql, conn),
                        };
                        adapter.InsertCommand.ExecuteNonQuery();
                        #endregion
                        break;
                    case "1":
                        #region Запрос
                        string sql_1 = "UPDATE gameobject_template SET AIName='SmartGameObjectAI' WHERE entry = " + entryId;
                        MySqlDataAdapter adapter_1 = new MySqlDataAdapter()
                        {
                            InsertCommand = new MySqlCommand(sql_1, conn),
                        };
                        adapter_1.InsertCommand.ExecuteNonQuery();
                        #endregion
                        break;
                    case "2":
                        #region Запрос
                        string sql_2 = "UPDATE areatrigger_scripts SET ScriptName='SmartTrigger' WHERE entry = " + entryId;
                        MySqlDataAdapter adapter_2 = new MySqlDataAdapter()
                        {
                            InsertCommand = new MySqlCommand(sql_2, conn),
                        };
                        adapter_2.InsertCommand.ExecuteNonQuery();
                        #endregion
                        break;
                }
            }
            try
            {
                if (type == "0" || type == "9")
                {
                    string sql = "SELECT * FROM world.creature_template WHERE entry = " + entryId;
                    string sql_local = "SELECT * FROM world.creature_template_locale WHERE entry = " + entryId + " AND locale = 'ruRU'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            textDGName.Text = rdr["name"].ToString();
                            textDGSmartAI.Text = rdr["AIName"].ToString() == "SmartAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                        }
                    }
                    rdr.Close();
                    MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                    MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                    if (rdr_local.HasRows)
                    {
                        while (rdr_local.Read())
                        {
                            textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                        }
                    }
                    rdr_local.Close();
                }
                else if (type == "1")
                {
                    string sql = "SELECT * FROM world.gameobject_template WHERE entry = " + entryId;
                    string sql_local = "SELECT * FROM world.gameobject_template_locale WHERE entry = " + entryId + " AND locale = 'ruRU'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            textDGName.Text = rdr["name"].ToString();
                            textDGSmartAI.Text = rdr["AIName"].ToString() == "SmartGameObjectAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                        }
                    }
                    rdr.Close();
                    MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                    MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                    if (rdr_local.HasRows)
                    {
                        while (rdr_local.Read())
                        {
                            textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                        }
                    }
                    rdr_local.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGridItem dataGridItem = ((DataGrid)sender).SelectedItem as DataGridItem;
            if (dataGridItem != null)
            {
                #region Разблокировка кнопок
                wrapPanelEditor.IsEnabled = true;
                wrapPanelParameters.IsEnabled = true;
                wrapPanelScriptDescription.IsEnabled = true;
                buttonUpdateTempBase.IsEnabled = true;
                buttonSave.IsEnabled = true;
                buttonCopy.IsEnabled = true;
                buttonDelete.IsEnabled = true;
                //textBoxId.IsEnabled = true;
                checkBoxId.IsEnabled = true;
                #endregion
                #region Обновление ComboBox-ов
                int index = comboBoxEvent.SelectedIndex; comboBoxEvent.SelectedIndex = 0 == index ? 1 : 0; comboBoxEvent.SelectedIndex = index;
                int index_1 = comboBoxAction.SelectedIndex; comboBoxAction.SelectedIndex = 0 == index_1 ? 1 : 0; comboBoxAction.SelectedIndex = index_1;
                int index_2 = comboBoxTarget.SelectedIndex; comboBoxTarget.SelectedIndex = 0 == index_2 ? 1 : 0; comboBoxTarget.SelectedIndex = index_2;
                #endregion
                #region Поле ID
                textBoxId.Text = dataGridItem.id.ToString();
                #endregion
                #region Заполнение полей Link
                textBoxComment.Text = dataGridItem.comment.ToString();
                textBoxSelectLinkIndex.Text = dataGridItem.link.ToString();
                foreach (DataGridItem item in dataGrid.Items)
                {
                    if (dataGridItem.id == item.link && item.link != 0)
                    {
                        textBoxSelectLinkPreserved.Text = item.id.ToString();
                        break;
                    }
                    else { textBoxSelectLinkPreserved.Text = "None"; }
                }
                #endregion
                #region Заполнение Event/Action/Target/Chance и т.д.
                try
                {
                    Dictionary<string, object> source = dataGridItem.source;
                    try { int eventType = Convert.ToInt32(source["event_type"]); comboBoxEvent.SelectedIndex = eventType; } catch { }
                    try { int actionType = Convert.ToInt32(source["action_type"]); comboBoxAction.SelectedIndex = actionType; } catch { }
                    try { int targetType = Convert.ToInt32(source["target_type"]); comboBoxTarget.SelectedIndex = targetType; } catch { }
                    try { int chanceType = Convert.ToInt32(source["event_chance"]); textBoxChance.Text = chanceType.ToString(); } catch { }
                    try { int eventPhaseType = Convert.ToInt32(source["event_phase_mask"]); textBoxMask.Text = eventPhaseType.ToString(); } catch { }
                    try { int eventFlagType = Convert.ToInt32(source["event_flags"]); textBoxEventFlag.Text = eventFlagType.ToString(); } catch { }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                #endregion
            }
            else
            {
                wrapPanelEditor.IsEnabled = false;
                wrapPanelParameters.IsEnabled = false;
                wrapPanelScriptDescription.IsEnabled = false;
                buttonUpdateTempBase.IsEnabled = false;
                buttonSave.IsEnabled = false;
                buttonCopy.IsEnabled = false;
                buttonDelete.IsEnabled = false;
                //textBoxId.IsEnabled = false;
                checkBoxId.IsEnabled = false;
            }
        }
        private void CopyContext_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            switch (menuItem.Tag.ToString())
            {
                case "DGname":
                    Clipboard.SetText(textDGName.Text);
                    break;
                case "DGnameLocal":
                    Clipboard.SetText(textDGNameLocal.Text);
                    break;
            }

        }
        private void ButtonRefreshEntryId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Очистка
                dataGrid.Items.Clear();
                #region Поля
                textBoxId.Text = "0";
                textBoxChance.Text = "0";
                textBoxMask.Text = "0";
                textBoxEventFlag.Text = "0";
                textBoxSelectLinkIndex.Text = "0";
                textBoxSelectLinkPreserved.Text = "0";
                textBoxEvent.Text = "0";
                textBoxAction.Text = "0";
                textBoxTarget.Text = "0";
                textBoxComment.Text = null;
                for (int i = 0; i < 5; i++)
                {
                    TextBox textBox = (TextBox)((StackPanel)EAT[0].Children[i]).Children[1];
                    textBox.Text = "0";
                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[0].Children[i]).Children[0];
                    textBlock.Text = "-UNUSED PARAMETR-";
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                    textBox.Background = new SolidColorBrush(Colors.DarkGray);
                }
                for (int i = 0; i < 6; i++)
                {
                    TextBox textBox = (TextBox)((StackPanel)EAT[1].Children[i]).Children[1];
                    textBox.Text = "0";
                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[1].Children[i]).Children[0];
                    textBlock.Text = "-UNUSED PARAMETR-";
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                    textBox.Background = new SolidColorBrush(Colors.DarkGray);
                    try { ((StackPanel)EAT[1].Children[i]).Children.RemoveAt(2); } catch { }
                }
                for (int i = 0; i < 8; i++)
                {
                    TextBox textBox = (TextBox)((StackPanel)EAT[2].Children[i]).Children[1];
                    textBox.Text = "0";
                    TextBlock textBlock = (TextBlock)((StackPanel)EAT[2].Children[i]).Children[0];
                    textBlock.Text = "-UNUSED PARAMETR-";
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkGray);
                    textBox.Background = new SolidColorBrush(Colors.DarkGray);
                }
                #endregion
                #region Свитки
                comboBoxAction.SelectedIndex = 0;
                comboBoxEvent.SelectedIndex = 0;
                comboBoxTarget.SelectedIndex = 0;
                #endregion
                #region Текст
                textDGSmartAI.Text = "Не найдено";
                textDGName.Text = "Не найдено";
                textDGNameLocal.Text = "Не найдено";
                #endregion
                #endregion
                string type = comboBoxType.SelectedIndex.ToString();
                string id = textBoxEntryId.Text.ToString();
                bool guidBonus = false;
                #region Блокрировка кнопок и полей
                buttonNew.IsEnabled = false;
                wrapPanelEditor.IsEnabled = false;
                wrapPanelParameters.IsEnabled = false;
                wrapPanelScriptDescription.IsEnabled = false;
                buttonUpdateTempBase.IsEnabled = false;
                #endregion
                if (id != "0")
                {
                    buttonNew.IsEnabled = true;
                    if (type != null && id != null)
                    {
                        try
                        {
                            if (type == "0" || type == "1" || type == "2") { buttonSwitchSmart.IsEnabled = true; }
                            else { buttonSwitchSmart.IsEnabled = false; }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                        try
                        {
                            string sql = $"SELECT * FROM world.smart_scripts WHERE entryorguid = {id} AND source_type = {type}";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    Dictionary<string, object> data = new Dictionary<string, object>();
                                    foreach (string name in smartScriptsColumnNames) { data.Add(name, rdr[name]); }
                                    dataGrid.Items.Add(new DataGridItem() { id = Convert.ToInt32(data["id"]), link = Convert.ToInt32(data["link"]), phase = Convert.ToInt32(data["event_phase_mask"]), comment = data["comment"].ToString(), source = data });
                                }
                            }
                            else
                            {
                                MessageBox.Show("Smart_scripts для данного EntryID не найдены!" + "\n" + "Вы можете создать новый.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            rdr.Close();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                        try
                        {
                            if (type == "0")
                            {

                                string sql = $"SELECT * FROM world.creature_template WHERE entry = {id}";
                                string sql_local = $"SELECT * FROM world.creature_template_locale WHERE entry = {id} AND locale = 'ruRU'";
                                if (id.Contains('-'))
                                {
                                    string newId = id.Replace("-", "");
                                    string _sql = $"SELECT id FROM world.creature WHERE guid = {newId}";
                                    MySqlCommand _cmd = new MySqlCommand(_sql, conn);
                                    MySqlDataReader _rdr = _cmd.ExecuteReader();
                                    guidBonus = true;
                                    if (_rdr.HasRows)
                                    {
                                        wrapPanelDataGrid.IsEnabled = true;
                                        while (_rdr.Read())
                                        {
                                            sql = $"SELECT * FROM world.creature_template WHERE entry = {_rdr["id"]}";
                                            sql_local = $"SELECT * FROM world.creature_template_locale WHERE entry = {_rdr["id"]} AND locale = 'ruRU'";
                                        }
                                    }
                                    else
                                    {
                                        wrapPanelDataGrid.IsEnabled = false;
                                    }
                                    _rdr.Close();
                                }
                                MySqlCommand cmd = new MySqlCommand(sql, conn);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                if (rdr.HasRows)
                                {
                                    wrapPanelDataGrid.IsEnabled = true;
                                    while (rdr.Read())
                                    {
                                        textDGName.Text = guidBonus ? "(GUID): " + rdr["name"].ToString() : rdr["name"].ToString();
                                        textDGSmartAI.Text = rdr["AIName"].ToString() == "SmartAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                                    }
                                }
                                else
                                {
                                    wrapPanelDataGrid.IsEnabled = false;
                                }
                                rdr.Close();
                                MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                                MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                                if (rdr_local.HasRows)
                                {
                                    while (rdr_local.Read())
                                    {
                                        textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                                    }
                                }
                                rdr_local.Close();
                            }
                            else if (type == "1")
                            {
                                string sql = "SELECT * FROM world.gameobject_template WHERE entry = " + id;
                                string sql_local = "SELECT * FROM world.gameobject_template_locale WHERE entry = " + id + " AND locale = 'ruRU'";
                                MySqlCommand cmd = new MySqlCommand(sql, conn);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                if (rdr.HasRows)
                                {
                                    wrapPanelDataGrid.IsEnabled = true;
                                    while (rdr.Read())
                                    {
                                        textDGName.Text = rdr["name"].ToString();
                                        textDGSmartAI.Text = rdr["AIName"].ToString() == "SmartGameObjectAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                                    }
                                }
                                else
                                {
                                    wrapPanelDataGrid.IsEnabled = false;
                                }
                                rdr.Close();
                                MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                                MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                                if (rdr_local.HasRows)
                                {
                                    while (rdr_local.Read())
                                    {
                                        textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                                    }
                                }
                                rdr_local.Close();
                            }
                            else if (type == "9")
                            {
                                string trimId = id.Remove(id.Length - 2);
                                string sql = $"SELECT * FROM world.creature_template WHERE entry = {trimId}";
                                MySqlCommand cmd = new MySqlCommand(sql, conn);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                if (rdr.HasRows)
                                {
                                    wrapPanelDataGrid.IsEnabled = true;
                                    while (rdr.Read())
                                    {
                                        textDGName.Text = rdr["name"].ToString();
                                        textDGSmartAI.Text = rdr["AIName"].ToString() == "SmartAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                                    }
                                    rdr.Close();
                                    string sql_local = "SELECT * FROM world.creature_template_locale WHERE entry = " + trimId + " AND locale = 'ruRU'";
                                    MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                                    MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                                    if (rdr_local.HasRows)
                                    {
                                        while (rdr_local.Read())
                                        {
                                            textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                                        }
                                    }
                                    rdr_local.Close();
                                }
                                else
                                {
                                    rdr.Close();
                                    string sql_second = $"SELECT * FROM world.gameobject_template WHERE entry = {trimId}";
                                    MySqlCommand cmd_second = new MySqlCommand(sql_second, conn);
                                    MySqlDataReader rdr_second = cmd_second.ExecuteReader();
                                    if (rdr_second.HasRows)
                                    {
                                        while (rdr_second.Read())
                                        {
                                            textDGName.Text = rdr_second["name"].ToString();
                                            textDGSmartAI.Text = rdr_second["AIName"].ToString() == "SmartGameObjectAI" ? "[SmartAi Активен]" : "[SmartAi Не активен]";
                                        }
                                    }
                                    rdr_second.Close();
                                    string sql_local = $"SELECT * FROM world.gameobject_template_locale WHERE entry = {trimId} AND locale = 'ruRU'";
                                    MySqlCommand cmd_local = new MySqlCommand(sql_local, conn);
                                    MySqlDataReader rdr_local = cmd_local.ExecuteReader();
                                    if (rdr_local.HasRows)
                                    {
                                        while (rdr_local.Read())
                                        {
                                            textDGNameLocal.Text = "[" + rdr_local["Name"].ToString() + "]";
                                        }
                                    }
                                    rdr_local.Close();
                                }

                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
                    }
                }
                if (selIndex != -1) { dataGrid.SelectedIndex = selIndex; selIndex = -1; }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                DataGridItem dataGridItem = e.Row.Item as DataGridItem;
                switch (dataGridItem.phase.ToString())
                //switch (dataGridItem.source["event_phase_mask"].ToString())
                {
                    case "1":
                        e.Row.Background = new SolidColorBrush(Colors.LightGreen);
                        break;
                    case "2":
                        e.Row.Background = new SolidColorBrush(Colors.Pink);
                        break;
                    case "4":
                        e.Row.Background = new SolidColorBrush(Colors.Turquoise);
                        break;
                    case "8":
                        e.Row.Background = new SolidColorBrush(Colors.MediumPurple);
                        break;
                    case "16":
                        e.Row.Background = new SolidColorBrush(Colors.LightYellow);
                        break;
                    case "32":
                        e.Row.Background = new SolidColorBrush(Colors.LightGray);
                        break;
                    case "64":
                        e.Row.Background = new SolidColorBrush(Colors.IndianRed);
                        break;
                    case "128":
                        e.Row.Background = new SolidColorBrush(Colors.RosyBrown);
                        break;
                    case "256":
                        e.Row.Background = new SolidColorBrush(Colors.DodgerBlue);
                        break;
                }
            }
            catch
            {
            }
        }
        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            var d = targetArray[1, 7];
            MessageBox.Show("Попу мыл?", "Слышь", MessageBoxButton.YesNo);
        }
        private void TextBoxId_PreviewTextInput_2(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string inputText = e.Text;
            string text = tb.Text;
            e.Handled = !IsTextAllowed(inputText);
            int number = -1;
            try { number = Convert.ToInt32(text); } catch { }
            if (text.Length > 3 || number > 100 || number < 0) { e.Handled = true; }
        }
        private void TextBoxId_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string inputText = e.Text;
            string text = tb.Text;
            e.Handled = !IsTextAllowed(inputText);
            if (text.Length > 1) { e.Handled = true; }
        }
        private void TextBoxId_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string inputText = e.Text;
            if (tb.Text.Contains("-")) { tb.Text = null; }
            e.Handled = !IsTextAllowed(inputText);
        }
        private void TextBoxId_PreviewTextInputWithMinus(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string inputText = e.Text;
            //if (tb.Text.Contains("-")) { tb.Text = null; }
            e.Handled = !IsTextAllowed(inputText);
        }
        private void CheckBoxId_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            switch (check.IsChecked)
            {
                case true:
                    textBoxId.IsEnabled = false;
                    break;
                case false:
                    textBoxId.IsEnabled = true;
                    break;
            }
        }
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            textBoxComment.Width = this.ActualWidth - textBlockComment.ActualWidth - 70;
            textBoxEntryId.Width = wrapPanelMain.ActualWidth - 50;
            commentColumn.Width = wrapPanelDataGrid.ActualWidth;/*- commentColumn.ActualWidth*/
            dataGrid.Height = ActualHeight * 0.32;
            inheritBehaviorButton.Width = ActualWidth * 0.4;

            double columnLength = 0; for (int i = 0; i < 3; i++) { columnLength += dataGrid.Columns[i].Width.Value; }
            dataGrid.Columns[3].Width = ActualWidth * 0.72 - columnLength;
            dataGrid.Width = ActualWidth * 0.72;

            tabControl.Width = ActualWidth / 2.2;
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            conn.Close();
        }
        #endregion
        #region Вспомогательные классы и функции
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        public static BitmapImage GetImageMultiextension(string path)
        {
            BitmapImage image = null;
            string[] extexnsions = new string[3] { ".png", ".jpeg", ".jpg" };
            foreach (string ext in extexnsions)
            {
                try
                {
                    image = new BitmapImage(new Uri(path + ext, System.UriKind.Relative) as System.Uri);
                    double heigth = image.Height;
                    break;
                }
                catch { }
            }
            return image;
        }
        public class DataGridItem
        {
            public int id { get; set; }
            public int phase { get; set; }
            public int link { get; set; }
            public string comment { get; set; }
            public Dictionary<string, object> source { get; set; }
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                e.Handled = true;
                return; // do not call the base class method OnPreviewKeyDown()
            }
            base.OnPreviewKeyDown(e);
        }
        public static List<int> IntSeparator(int unseparatedInt, int[] array)
        {
            List<int> result = new List<int>();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int integer = array[i];
                int num = unseparatedInt - integer;
                if (num == 0) { result.Add(integer); break; }
                else if (num < 0) { continue; }
                else if (num > 0) { result.Add(integer); result.AddRange(IntSeparator(num, array.Take(array.Length - 1).ToArray())); break; }
            }
            return result;
        }
        public static int getMissingNo(int[] arr)
        {
            if (arr.Length == 0) { return 0; }
            if (arr.Min() == 0)
            {
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    if (arr[i + 1] - arr[i] > 1)
                    {
                        return arr[i] + 1;
                    }
                }
            }
            else
            {
                return 0;
            }
            return arr.Last() + 1;
        }
        #endregion
    }
}
