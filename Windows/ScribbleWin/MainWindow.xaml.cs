using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Scribbles.Drawing.EType;
namespace Scribbles;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class ScribbleWin : Window {
   #region Constructors ---------------------------------------------
   public ScribbleWin () {
      Cursor = Cursors.Arrow;
      InitializeComponent ();
   }
   #endregion

   #region Properties -----------------------------------------------
   public InkControl InkControl => mInkControl;
   #endregion

   #region Event Handlers -------------------------------------------
   void OnEraserClick (object sender, RoutedEventArgs e) {
      Window thickness = new EraserProperties () { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
      thickness.Show ();
   }

   protected override void OnClosing (CancelEventArgs e) {
      if (InkControl.ChangesSaved) { base.OnClosing (e); return; }
      string message = "You have unsaved changes. Are you sure you want to exit?";
      e.Cancel = MessageBox.Show (message, "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
   }

   void OnOpen (object sender, RoutedEventArgs e) {
      OpenFileDialog openFile = new () {
         CheckFileExists = true, CheckPathExists = true, Multiselect = false,
         InitialDirectory = "C:\\etc", Filter = $"BinaryFiles |*.bin", DefaultExt = ".bin"
      };
      if (openFile.ShowDialog () == true)
         try {
            mInkControl.Open (openFile.FileName, openFile.FilterIndex);
         } catch (Exception) { MessageBox.Show ("Couldn't Open file. Unknown FileFormat."); }
   }

   void OnRedo (object sender, RoutedEventArgs e)
     => InkControl?.Redo ();

   void OnPenClick (object sender, RoutedEventArgs e)
      => InkControl.Shape (DOODLE);

   void OnPenRightClick (object sender, MouseButtonEventArgs e) {
      Window penOptions = new PenProperties () { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
      penOptions.Show ();
   }

   void OnSave (object sender, RoutedEventArgs e) {
      SaveFileDialog saveFile = new () {
         CheckPathExists = true, InitialDirectory = "C:\\etc",
         Filter = $"BinaryFiles |*.bin", AddExtension = true, DefaultExt = ".bin"
      };
      if (saveFile.ShowDialog () == true)
         try {
            InkControl?.Save (saveFile.FileName, saveFile.FilterIndex);
         } catch (NotImplementedException) { MessageBox.Show ("File not saved."); }
   }

   void OnToolBoxClick (object sender, RoutedEventArgs e)
      => InkControl.Shape (((Button)sender).Name switch {
         "mRect" => RECT, "mRectFilled" => FILLEDRECT,
         "mCircle" => CIRCLE1, "mArc" => ARC,
         "mLine" => LINE, "mCLine" => CONNECTEDLINE,
         "mEllipse" => ELLIPSE, "mSelect" => SELECTIONBOX,
         _ => throw new NotImplementedException ()
      });

   void OnUndo (object sender, RoutedEventArgs e)
      => InkControl?.Undo ();
   #endregion
}

public static class ScribbleGlobals {
   public const string Version = "1.1.0";
}