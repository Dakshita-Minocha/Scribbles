using System.Collections.Generic;
using System.Windows.Media;
namespace Scribbles;

public class SelectionBox : IObject, IDrawable {
   #region Properties -----------------------------------------------
   public Point BottomRight { get; set; }

   public List<(IObject Obj, Brush Color)> SelectedItems => mSelectedItems;

   public Point TopLeft { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public void Draw (DrawingContext dc) {
      dc.DrawRectangle (null, mPen, new (new System.Windows.Point (TopLeft.X, TopLeft.Y), new System.Windows.Point (BottomRight.X, BottomRight.Y)));
   }
   static readonly Pen mPen = new (Brushes.BlueViolet, 1) { DashStyle = new () { Dashes = new DoubleCollection (new List<double> () { 8, 8 }) } };
   
   public bool IsSelected (SelectionBox box) => false;

   void Restore () {
      SelectedItems.ForEach (item => ((IShape)item.Obj).Color = item.Color);
      SelectedItems.Clear ();
   }

   public void Select (Drawing drawing) {
      // Assigning correct values if user starts drawing from TopRight
      if (BottomRight.X < TopLeft.X)
         (TopLeft, BottomRight) = (new Point (BottomRight.X, TopLeft.Y), new Point (TopLeft.X, BottomRight.Y));
      if (SelectedItems.Count > 0) Restore ();
      foreach (var obj in drawing.Shapes) {
         var shape = (IShape)obj;
         if (shape.IsSelected (this)) {
            SelectedItems.Add ((shape, shape.Color));
            shape.Color = Brushes.Blue;
         }
      }
   }
   List<(IObject Obj, Brush Color)> mSelectedItems = new ();
   #endregion
}