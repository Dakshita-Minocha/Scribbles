using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Scribbles;

// input bar -- prompt
// transform and inverse tranform for drawing <--> screen coordinates
public partial class ScribbleWin : Window {
   #region Constructor ----------------------------------------------
   public ScribbleWin () {
      InitializeComponent ();
      DataContext = mCanvas;
      mDoc = new ();
   }
   DocManager mDoc;
   #endregion

   #region Private Data ---------------------------------------------
   Widget? mState;
   #endregion

   #region Event Handlers -------------------------------------------
   protected override void OnClosing (CancelEventArgs e) {
      if (!mDoc.IsModified) { base.OnClosing (e); return; }
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
            mDoc.Load (openFile.FileName);
         } catch (Exception) { MessageBox.Show ("Couldn't Open file. Unknown FileFormat."); }
   }

   void OnRedo (object sender, RoutedEventArgs e) { }
   //=> InkControl?.Redo ();

   void OnSave (object sender, RoutedEventArgs e) {
      SaveFileDialog saveFile = new () {
         CheckPathExists = true, InitialDirectory = "C:\\etc",
         Filter = $"BinaryFiles |*.bin", AddExtension = true, DefaultExt = ".bin"
      };
      if (saveFile.ShowDialog () == true)
         try {
            mDoc.Save (saveFile.FileName);
         } catch (NotImplementedException) { MessageBox.Show ("File not saved."); }
   }

   void OnToolBoxClick (object sender, RoutedEventArgs e) {
      Widget state = ((Button)sender).Name switch {
         "mLine" => new LineWidget (mCanvas),
         "mSelect" => new SelectionWidget (mCanvas),
         _ => throw new NotImplementedException ()
      };
      if (state != mState) {
         mState?.Detach ();
         mState = state;
         mState.Attach ();
      }
   }
   #endregion
}