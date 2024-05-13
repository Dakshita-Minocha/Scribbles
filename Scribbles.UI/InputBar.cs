using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace Scribbles;

public class InputBar : StackPanel {
   #region Constructors ---------------------------------------------
   InputBar () { }
   public InputBar (Widget widget) {
      mWid = widget;
      var shapeType = mWid.GetType ();
      Orientation = Orientation.Horizontal;
      VerticalAlignment = System.Windows.VerticalAlignment.Top;
      var propInfo = shapeType.GetProperties ();
      foreach (var prop in propInfo) {
         Children.Add (new Label () { Name = prop.Name, Content = prop.Name });
         var box = new TextBox () { Name = prop.Name, Width = 50, Height = 20, Background = Brushes.White, DataContext = prop };
         var binding = new Binding (prop.Name) { Source = mWid, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
         box.SetBinding (TextBox.TextProperty, binding);
         box.KeyDown += OnKeyDown;
         Children.Add (box);
      }
   }
   #endregion

   #region Properties -----------------------------------------------
   public static InputBar Empty => new () {
      Orientation = Orientation.Horizontal,
      VerticalAlignment = System.Windows.VerticalAlignment.Top
   };
   #endregion

   #region Methods --------------------------------------------------
   void OnKeyDown (object sender, KeyEventArgs e) {
      if (e.Key is not Key.Enter || mWid is null) return;
      mWid.CreateNew ();
   }
   #endregion

   #region Private Data ---------------------------------------------
   Widget? mWid;
   #endregion
}
