using Lib;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Drawing = Lib.Drawing;
namespace Scribbles;

public partial class InkPad : Canvas, INotifyPropertyChanged {
   #region Constructors ---------------------------------------------
   public InkPad () {
      Cursor = Cursors.Pen;
      MouseWheel += OnMouseWheel;
   }
   #endregion

   #region Properties -----------------------------------------------
   public Drawing Drawing {
      get {
         mDrawing ??= new ();
         Foreground = Brushes.Black;
         return mDrawing;
      }
      set {
         if (value is null) return;
         mDrawing = value;
      }
   }
   Drawing? mDrawing;

   public PLine? FeedBack {
      get {
         Foreground = Brushes.Red;
         return mFeedBack;
      }
      set {
         mFeedBack = value;
      }
   }
   PLine? mFeedBack;

   public Brush Foreground {
      get => mPainter is not null ? mPainter.Color : Brushes.Black;
      set {
         if (mPainter is not null)
            mPainter.Color = value;
      }
   }

   public InputBar InputBar {
      get => mInputBar; set {
         Children.Remove (mInputBar);
         mInputBar = value;
         Children.Add (mInputBar);
      }
   }
   public InputBar mInputBar = InputBar.Empty;
   public string Prompt {
      get => mPrompt;
      set {
         if (value == mPrompt) return;
         mPrompt = value;
         OnPropertyChanged (nameof (Prompt));
      }
   }
   string mPrompt = "Select Mode";

   /// <summary>Converts to Screen Space.</summary>
   public Matrix Xfm { get; set; }

   /// <summary>Converts to Drawing Space.</summary>
   public Matrix InvXfm { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public void AddToDrawing (PLine obj) {
      Drawing.Shapes.Add (obj);
      MainWindow.IsModified = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   void OnMouseWheel (object sender, MouseWheelEventArgs e) {
      double zoomFactor = 0.5 * e.Delta;

   }

   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      mPainter = new (dc, Xfm);
      if (mDrawing is null) return;
      foreach (var shape in mDrawing.Shapes)
         shape.Draw (mPainter);
      FeedBack?.Draw (mPainter);
   }
   Painter? mPainter;

   void OnPropertyChanged (string info)
      => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (info));
   #endregion

   #region Private Data ---------------------------------------------
   public event PropertyChangedEventHandler? PropertyChanged;
   ScribbleWin MainWindow => mMainWindow ??= (ScribbleWin)((StackPanel)((DockPanel)((TabControl)((TabItem)Parent).Parent).Parent).Parent).Parent;
   ScribbleWin? mMainWindow;
   #endregion
}
