namespace Lib;

public class Drawing : IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public Drawing () => Shapes = new ();
   #endregion

   #region Properties -----------------------------------------------
   public List<PLine> Shapes { get; }
   public bool IsSelected { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public bool SetSelected (SelectionBox box) {
      Shapes.ForEach (x => x.SetSelected (box));
      return IsSelected = false;
   }

   public void Draw (IDrawer d)
      => d.DrawDrawing (this);

   public void Save (IFileWriter writer)
      => writer.WriteDrawing (this);

   public static IStorable Load (IFileReader reader)
      => reader.ReadDrawing ();
   #endregion

   #region Enum -----------------------------------------------------
   public enum EType { POINT, DOODLE, LINE, CONNECTEDLINE, RECT, FILLEDRECT, CIRCLE1, CIRCLE2, ELLIPSE, ARC, SELECTIONBOX };
   #endregion
}