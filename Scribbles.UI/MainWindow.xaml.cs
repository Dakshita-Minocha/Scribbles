using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Scribbles;

// input bar -- prompt
// what is feedback ???
// show only feedback until drawing is complete
// add to drawing only after drawing is complete and then call onrender ()
// transform and inverse tranform for drawing <--> screen coordinates
public partial class ScribbleWin : Window {
   #region Constructor ----------------------------------------------
   public ScribbleWin () {
      InitializeComponent ();
      DataContext = mCanvas;
   }
   #endregion

   #region Private Data ---------------------------------------------
   Widget? mState;
   #endregion

   #region Event Handlers -------------------------------------------
   protected override void OnClosing (CancelEventArgs e) {
      //if (InkControl.ChangesSaved) { base.OnClosing (e); return; } // i've to change this to mDoc.isModified somehow
      //string message = "You have unsaved changes. Are you sure you want to exit?";
      //e.Cancel = MessageBox.Show (message, "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
   }

   void OnOpen (object sender, RoutedEventArgs e) {
      //OpenFileDialog openFile = new () {
      //   CheckFileExists = true, CheckPathExists = true, Multiselect = false,
      //   InitialDirectory = "C:\\etc", Filter = $"BinaryFiles |*.bin", DefaultExt = ".bin"
      //};
      //if (openFile.ShowDialog () == true)
      //   try {
      //      mInkControl.Open ();
      //   } catch (Exception) { MessageBox.Show ("Couldn't Open file. Unknown FileFormat."); }
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
            //InkControl?.Save (saveFile.FileName);
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