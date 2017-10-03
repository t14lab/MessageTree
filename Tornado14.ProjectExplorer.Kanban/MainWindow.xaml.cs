using System;
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

namespace Tornado14.ProjectExplorer.Kanban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Point _anchorPoint;
        Point _currentPoint;
        bool _isInDrag;
        private readonly TranslateTransform _transform = new TranslateTransform();

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isInDrag) return;
            var element = sender as FrameworkElement;
            if (element != null) element.ReleaseMouseCapture();
            _isInDrag = false;
            e.Handled = true;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _anchorPoint = e.GetPosition(null);
            if (element != null) element.CaptureMouse();
            _isInDrag = true;
            e.Handled = true;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isInDrag) return;
            _currentPoint = e.GetPosition(null);

            _transform.X += _currentPoint.X - _anchorPoint.X;
            _transform.Y += (_currentPoint.Y - _anchorPoint.Y);
            RenderTransform = _transform;
            _anchorPoint = _currentPoint;
        }
    }
}
