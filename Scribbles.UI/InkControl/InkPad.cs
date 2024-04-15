using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
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
   ScribbleWin ScribbleWin => (ScribbleWin)((Grid)Parent).Parent;

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
   }
   Painter? mPainter;

   #endregion

   #region Methods --------------------------------------------------
   #endregion
}
