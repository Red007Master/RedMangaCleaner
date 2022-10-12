using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.CustomControls
{
    public class TextWithCheckBox : Control
    {
        static TextWithCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextWithCheckBox), new FrameworkPropertyMetadata(typeof(TextWithCheckBox)));
        }
    }
}
