using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeCharming.Metro.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BeCharming.Metro
{
    public class VariableGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var viewModel = item as ShareTarget;

            element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, viewModel.Width);
            element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.Height);
            element.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
            element.SetValue(VerticalContentAlignmentProperty, VerticalAlignment.Stretch);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
