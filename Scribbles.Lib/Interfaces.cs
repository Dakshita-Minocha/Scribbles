namespace Lib;

public interface IObject {
   public bool SetSelected (SelectionBox box);
   public bool IsSelected { get; set; }
}

public interface IStorable : IObject {
   public void SaveBinary (BinaryWriter writer);
   public static abstract IObject LoadBinary (BinaryReader reader, string version);
}

public interface IDrawable : IObject {
   public void Draw (IDrawer d);
}

public interface IDrawer {
   public void Draw (Point d);
   public void Draw (Line obj);
   public void Draw (CLine obj);
   public void Draw (Rect obj);
   public void Draw (Ellipse obj);
   public void Draw (Arc obj);
   public void Draw (SelectionBox obj);
}