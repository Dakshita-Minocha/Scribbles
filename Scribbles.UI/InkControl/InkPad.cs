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
   }
   #endregion

   #region Properties -----------------------------------------------
   public Brush Foreground {
      get => mPainter is not null ? mPainter.Color : Brushes.Black;
      set {
         if (mPainter is not null)
            mPainter.Color = value;
      }
   }

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

   public IDrawable? FeedBack {
      get {
         Foreground = Brushes.Red;
         return mFeedBack;
      }
      set {
         mFeedBack = value;
      }
   }
   IDrawable? mFeedBack;

   public string Prompt {
      get => mPrompt;
      set {
         if (value == mPrompt) return;
         mPrompt = value;
         OnPropertyChanged (nameof (Prompt));
      }
   }
   string mPrompt = "Select Mode";
   #endregion

   #region Methods --------------------------------------------------
   public void AddDrawing (IDrawable obj) {
      Drawing.Shapes.Add (obj);
      MainWindow.IsModified = true;
   }
   #endregion

   #region Implementation -------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      mPainter = new (dc);
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
