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
using static Ohana3DS_Rebirth.Ohana.RenderBase;

namespace GUI
{
    /// <summary>
    /// Interaction logic for TextureDetails.xaml
    /// </summary>
    public partial class TextureDetails : UserControl
    {
        public TextureDetails(OTexture texture)
        {
            InitializeComponent();
            this.DataContext = texture;
        }
    }
}
