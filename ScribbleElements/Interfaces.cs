using System.IO;
using System.Windows.Media;
namespace Scribbles;

public interface IObject { }

public interface IShape : IObject, IDrawable {
   public Brush Color { get; set; }
   public double Thickness { get; set; }
}

public interface IStorable : IObject {
   public void SaveBinary (BinaryWriter writer);
   public static abstract IObject LoadBinary (BinaryReader reader, string version);
}

public interface IDrawable {
   public void Draw (DrawingContext dc);
}
