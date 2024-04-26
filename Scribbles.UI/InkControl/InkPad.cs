using Lib;
using System;
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
         ScribbleWin.mPrompt.Text = value;
      }
   }
   string mPrompt = "Select Mode";
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

   ScribbleWin ScribbleWin => mMainWindow ??= (ScribbleWin)((StackPanel)((DockPanel)((DockPanel)Parent).Parent).Parent).Parent;
   ScribbleWin? mMainWindow;
   #endregion
}
