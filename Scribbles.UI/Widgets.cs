using Lib;
using System.Windows.Input;
namespace Scribbles;

/// <summary>An application, or a component of an interface, that enables a user to perform a function or access a service.</summary>
public abstract class Widget {
   public Widget (InkPad eventSource) { mEventSource = eventSource; }
   protected readonly InkPad mEventSource;
   public abstract void Attach ();
   public abstract void Detach ();
   protected abstract void OnMouseDown (object sender, MouseButtonEventArgs e);
   protected abstract void OnMouseMove (object sender, MouseEventArgs e);
   protected abstract void OnMouseUp (object sender, MouseEventArgs e);
   protected enum DragState { MouseDown, MouseDrag, MouseUp }
}


/// <summary>Actions that last (For eg: Drawing)</summary>
public abstract class TransientWidget : Widget {
   public TransientWidget (InkPad eventSource) : base (eventSource) { }
}

/// <summary>Actions that are seen only for the time being they are being performed (For eg: Selection)</summary>
public abstract class IntransientWidget : Widget {
   public IntransientWidget (InkPad eventSource) : base (eventSource) { }
}

public class SelectionWidget : TransientWidget {
   public SelectionWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = "Drag over area to Select";
   }
   SelectionBox? mSB;
   DragState? mDragState;

   public override void Attach () {
      mEventSource.MouseLeftButtonUp += OnMouseUp;
      mEventSource.MouseMove += OnMouseMove;
      mEventSource.MouseLeftButtonDown += OnMouseDown;
   }

   public override void Detach () {
      mEventSource.MouseLeftButtonUp -= OnMouseUp;
      mEventSource.MouseMove -= OnMouseMove;
      mEventSource.MouseDown -= OnMouseDown;
   }

   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mSB = new ();
      mDragState = DragState.MouseDown;
      var pt = e.GetPosition (mEventSource);
      mSB.TopLeft = new (pt.X, pt.Y);
      mEventSource.Drawing.Shapes.Add (mSB);
   }

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mDragState == DragState.MouseUp || mSB is null) return;
      mDragState = DragState.MouseDrag;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState != DragState.MouseDrag || mSB is null) return;
      mDragState = DragState.MouseUp;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.Drawing.Shapes.RemoveAt (mEventSource.Drawing.Shapes.Count - 1);
      mEventSource.InvalidateVisual ();
      Detach ();
   }
}
public class LineWidget : IntransientWidget {
   public LineWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = "Drag to Draw";
   }
   Line? mLine;
   DragState? mDragState;

   public override void Attach () {
      mEventSource.MouseLeftButtonUp += OnMouseUp;
      mEventSource.MouseMove += OnMouseMove;
      mEventSource.MouseLeftButtonDown += OnMouseDown;
   }

   public override void Detach () {
      mEventSource.MouseLeftButtonUp -= OnMouseUp;
      mEventSource.MouseMove -= OnMouseMove;
      mEventSource.MouseDown -= OnMouseDown;
   }

   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mDragState = DragState.MouseDown;
      mLine = new Line () { Thickness = 4 };
      var pt = e.GetPosition (mEventSource);
      mLine.Start = new (pt.X, pt.Y);
      mEventSource.Drawing.Shapes.Add (mLine);
   }

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mDragState == DragState.MouseUp || mLine is null) return;
      mDragState = DragState.MouseDrag;
      var pt = e.GetPosition (mEventSource);
      mLine.End = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState != DragState.MouseDrag || mLine is null) return;
      mDragState = DragState.MouseUp;
      var pt = e.GetPosition (mEventSource);
      mLine.End = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }
}
