namespace Lib;

public interface IObject {
   public bool IsSelected (SelectionBox box); 
}

public interface IShape : IObject, IDrawable {
   public string Color { get; set; }
   public double Thickness { get; set; }
}

public interface IStorable : IObject {
   public void SaveBinary (BinaryWriter writer);
   public static abstract IObject LoadBinary (BinaryReader reader, string version);
}

public interface IDrawable {
   //public void Draw (DrawingContext dc);
}
