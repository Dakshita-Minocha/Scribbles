using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Scribbles;

// input bar -- prompt
// transform and inverse tranform for drawing <--> screen coordinates
// --> Panned () - Gets Mouse move value and updates value of view space start and end points
// and translates value of identity matrix with dx, dy
// make readonly struct Bounds
// --> Inflated () - returns new Bound with inflated values for zoom acc to zoomFactor
// Compute zoom extents --> scale matrix * transform Matrix
public partial class ScribbleWin : Window {
   #region Constructor ----------------------------------------------
   public ScribbleWin () {
      InitializeComponent ();
      NewWindow ();
   }
   DocManager? mDoc;
   InkPad? mCanvas;
   #endregion

   #region Methods --------------------------------------------------
   public bool IsModified {
      get => mDoc?.IsModified ?? false;
      set {
         if (mDoc is null) return;
         mDoc.IsModified = value;
      }
   }

   void NewWindow () {
      mCanvasDock.Children.Add (mCanvas = new InkPad () { Background = Brushes.LightGray });
      // Binding DataContext for showing prompt
      var binding = new Binding ("Prompt") { Source = mCanvas };
      mPrompt.DataContext = mCanvas.Prompt;
      mPrompt.SetBinding (TextBlock.TextProperty, binding);
      // Binding Commands to IsModified property to enable buttons
      mDoc = new (mCanvas.Drawing);
      Binding commandBinding = new ("FileName") { Source = mDoc };
      DataContext = mDoc;
      SetBinding (TitleProperty, commandBinding);
   }
   #endregion

   #region Private Data ---------------------------------------------
   Widget? mState;
   #endregion

   #region Event Handlers -------------------------------------------
   protected override void OnClosing (CancelEventArgs e) {
      if (mDoc is null || !mDoc.IsModified) { base.OnClosing (e); return; }
      string message = "You have unsaved changes. Are you sure you want to exit?";
      e.Cancel = MessageBox.Show (message, "Close", MessageBoxButton.YesNo) != MessageBoxResult.Yes;
   }

   void OnNew (object sender, RoutedEventArgs e) {
      NewWindow ();
   }

   void OnOpen (object sender, RoutedEventArgs e) {
      NewWindow ();
      if (mCanvas is null) return;
      mDoc = new (mCanvas.Drawing) { IsModified = true };
      mDoc.Load ();
      mCanvas.InvalidateVisual ();
   }

   void OnRedo (object sender, RoutedEventArgs e) { }
   //=> InkControl?.Redo ();

   void OnSave (object sender, RoutedEventArgs e) {
      mDoc?.Save ();
   }

   void OnToolBoxClick (object sender, RoutedEventArgs e) {
      if (mCanvas is null) return;
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

   void CommandBindingCanExecute (object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
   }
}