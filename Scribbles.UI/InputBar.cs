using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Scribbles;
public class InputBar : StackPanel {
   public InputBar (Widget widget) {
      var shapeType = widget.GetType ();
      Orientation = Orientation.Horizontal;
      VerticalAlignment = System.Windows.VerticalAlignment.Top;
      var propInfo = shapeType.GetProperties ();
      foreach (var prop in propInfo) {
         Children.Add (new Label () { Name = prop.Name, Content = prop.Name });
         var box = new TextBox () { Name = prop.Name, Width = 50, Height = 20, Background = Brushes.White };
         var binding = new Binding (prop.Name) { Source = widget };
         DataContext = widget;
         box.SetBinding (TextBox.TextProperty, binding);
         Children.Add (box);
      }
   }

   InputBar () { }

   public static InputBar Empty => new () {
      Orientation = Orientation.Horizontal,
      VerticalAlignment = System.Windows.VerticalAlignment.Top
   };
}
