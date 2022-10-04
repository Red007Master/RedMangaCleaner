using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.CustomControls
{
    public class EditableObjectSelector : Control
    {
        static EditableObjectSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableObjectSelector), new FrameworkPropertyMetadata(typeof(EditableObjectSelector)));
        }
    }
}
