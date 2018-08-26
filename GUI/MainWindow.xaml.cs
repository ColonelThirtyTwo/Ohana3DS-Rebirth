using Microsoft.Win32;
using Ohana3DS_Rebirth.Ohana;
using Ohana3DS_Rebirth.Ohana.Containers;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Ohana3DS_Rebirth.Ohana.RenderBase;

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
            CloseContainer();
        }

        /// <summary>
        /// Opens a container object for viewing, replacing an existing open container.
        /// </summary>
        /// <param name="container"></param>
        public void OpenContainer(OContainer container, string name="Root")
        {
            SetVieportLabel("Double click a container entry to open it");
            RootContainer = container;

            ArchiveContent.Items.Clear();
            var root_item = new TreeViewItem();
            root_item.Header = "Root";
            root_item.IsExpanded = true;

            PopulateContainerEntry(container, root_item);
            ArchiveContent.Items.Add(root_item);

            CloseMenuItem.IsEnabled = true;
            ArchiveContent.IsEnabled = true;
        }

        private void PopulateContainerEntry(OContainer container, TreeViewItem parent)
        {
            foreach (var entry in RootContainer.content)
            {
                var item = new TreeViewItem();
                item.Header = entry.name;
                item.MouseDoubleClick += (object sender, MouseButtonEventArgs e) => {
                    OpenContainerEntry(container, entry, item);
                };

                parent.Items.Add(item);
            }
        }

        /// <summary>
        /// Opens a texture in the viewport.
        /// 
        /// Does not affect the currently loaded container tree.
        /// </summary>
        /// <param name="texture">Texture to view</param>
        public void ViewTexture(OTexture texture)
        {
            Viewport.Children.Clear();
            var viewport_control = new TextureView(texture);
            Viewport.Children.Add(viewport_control);
        }

        /// <summary>
        /// Closes the container (if any) and clears the viewport.
        /// </summary>
        public void CloseContainer()
        {
            SetVieportLabel("No file opened");
            RootContainer = null;
            ArchiveContent.Items.Clear();
            CloseMenuItem.IsEnabled = false;
            ArchiveContent.IsEnabled = false;
        }

        private void SetVieportLabel(string contents)
        {
            Viewport.Children.Clear();
            var viewport_label = new Label()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = contents
            };
            Viewport.Children.Add(viewport_label);
        }

        private void OpenContainerEntry(OContainer container, OContainer.fileEntry entry, TreeViewItem item)
        {
            FileIO.file file;
            try
            {
                var stream = new MemoryStream(container.Load(entry));
                file = FileIO.load(stream);
            }
            catch (IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ParseException err)
            {
                MessageBox.Show(this, "Could not parse file.\n" + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (file.type == FileIO.formatType.container)
            {
                PopulateContainerEntry((OContainer)file.data, item);
            }
            else if (file.type == FileIO.formatType.image)
            {
                ViewTexture((OTexture)file.data);
            }
            else
            {
                MessageBox.Show(this, "Could not load file: unsupported file type: "+file.type, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            if (dialog.ShowDialog() != true)
                return;

            FileIO.file file;
            try
            {
                file = FileIO.load(dialog.FileName);
            }
            catch (IOException err)
            {
                MessageBox.Show(this, "Could not load file: " + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (ParseException err)
            {
                MessageBox.Show(this, "Could not parse file.\n" + err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (file.type == FileIO.formatType.container)
            {
                OpenContainer((OContainer)file.data);
            }
            else if (file.type == FileIO.formatType.image)
            {
                ViewTexture((OTexture)file.data);
            }
            else
            {
                MessageBox.Show(this, "Could not load file: unsupported file type", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CloseContainer();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
