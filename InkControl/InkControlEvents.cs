using System.Windows.Controls;
using System.Windows.Input;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Event Handlers -------------------------------------------
   protected override void OnStylusDown (StylusDownEventArgs e) {
      Stylus.Capture (this);
      if (e.StylusDevice == null || mLine != null || mDrawing == null) return;
      mLine = new () { Color = PenColor, Thickness = PenColor == Background ? EraserThickness : PenThickness };
      mDrawing.Shapes.Add (mLine);
      mLine.Add (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }
   protected override void OnStylusMove (StylusEventArgs e) {
      if (e.StylusDevice == null || mLine == null || mDrawing == null) return;
      mLine.Add (e.GetPosition (this));
   }

   protected override void OnStylusUp (StylusEventArgs e) {
      if (e.StylusDevice == null || mLine == null || mDrawing == null) return;
      mLine.Add (e.GetPosition (this));
      InvalidateVisual ();
      Stylus.Capture (null);
   }

   protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mDrawing == null) return;
      mLine = new () { Color = PenColor, Thickness = PenColor == Background ? EraserThickness : PenThickness };
      mDrawing.Shapes.Add (mLine);
      mLine.Add (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }

   protected override void OnMouseMove (MouseEventArgs e) {
      if (e.StylusDevice != null || e.LeftButton == MouseButtonState.Released || mLine == null) return;
      mLine.Add (e.GetPosition (this));
   }

   protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mLine == null) return;
      mLine.Add (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseRightButtonDown (MouseButtonEventArgs e) {
      mDrawing.Shapes.ForEach (mUndoneStrokes.Push);
      mDrawing.Shapes.Clear ();
      InvalidateVisual ();
   }
   #endregion
}
