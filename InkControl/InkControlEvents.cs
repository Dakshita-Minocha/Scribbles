using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Event Handlers -------------------------------------------
   protected override void OnStylusDown (StylusDownEventArgs e) {
      Stylus.Capture (this);
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Down (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }
   protected override void OnStylusMove (StylusEventArgs e) {
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Move (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnStylusUp (StylusEventArgs e) {
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Up (e.GetPosition (this));
      InvalidateVisual ();
      Stylus.Capture (null);
   }

   protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mDrawing == null) return;
      Down (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }

   protected override void OnMouseMove (MouseEventArgs e) {
      if (e.StylusDevice != null || e.LeftButton == MouseButtonState.Released || mShape == null) return;
      Move (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mShape == null) return;
      Up (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseRightButtonDown (MouseButtonEventArgs e) {
      mDrawing?.Shapes.ForEach (mUndoneStrokes.Push);
      mDrawing?.Shapes.Clear ();
      InvalidateVisual ();
   }
   #endregion

   #region Implementation -------------------------------------------
   void Up (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y);
            mShape = new Line ();
            break;
         case CLine cLine:
            if (cLine.Points.Count > 1) cLine.Points[^1] = new (pt.X, pt.Y);
            break;
         case Doodle dood:
            dood.Add (pt);
            mShape = new Doodle ();
            break;
         case Rect rect:
            rect.BottomRight = new (pt.X, pt.Y);
            mShape = new Rect (false);
            break;
      };
   }

   void Move (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y); break;
         case CLine cLine:
            if (cLine.Points.Count > 1) cLine.Points[^1] = new (pt.X, pt.Y);
            else cLine.Points.Add (new (pt.X, pt.Y));
            break;
         case Doodle dood:
            dood.Add (pt);
            break;
         case Rect rect:
            rect.BottomRight = new (pt.X, pt.Y); break;
      };
   }

   void Down (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            (line.Color, line.Thickness) = (PenColor, PenThickness);
            mDrawing?.Shapes.Add (line);
            line.Start = new (pt.X, pt.Y);
            break;
         case CLine cLine:
            (cLine.Color, cLine.Thickness) = (PenColor, PenThickness);
            cLine.Points.Add (cLine.Points.Count == 0 ? new Point (pt.X, pt.Y) : cLine.Points.Last ());
            mDrawing?.Shapes.Add (cLine);
            break;
         case Doodle dood:
            dood.Color = PenColor;
            dood.Thickness = PenColor == Background ? EraserThickness : PenThickness;
            mDrawing?.Shapes.Add (dood);
            dood.Add (pt);
            break;
         case Rect rect:
            (rect.Color, rect.Thickness) = (PenColor, PenThickness);
            mDrawing?.Shapes.Add (rect);
            rect.TopLeft = new (pt.X, pt.Y);
            break;
      };
   }
   #endregion
}
