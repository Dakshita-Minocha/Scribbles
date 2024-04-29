using System.Windows.Media;
using System.Collections.Generic;
using Lib;
namespace Scribbles;

/// <summary>
/// Drawing Context Adapter -- Converts drawing points to Screen Points and then draws the shape.
/// </summary>
class Painter: IDrawer {
   #region Constructor ----------------------------------------------
   public Painter (DrawingContext dc) { mDC = dc; }
   DrawingContext mDC;
   #endregion

   #region Properties -----------------------------------------------
   public Brush Color {
      get => mColor;
      set {
         if (mColor == value) return;
         mColor = value;
         mPen = null;
      }
   }
   Brush mColor = Brushes.Black;
   #endregion

   #region Methods --------------------------------------------------
   public void Draw (Line line) {
      mPen = new (line.IsSelected ? Brushes.Blue : Color, 1);
      mDC.DrawLine (mPen, new (line.Start.X, line.Start.Y), new (line.End.X, line.End.Y));
   }

   public void Draw (CLine cLine) {
      mPen ??= new Pen (Color, 1);
      for (int i = 0; i < cLine.Points.Count - 1; i++)
         mDC.DrawLine (mPen, new (cLine.Points[i].X, cLine.Points[i].Y), new (cLine.Points[i + 1].X, cLine.Points[i + 1].Y));
   }

   public void Draw (Point dood) {
      //mPen ??= new (Color, 1);
      //for (int i = 0; i < dood.Points.Count - 2; i++)
      //   mDC.DrawLine (mPen, new (dood.Points[i].X, dood.Points[i].Y), new (dood.Points[i + 1].X, dood.Points[i + 1].Y));
   }

   public void Draw (Rect rect) {
      mPen ??= new Pen (Color, 1);
      mDC.DrawRectangle (null, mPen, new System.Windows.Rect (new (rect.TopLeft.X, rect.TopLeft.Y), new System.Windows.Point (rect.BottomRight.X, rect.BottomRight.Y)));
   }

   public void Draw (Ellipse eli) {
      mPen ??= new Pen (Color, 1);
      mDC.DrawEllipse (null, mPen, new (eli.Center.X, eli.Center.Y), eli.RadiusX, eli.RadiusY);
   }

   public void Draw (Arc arc) {
      mPen ??= new (Color, 1);
      mPF.Segments.Clear ();
      mPF.StartPoint = new (arc.StartPoint.X, arc.StartPoint.Y);
      mPF.Segments.Add (new ArcSegment (new (arc.EndPoint.X, arc.EndPoint.Y), new (arc.Radius, arc.Radius), 0, true, 0, true));
      mDC.DrawGeometry (null, mPen, mPG);
   }

   public void Draw (SelectionBox box) {
      Color = Brushes.Blue;
      mDC.DrawRectangle (null, mSelectionPen, new (new (box.TopLeft.X, box.TopLeft.Y), new System.Windows.Point (box.BottomRight.X, box.BottomRight.Y)));
   }
   static readonly Pen mSelectionPen = new (Brushes.BlueViolet, 1) { DashStyle = new () { Dashes = new DoubleCollection (new List<double> () { 8, 8 }) } };
   
   public void Draw (Lib.Drawing dwg) {
      foreach (var shape in dwg.Shapes)
         shape.Draw (this);
   }
   #endregion

   #region Private Data ---------------------------------------------
   Pen? mPen;
   static readonly PathFigure mPF = new ();
   static readonly List<PathFigure> mIPF = new () { mPF };
   static readonly PathGeometry mPG = new (mIPF);
   #endregion
}
