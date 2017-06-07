using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTranslator
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DropDownTemplate { get; set; }
        public DataTemplate SelectedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ComboBoxItem comboBoxItem = GetVisualParent<ComboBoxItem>(container);
            if (comboBoxItem != null)
            {
                return DropDownTemplate;
            }
            return SelectedTemplate;
        }

        private static T GetVisualParent<T>(object childObject) where T : Visual
        {
            var child = childObject as DependencyObject;
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }
    }
}
