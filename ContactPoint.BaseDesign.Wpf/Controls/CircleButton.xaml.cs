using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContactPoint.BaseDesign.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for CircleButton.xaml
    /// </summary>
    public partial class CircleButton
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(FrameworkElement), typeof(CircleButton), new PropertyMetadata(null));

        public FrameworkElement Icon
        {
            get { return (FrameworkElement)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Text { get; set; }

        public CircleButton()
        {
            InitializeComponent();
        }
    }
}
