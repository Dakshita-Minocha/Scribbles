namespace Lib;

public interface IObject {
   public bool SetSelected (SelectionBox box);
   public bool IsSelected { get; set; }
}

public interface IFileWriter {
   public void WriteDrawing (Drawing dwg);
   public void WritePLine (PLine pline);
   public void WritePoint (Point pt);
}

public interface IFileReader {
   public Drawing ReadDrawing ();
   public PLine ReadPLine ();
   public Point ReadPoint ();
}

public interface IStorable : IObject {
   public void Save (IFileWriter writer);
   public static abstract IStorable Load (IFileReader reader);
}

public interface IDrawable : IObject {
   public void Draw (IDrawer d);
}

public interface IDrawer {
   public void DrawPoint (Point d);
   public void DrawSelection (SelectionBox obj);
   public void DrawDrawing (Drawing obj);
   public void DrawLine (PLine obj);
   public void DrawRect (PLine obj);
   public void DrawArc (PLine obj);
}