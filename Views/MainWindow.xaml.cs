using System.Windows;
using System.Windows.Interactivity;

namespace AutoRingSRS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Interaction.GetBehaviors(this);
            InitializeComponent();
        }
    }
}
