using System.Windows.Media;
using System.Collections.Generic;
using Lib;
namespace Scribbles;

class Painter {
   #region Constructors ---------------------------------------------
   public Painter () { }
   static Pen? mPen;
   static readonly PathFigure mPF = new ();
   static readonly List<PathFigure> mIPF = new () { mPF };
   static readonly PathGeometry mPG = new (mIPF);
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
   Brush mColor = Brushes.White;
   #endregion

   #region Methods --------------------------------------------------
   public void Paint (Line line, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (line.Color));
      mPen ??= new (Color, line.Thickness);
      dc.DrawLine (mPen, new (line.Start.X, line.Start.Y), new (line.End.X, line.End.Y));
   }

   public void Paint (CLine cLine, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (cLine.Color));
      mPen ??= new Pen (Color, cLine.Thickness);
      for (int i = 0; i < cLine.Points.Count - 1; i++)
         dc.DrawLine (mPen, new (cLine.Points[i].X, cLine.Points[i].Y), new (cLine.Points[i + 1].X, cLine.Points[i + 1].Y));
   }

   public void Paint (Doodle dood, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (dood.Color));
      mPen ??= new (Color, dood.Thickness);
      for (int i = 0; i < dood.Points.Count - 2; i++)
         dc.DrawLine (mPen, new (dood.Points[i].X, dood.Points[i].Y), new (dood.Points[i + 1].X, dood.Points[i + 1].Y));
   }

   public void Paint (Rect rect, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (rect.Color));
      mPen ??= new Pen (Color, rect.Thickness);
      dc.DrawRectangle (rect.Fill ? Color : null, mPen, new System.Windows.Rect (new (rect.TopLeft.X, rect.TopLeft.Y), new System.Windows.Point (rect.BottomRight.X, rect.BottomRight.Y)));
   }

   public void Paint (Ellipse eli, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (eli.Color));
      mPen ??= new Pen (Color, eli.Thickness);
      dc.DrawEllipse (null, mPen, new (eli.Center.X, eli.Center.Y), eli.RadiusX, eli.RadiusY);
   }

   public void Paint (Arc arc, DrawingContext dc) {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (arc.Color));
      mPen ??= new (Color, arc.Thickness);
      mPF.Segments.Clear ();
      mPF.StartPoint = new (arc.StartPoint.X, arc.StartPoint.Y);
      mPF.Segments.Add (new ArcSegment (new (arc.EndPoint.X, arc.EndPoint.Y), new (arc.Radius, arc.Radius), 0, true, 0, true));
      dc.DrawGeometry (null, mPen, mPG);
   }

   public void Paint (SelectionBox box, DrawingContext dc) {
      Color = Brushes.Blue;
      dc.DrawRectangle (null, mSelectionPen, new (new (box.TopLeft.X, box.TopLeft.Y), new System.Windows.Point (box.BottomRight.X, box.BottomRight.Y)));
   }
   static readonly Pen mSelectionPen = new (Brushes.BlueViolet, 1) { DashStyle = new () { Dashes = new DoubleCollection (new List<double> () { 8, 8 }) } };
   #endregion
}
