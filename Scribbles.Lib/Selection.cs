namespace Lib;

public class SelectionBox : IObject, IDrawable {
   #region Properties -----------------------------------------------
   public Point BottomRight { get; set; }

   public Point TopLeft { get; set; }

   public bool IsSelected { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public void Draw (IDrawer d)
      => d.DrawSelection (this);

   public bool SetSelected (SelectionBox box) => false;

   public bool Select (Drawing drawing) {
      // Assigning correct values if user starts drawing from TopRight
      if (BottomRight.X < TopLeft.X)
         (TopLeft, BottomRight) = (new Point (BottomRight.X, TopLeft.Y), new Point (TopLeft.X, BottomRight.Y));
      mSelectedItems.Clear ();
      foreach (var shape in drawing.Shapes)
         if (shape.SetSelected (this)) mSelectedItems.Add (shape);
      return mSelectedItems.Count > 0;
   }
   List<IObject> mSelectedItems = new ();
   #endregion
}