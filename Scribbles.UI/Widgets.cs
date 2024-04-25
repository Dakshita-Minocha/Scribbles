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
   protected abstract void OnMouseUp (object sender, MouseEventArgs e);
}

/// <summary>Actions that last (For eg: Drawing)</summary>
public abstract class TransientWidget : Widget {
   public TransientWidget (InkPad eventSource) : base (eventSource) { }
   protected abstract void OnMouseDrag (object sender, MouseEventArgs e);
   protected enum MouseState { MouseDown, MouseDrag, MouseUp }

}

/// <summary>Actions that are seen only for the time being they are being performed (For eg: Selection)</summary>
public abstract class IntransientWidget : Widget {
   public IntransientWidget (InkPad eventSource) : base (eventSource) { }
   protected abstract void OnMouseMove (object sender, MouseEventArgs e);
   protected enum MouseState { MouseDown, MouseMove, MouseUp }

}

public class SelectionWidget : TransientWidget {
   public SelectionWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = "Drag over area to Select";
   }
   SelectionBox? mSB;
   MouseState? mDragState;

   public override void Attach () {
      mEventSource.MouseLeftButtonUp += OnMouseUp;
      mEventSource.MouseMove += OnMouseDrag;
      mEventSource.MouseLeftButtonDown += OnMouseDown;
   }

   public override void Detach () {
      mEventSource.MouseLeftButtonUp -= OnMouseUp;
      mEventSource.MouseMove -= OnMouseDrag;
      mEventSource.MouseDown -= OnMouseDown;
   }

   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      mSB = new ();
      mDragState = MouseState.MouseDown;
      var pt = e.GetPosition (mEventSource);
      mSB.TopLeft = new (pt.X, pt.Y);
      mEventSource.FeedBack = mSB;
   }

   protected override void OnMouseDrag (object sender, MouseEventArgs e) {
      if (mDragState == MouseState.MouseUp || mSB is null) return;
      mDragState = MouseState.MouseDrag;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState == MouseState.MouseUp || mSB is null) return;
      mDragState = MouseState.MouseUp;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.FeedBack = null;
      mEventSource.InvalidateVisual ();
   }
}
public class LineWidget : IntransientWidget {
   public LineWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = "Drag to Draw";
   }
   Line? mLine;
   MouseState? mDragState;

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
      if (mDragState == MouseState.MouseDown) return;
      switch (mCount) {
         case 0:
            mCount++;
            mLine = new Line () { Thickness = 1 };
            var pt = e.GetPosition (mEventSource);
            mLine.Start = new (pt.X, pt.Y);
            mEventSource.FeedBack = mLine;
            mDragState = MouseState.MouseUp;
            break;
         case 1:
            if (mLine is null) return;
            pt = e.GetPosition (mEventSource);
            mLine.End = new (pt.X, pt.Y);
            mEventSource.Drawing.Shapes.Add (mLine);
            mEventSource.FeedBack = mLine = null;
            mDragState = MouseState.MouseUp;
            mEventSource.InvalidateVisual ();
            mCount = 0;
            break;
         default: break;
      }
   }
   int mCount;

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mLine is null) return;
      mDragState = MouseState.MouseMove;
      var pt = e.GetPosition (mEventSource);
      mLine.End = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState == MouseState.MouseMove || mLine is null) return;
      mDragState = MouseState.MouseUp;
   }
}
