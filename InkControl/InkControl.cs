using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Scribbles.Drawing.EType;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Constructors ---------------------------------------------
   public InkControl () {
      DataContext = this;
      PenThickness = EraserThickness = 1;
      PenColor = Brushes.White;
      mDrawing = new ();
      Cursor = Cursors.Pen;
   }
   IDrawable? mShape;
   Drawing mDrawing;
   #endregion

   #region Properties -----------------------------------------------
   public bool ChangesSaved = true;
   public Brush PenColor { get; set; }
   public double PenThickness { get; set; }
   public double EraserThickness { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnRender (DrawingContext dc) {
      base.OnRender (dc);
      mDrawing?.Draw (dc);
   }

   public void Erase ()
      => (PenColor, mShape) = (Background, new Doodle ());

   public void Shape (Drawing.EType type)
      => mShape = type switch {
         DOODLE => new Doodle (),
         LINE => new Line(), CONNECTEDLINE => new CLine(),
         RECT => new Rect (false), FILLEDRECT => new Rect (true),
         CIRCLE1 => new Circle1(), CIRCLE2 => new Circle2(),
         ELLIPSE => new Ellipse (), ARC => new Arc (),
         POINT => new Doodle (), SELECTIONBOX => new SelectionBox (),
         _ => new Doodle ()
      };

   public void Open (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Open, FileAccess.Read);
      Drawing dr;
      switch (filterIdx) {
         case 1:
            BinaryReader br = new (file, Encoding.ASCII);
            dr = (Drawing)Drawing.LoadBinary (br, "");
            break;
         default: throw new NotImplementedException ();
      }
      InvalidateVisual ();
      ChangesSaved = true;
      mUndoneStrokes.Clear ();
      mDrawing = new ();
      mDrawing.Shapes.Add (dr);
   }

   public void Redo () {
      if (mUndoneStrokes.Count == 0) return;
      mDrawing?.Shapes.Add (mUndoneStrokes.Pop ());
      InvalidateVisual ();
   }
   readonly Stack<IDrawable> mUndoneStrokes = new ();

   public void Save (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Create, FileAccess.Write);
      switch (filterIdx) {
         case 1:
            BinaryWriter bw = new (file, Encoding.ASCII);
            bw.Write ($"Version:{ScribbleGlobals.Version}\n");
            mDrawing?.SaveBinary (bw);
            break;
         default: throw new NotImplementedException ();
      }
      ChangesSaved = true;
   }

   public void Undo () {
      if (mDrawing?.Shapes.Count == 0 || mDrawing?.Shapes.Last () is Drawing) return;
      mUndoneStrokes.Push (mDrawing?.Shapes.Last ()!);
      mDrawing?.Shapes.RemoveAt (mDrawing.Shapes.Count - 1);
      InvalidateVisual ();
   }
   #endregion
}
