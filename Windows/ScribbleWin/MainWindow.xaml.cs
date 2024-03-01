using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Scribbles.Drawing.EType;

namespace Scribbles;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class ScribbleWin : Window {
   public ScribbleWin () {
      Cursor = Cursors.Arrow;
      InitializeComponent ();
   }

   public InkControl InkControl => mInkControl;

   private void OnEraserClick (object sender, RoutedEventArgs e) {
      Window thickness = new EraserProperties () { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
      thickness.Show ();
   }

   private void OnPenClick (object sender, RoutedEventArgs e)
      => InkControl.Shape (DOODLE);

   private void OnPenRightClick (object sender, MouseButtonEventArgs e) {
      Window penOptions = new PenProperties () { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
      penOptions.Show ();
   }

   private void OnShapeClick (object sender, RoutedEventArgs e)
      => InkControl.Shape (((Button)sender).Name switch {
         "mRect" => RECT, "mRectFilled" => FILLEDRECT,
         "mCircle" => CIRCLE1, "mArc" => ARC,
         "mLine" => LINE, "mCLine" => CONNECTEDLINE,
         "mEllipse" => ELLIPSE,
         _ => throw new NotImplementedException ()
      });

   private void OnUndo (object sender, RoutedEventArgs e)
      => InkControl?.Undo ();

   private void OnRedo (object sender, RoutedEventArgs e)
      => InkControl?.Redo ();

   private void OnSave (object sender, RoutedEventArgs e) {
      SaveFileDialog saveFile = new () {
         CheckPathExists = true, InitialDirectory = "C:\\etc",
         Filter = $"BinaryFiles |*.bin", AddExtension = true, DefaultExt = ".bin"
      };
      if (saveFile.ShowDialog () == true)
         try {
            InkControl?.Save (saveFile.FileName, saveFile.FilterIndex);
         } catch (NotImplementedException) { MessageBox.Show ("File not saved."); }
   }

   private void OnOpen (object sender, RoutedEventArgs e) {
      OpenFileDialog openFile = new () {
         CheckFileExists = true, CheckPathExists = true, Multiselect = false,
         InitialDirectory = "C:\\etc", Filter = $"BinaryFiles |*.bin", DefaultExt = ".bin"
      };
      if (openFile.ShowDialog () == true)
         try {
            mInkControl.Open (openFile.FileName, openFile.FilterIndex);
         } catch (Exception) { MessageBox.Show ("Couldn't Open file. Unknown FileFormat."); }
   }
}