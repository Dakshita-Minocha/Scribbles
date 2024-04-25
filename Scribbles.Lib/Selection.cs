namespace Lib;

public class SelectionBox : IObject, IDrawable {
   #region Properties -----------------------------------------------
   public Point BottomRight { get; set; }

   public List<(IObject Obj, string Color)> SelectedItems => mSelectedItems;

   public Point TopLeft { get; set; }
   #endregion

   #region Methods --------------------------------------------------   
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
            shape.Color = "#FF87CEFA";
         }
      }
   }
   List<(IObject Obj, string Color)> mSelectedItems = new ();
   #endregion
}