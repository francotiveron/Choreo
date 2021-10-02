using Choreo.Input;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotorSettingsPage.xaml
    /// </summary>
    public partial class MotorPage : UserControl {
        public MotorPage() {
            InitializeComponent();
            FocusManager.AddGotFocusHandler(this, Focus);
            ConfigSel.Items = Storage.LoadAxisConfigurationsNames();
        }

        bool? _isGroup = null;
        bool IsGroup
        {
            get
            {
                if (_isGroup is null)
                {
                    _isGroup = DataContext is Group;
                }
                return _isGroup.Value;
            }
        }

        Grid Grid => IsGroup ? EditableElementsGridG : EditableElementsGrid;
        DataItemUI AxisNameUI => IsGroup ? AxisNameG : AxisName;
        DataItemUI SetPositionUI => IsGroup ? SetPositionG : SetPosition;

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => FocusManager.SetFocusedElement(Grid, SetPositionUI);
        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui) {
                if (diui == AxisNameUI) {
                    NumPad.DataItem = null;
                    AlNumPad.DataItem = diui;
                }
                else {
                    NumPad.DataItem = diui;
                    AlNumPad.DataItem = null;
                }
            }
            e.Handled = true;
        }
        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(Grid, e.DataItem.Navigate(e.Name));

        private void AlNumPad_AlNumEvent(object sender, Input.AlphaNumericPad.AlNumEventArgs e) {
            FocusManager.SetFocusedElement(Grid, null);
            AlNumPad.DataItem = null;

            if (e.Name != "ENTER") return;

            #region Save Axis Configuration
            bool IsValid(string s)
            {
                Regex r = new Regex("^[a-zA-Z0-9\\s]+$");
                return r.IsMatch(s);
            }

            if (str != null && IsValid(str.StrVal))
            {
                try
                {
                    var config = new AxisConfiguration(str.StrVal, (Axis)DataContext);
                    Storage.Save(config);
                }
                finally
                {
                    str = null;
                    ConfigSel.Items = Storage.LoadAxisConfigurationsNames();
                }
            }
            #endregion
        }

        private void SaveNewPositionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Log.OkCancel("Click OK to Confirm", "Save New Position"))
                Plc.Calibrate(DataContext as Axis);
        }

        class Str : IStrVal
        {
            public bool IsPassword => false;
            public string StrVal { get; set; }
        }

        Str str;

        private void ConfigSel_CfgSelEvent(object sender, Input.ConfigSelector.CfgSelEventArgs e)
        {
            if (e != null && !string.IsNullOrWhiteSpace(e.Name))
            {
                //Load or Delete
                if (e.Name[0] == '-')
                {
                    //Delete
                    string name = e.Name.Substring(1);
                    if (Log.OkCancel($"Delete Axis Configuration {name}?", "Please Confirm")) {
                        Storage.DeleteAxisConfiguration(name);
                        ConfigSel.Items = Storage.LoadAxisConfigurationsNames();
                    }
                }
                else
                {
                    //Load
                    var config = new AxisConfiguration(e.Name);
                    Storage.Load(config);
                    config.Download((Axis)DataContext);
                }
                return;
            }
            //Save
            str = new Str();
            NumPad.DataItem = null;
            AlNumPad.DataItem = str;
        }
    }
}
