using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Constructors ---------------------------------------------
   public InkControl () {
      DataContext = this;
      PenThickness = EraserThickness = 1;
      PenColor = Brushes.White;
      mPen = new () { Brush = PenColor, Thickness = PenThickness };
      mDrawing = new ();
   }
   Point mInitial, mCurrent;
   Pen mPen;
   Line? mLine;
   Drawing mDrawing;
   #endregion

   #region Properties -----------------------------------------------
   public Brush PenColor { get; set; }
   public double PenThickness { get; set; }
   public double EraserThickness { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      if (mDrawing == null) return;
      mDrawing.Draw (dc);
   }

   public void Erase ()
      => PenColor = Background;

   public void Open (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Open, FileAccess.Read);
      switch (filterIdx) {
         case 1:
            BinaryReader br = new (file, Encoding.ASCII);
            mDrawing = Drawing.LoadBinary (br) as Drawing;
            break;
         case 2:
            StreamReader sr = new (file);
            mDrawing = Drawing.LoadText (sr) as Drawing;
            break;
         default: throw new NotImplementedException ();
      }
      InvalidateVisual ();
   }

   public void Redo () {
      if (mUndoneStrokes.Count == 0) return;
      mDrawing.Shapes.Add (mUndoneStrokes.Pop () as IShape); // Change to switch case for actions
      InvalidateVisual ();
   }
   Stack<IObject> mUndoneStrokes = new ();

   public void Save (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Create, FileAccess.Write);
      switch (filterIdx) {
         case 1:
            BinaryWriter bw = new (file, Encoding.ASCII);
            mDrawing.SaveBinary (bw);
            break;
         case 2:
            StreamWriter sw = new (file);
            mDrawing.SaveText (sw);
            break;
         default: throw new NotImplementedException ();
      }
   }

   public void Undo () {
      if (mDrawing.Shapes.Count == 0) return;
      mUndoneStrokes.Push (mDrawing.Shapes.LastOrDefault ()!);
      mDrawing.Shapes.RemoveAt (mDrawing.Shapes.Count - 1);
      InvalidateVisual ();
   }
   #endregion
}
