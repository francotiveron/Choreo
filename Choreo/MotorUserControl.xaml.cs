﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotorUserControl.xaml
    /// </summary>
    public partial class MotorUserControl : UserControl
    {
        public MotorUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var uc = (MotorUserControl)sender;
            var row = (int)uc.GetValue(Grid.RowProperty);
            var col = (int)uc.GetValue(Grid.ColumnProperty);
            var index = row * 8 + col;
            DataContext = VM.Motors[index];
        }
    }
}
