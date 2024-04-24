using Lib;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Drawing = Lib.Drawing;
namespace Scribbles;

public partial class InkPad : Canvas {
   #region Constructors ---------------------------------------------
   public InkPad () {
      mDrawing = new ();
      Cursor = Cursors.Pen;
   }
   #endregion

   #region Properties -----------------------------------------------
   public Drawing Drawing {
      get {
         mDrawing ??= new ();
         return mDrawing;
      }
      set {
         if (value is null) return;
         mDrawing = value;
      }
   }
   Drawing? mDrawing;

   public IDrawable? FeedBack { get; set; }

   public string Prompt {
      get => mPrompt;
      set {
         if (value == mPrompt) return;
         mPrompt = value;
         ScribbleWin.mPrompt.Text = value;
      }
   }
   string mPrompt = "Select Mode";
   #endregion

   #region Implementation -------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      mPainter ??= new ();
      if (mDrawing is null) return;
      foreach (var shape in mDrawing.Shapes)
         mPainter.Paint (shape as dynamic, dc);
      if (FeedBack is not null) mPainter.Paint (FeedBack as dynamic, dc);
   }
   Painter? mPainter;

   ScribbleWin ScribbleWin => mMainWindow ??= (ScribbleWin)((StackPanel)((DockPanel)((DockPanel)Parent).Parent).Parent).Parent;
   ScribbleWin? mMainWindow;
   #endregion

   #region Methods --------------------------------------------------
   #endregion
}
