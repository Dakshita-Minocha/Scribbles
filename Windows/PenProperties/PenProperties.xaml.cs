using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Scribbles; 

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
public partial class PenProperties : Window {
   public PenProperties ()
      => InitializeComponent ();

   ScribbleWin mOwner => (ScribbleWin)Owner;

   private void OnChangeColour (object sender, RoutedEventArgs e) {
      mOwner.InkControl.PenColor = ((Button)sender).Name[4..] switch {
         "Pink" => Brushes.HotPink, "Red" => Brushes.IndianRed,
         "Blue" => Brushes.Indigo, "White" => Brushes.White, _ => Brushes.White
      };
      Close ();
   }
   private void OnSizeClick (object sender, RoutedEventArgs e) {
      mOwner.InkControl.PenThickness = double.Parse (((Button)sender).Name[5..]);
      Close ();
   }
}
