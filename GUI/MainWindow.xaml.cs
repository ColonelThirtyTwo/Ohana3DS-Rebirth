using Microsoft.Win32;
using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Containers;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OContainer RootContainer;

        public MainWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            if (dialog.ShowDialog() != true)
                return;

            FileIO.file file;
            try {
                file = FileIO.load(dialog.FileName);
            } catch(IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(file.type != FileIO.formatType.container)
            {
                MessageBox.Show(this, "Could not load file: not a container", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RootContainer = (OContainer)file.data;
            UpdateUI();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RootContainer = null;
            UpdateUI();
        }

        private void UpdateUI()
        {
            Viewport.Children.Clear();
            var viewport_label = new Label();
            viewport_label.VerticalAlignment = VerticalAlignment.Center;
            viewport_label.HorizontalAlignment = HorizontalAlignment.Center;

            if (RootContainer != null)
            {
                ArchiveContent.Items.Clear();
                foreach (var item in RootContainer.content)
                {
                    ArchiveContent.Items.Add(item.name);
                }
                CloseMenuItem.IsEnabled = true;
                ArchiveContent.IsEnabled = true;

                

                viewport_label.Content = "Select an item in the container to open (TODO: actually implement that)";
                
            }
            else
            {
                ArchiveContent.Items.Clear();
                CloseMenuItem.IsEnabled = false;
                ArchiveContent.IsEnabled = false;
                viewport_label.Content = "Open a file to start";
            }

            Viewport.Children.Add(viewport_label);
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
