using Lib;
using System.Windows.Input;
namespace Scribbles;

/// <summary>An application, or a component of an interface, that enables a user to perform a function or access a service.</summary>
public abstract class Widget {
   public Widget (InkPad eventSource) { mEventSource = eventSource; }
   protected readonly InkPad mEventSource;

   public void Attach () {
      mEventSource.MouseLeftButtonUp += OnMouseUp;
      mEventSource.MouseMove += OnMouseMove;
      mEventSource.MouseLeftButtonDown += OnMouseDown;
   }

   public void Detach () {
      mEventSource.MouseLeftButtonUp -= OnMouseUp;
      mEventSource.MouseMove -= OnMouseMove;
      mEventSource.MouseDown -= OnMouseDown;
   }
   protected abstract void OnMouseDown (object sender, MouseButtonEventArgs e);
   protected abstract void OnMouseMove (object sender, MouseEventArgs e);
   protected abstract void OnMouseUp (object sender, MouseEventArgs e);
   protected enum MouseState { MouseDown, MouseMove, MouseDrag, MouseUp }
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
   #region Constructor ----------------------------------------------
   public SelectionWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt;
   }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      if (mEventSource.Prompt != mStartPrompt) return;
      mSB = new ();
      mDragState = MouseState.MouseDown;
      var pt = e.GetPosition (mEventSource);
      mSB.TopLeft = new (pt.X, pt.Y);
      mEventSource.FeedBack = mSB;
   }

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
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
      if (mSB.Select (mEventSource.Drawing)) { } // mEventSource.Prompt = "Items Selected";
      mEventSource.InvalidateVisual ();
   }
   #endregion

   #region Private Data ---------------------------------------------
   string mStartPrompt = "Drag over area to Select";
   SelectionBox? mSB;
   MouseState? mDragState;
   #endregion
}

public class LineWidget : IntransientWidget {
   #region Constructor ----------------------------------------------
   public LineWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt;
   }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      if (mDragState == MouseState.MouseDown || mEventSource.Prompt != mStartPrompt) return;
      switch (mCount) {
         case 0:
            mCount++;
            mLine = new Line ();
            var pt = e.GetPosition (mEventSource);
            mLine.Start = new (pt.X, pt.Y);
            mEventSource.FeedBack = mLine;
            //mEventSource.Prompt = "Click on End Point to complete line.";
            mDragState = MouseState.MouseUp;
            break;
         case 1:
            if (mLine is null) return;
            pt = e.GetPosition (mEventSource);
            mLine.End = new (pt.X, pt.Y);
            mEventSource.AddDrawing (mLine);
            mEventSource.FeedBack = mLine = null;
            mDragState = MouseState.MouseUp;
            mEventSource.InvalidateVisual ();
            mCount = 0;
            mEventSource.Prompt = mStartPrompt;
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
   #endregion

   #region Private Data ---------------------------------------------
   string mStartPrompt = "Click on Start Point to start";
   Line? mLine;
   MouseState? mDragState;
   #endregion
}

public class RectWidget : IntransientWidget {
   #region Constructor ----------------------------------------------
   public RectWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt;
   }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnMouseDown (object sender, MouseButtonEventArgs e) {
      if (mDragState == MouseState.MouseDown || mEventSource.Prompt != mStartPrompt) return;
      switch (mCount) {
         case 0:
            mCount++;
            mRect = new Rect ();
            var pt = e.GetPosition (mEventSource);
            mRect.TopLeft = new (pt.X, pt.Y);
            mEventSource.FeedBack = mRect;
            //mEventSource.Prompt = "Click on End Point to complete line.";
            mDragState = MouseState.MouseUp;
            break;
         case 1:
            if (mRect is null) return;
            pt = e.GetPosition (mEventSource);
            mRect.BottomRight = new (pt.X, pt.Y);
            mEventSource.AddDrawing (mRect);
            mEventSource.FeedBack = mRect = null;
            mDragState = MouseState.MouseUp;
            mEventSource.InvalidateVisual ();
            mCount = 0;
            mEventSource.Prompt = mStartPrompt;
            break;
         default: break;
      }
   }
   int mCount;

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mRect is null) return;
      mDragState = MouseState.MouseMove;
      var pt = e.GetPosition (mEventSource);
      mRect.BottomRight = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState == MouseState.MouseMove || mRect is null) return;
      mDragState = MouseState.MouseUp;
   }
   #endregion

   #region Private Data ---------------------------------------------
   string mStartPrompt = "Click on Start Point to start drawing.";
   Rect? mRect;
   MouseState? mDragState;
   #endregion
}