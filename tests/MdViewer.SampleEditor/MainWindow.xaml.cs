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

namespace MdViewer.SampleEditor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Editor.Text = @"# Hello, markdown

Try enter **something** here!
";
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e) {
            MdViewer.Content = Editor.Text;
        }
    }
}