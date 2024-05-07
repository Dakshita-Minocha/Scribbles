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
      if (mCanvas is null) return;
   }
   DocManager? mDoc;
   InkPad? mCanvas;
   #endregion

   #region Properties ----------------------------------------------
   public bool IsModified {
      get => mDoc is not null && mDoc.IsModified;
      set {
         if (mDoc is null) return;
         mDoc.IsModified = value;
      }
   }
   #endregion

   #region Implementation -------------------------------------------
   void NewWindow () {
      mCanvas = new InkPad () { Background = Brushes.LightGray, Width = mTabs.Width, Height = mTabs.Height };
      // Binding DataContext for showing prompt
      var binding = new Binding ("Prompt") { Source = mCanvas };
      mPrompt.DataContext = mCanvas.Prompt;
      mPrompt.SetBinding (TextBlock.TextProperty, binding);
      // Binding Commands to IsModified property to enable buttons
      mDoc = new (mCanvas.Drawing);
      Binding tabHeaderBinding = new ("FileName") { Source = mDoc };
      DataContext = mDoc;
      var newTab = new TabItem () { Content = mCanvas, Tag = mDoc };
      newTab.SetBinding (TabItem.HeaderProperty, tabHeaderBinding);
      mTabs.Items.Add (newTab);
      mCanvas.Loaded += delegate {
         var bound = new Bound (new Lib.Point (-10, -10), new Lib.Point (1000, 1000));
         mProjXfm = Transform.ComputeProjectionXfm (mCanvas.ActualHeight, mCanvas.ActualWidth, 10, bound);
         mCanvas.Xfm = mInvProjXfm = mProjXfm;
         mInvProjXfm.Invert ();
      };
   }
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
      if (mCanvas is null || mDoc is null) return;
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
         "mRect" => new RectWidget (mCanvas),
         "mSelect" => new SelectionWidget (mCanvas),
         _ => throw new NotImplementedException ()
      };
      if (state != mState) {
         mState?.Detach ();
         mState = state;
         mState.Attach ();
      }
   }

   void OnTabChanged (object sender, SelectionChangedEventArgs e) {
      var tc = (TabItem)((TabControl)sender).SelectedItem;
      mCanvas = (InkPad)tc.Content;
      mDoc = (DocManager)tc.Tag;
   }

   void CommandBindingCanExecute (object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
   }
   #endregion

   #region Private Data ---------------------------------------------
   Widget? mState;
   Matrix mProjXfm, mInvProjXfm;
   #endregion
}