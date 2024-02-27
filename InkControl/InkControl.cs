using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
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
   }
   IShape? mShape;
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
      => (PenColor, mShape) = (Background, new Doodle ());

   public void Shape (Drawing.EType type)
      => mShape = type switch {
         DOODLE => new Doodle (),
         LINE => new Line(), CONNECTEDLINE => new CLine(),
         RECT => new Rect (false), FILLEDRECT => new Rect (true),
         POINT => new Doodle (),
         _ => new Doodle ()
      };

   public void Open (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Open, FileAccess.Read);
      switch (filterIdx) {
         case 1:
            BinaryReader br = new (file, Encoding.ASCII);
            mDrawing = Drawing.LoadBinary (br) as Drawing;
            break;
         default: throw new NotImplementedException ();
      }
      InvalidateVisual ();
   }

   public void Redo () {
      if (mUndoneStrokes.Count == 0) return;
      mDrawing?.Shapes.Add (mUndoneStrokes.Pop () as IShape); // Change to switch case for actions
      InvalidateVisual ();
   }
   Stack<IObject> mUndoneStrokes = new ();

   public void Save (string path, int filterIdx) {
      using var file = new FileStream (path, FileMode.Create, FileAccess.Write);
      switch (filterIdx) {
         case 1:
            BinaryWriter bw = new (file, Encoding.ASCII);
            mDrawing?.SaveBinary (bw);
            break;
         default: throw new NotImplementedException ();
      }
   }

   public void Undo () {
      if (mDrawing?.Shapes.Count == 0) return;
      mUndoneStrokes.Push (mDrawing.Shapes.LastOrDefault ()!);
      mDrawing.Shapes.RemoveAt (mDrawing.Shapes.Count - 1);
      InvalidateVisual ();
   }
   #endregion
}
