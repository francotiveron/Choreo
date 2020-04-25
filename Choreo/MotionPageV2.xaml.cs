using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotionPageV2.xaml
    /// </summary>
    public partial class MotionPageV2 : UserControl {
        public MotionPageV2() {
            InitializeComponent();
            InitializeCheckGrids();
            FocusManager.AddGotFocusHandler(this, Focus);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (((Motion)DataContext).Hook is Group) HookGrid.Children.Remove(AxisGroup);
            FocusManager.SetFocusedElement(EditableElementsGrid, Velocity);
        }

        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui)
                NumPad.DataItem = diui;
            e.Handled = true;
        }

        private void InitializeCheckGrids() {
            int? motorHook = null, groupHook = null;

            switch (VM.Motion.Hook) {
                case Motor m:
                    motorHook = m.Index;
                    VM.Motion.Motors[m.Index] = true;
                    break;
                case Group g:
                    groupHook = g.Index;
                    VM.Motion.Groups[g.Index] = true;
                    break;
            }
            InitializeMotorsCheckGrid(motorHook);
            InitializeGroupsCheckGrid(groupHook);
        }

        private void InitializeMotorsCheckGrid(int? hook) {
            MotorsCheckGrid.Children.Clear();
            for (int i = 0; i < 16; i++) {
                var cb = CreateCheckBox(VM.Motors[i], i, $"Motors[{i}]", i == hook);
                cb.Tag = "Motor";
                MotorsCheckGrid.Children.Add(cb);
            }
        }

        private void InitializeGroupsCheckGrid(int? hook) {
            GroupsCheckGrid.Children.Clear();
            for (int i = 0; i < 8; i++) {
                var cb = CreateCheckBox(VM.Groups[i], i, $"Groups[{i}]", i == hook);
                cb.Tag = "Group";
                GroupsCheckGrid.Children.Add(cb);
            }
        }

        CheckBox CreateCheckBox(object dc, int i, string binding, bool disabled) {
            var cb = new CheckBox();
            cb.VerticalContentAlignment = VerticalAlignment.Center;
            cb.HorizontalAlignment = HorizontalAlignment.Center;
            var b = new Binding("Name");
            b.Source = dc;
            cb.SetBinding(CheckBox.ContentProperty, b);
            cb.SetValue(Grid.RowProperty, i % 4);
            cb.SetValue(Grid.ColumnProperty, i / 4);
            b = new Binding(binding);
            cb.SetBinding(CheckBox.IsCheckedProperty, b);
            cb.IsEnabled = !disabled;
            cb.Click += Cb_Click;
            return cb;
        }

        private void Cb_Click(object sender, RoutedEventArgs e) {
            Control c = null;
            switch(((CheckBox)sender).Tag) {
                case "Motor":
                    c = MaG.Motors;
                    break;
                case "Group":
                    c = MaG.Groups;
                    break;
            }
            c.GetBindingExpression(ContentProperty).UpdateTarget();
        }

        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, e.DataItem.Navigate(e.Name));

        private void Button_Click(object sender, RoutedEventArgs e) {
            Relative = sender == MoveRelativeButton;
            FocusManager.SetFocusedElement(EditableElementsGrid, Relative ? RelativeSetpoint : AbsoluteSetPoint);
        }

        bool Relative {
            get {
                var motion = DataContext as Motion;
                return motion.Relative;
            }
            set {
                var motion = DataContext as Motion;
                motion.Relative = value;
            }
        }
    }
}
