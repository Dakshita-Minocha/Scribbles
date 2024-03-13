using System.Windows;
using System.Windows.Controls;
namespace Scribbles;

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
public partial class EraserProperties : Window {
   #region Constructors ---------------------------------------------
   public EraserProperties ()
   => InitializeComponent ();
   #endregion

   #region Implementation -------------------------------------------
   ScribbleWin mOwner => (ScribbleWin)Owner;

   void OnSizeClick (object sender, RoutedEventArgs e) {
      double thickness = double.Parse (((Button)sender).Name[5..]);
      mOwner.InkControl.EraserThickness = thickness;
      mOwner.InkControl.Erase ();
      Close ();
   }
   #endregion
}
