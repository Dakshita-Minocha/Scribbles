using Lib;
using System.Windows.Input;
namespace Scribbles;

/// <summary>An application, or a component of an interface, that enables a user to perform a function or access a service.</summary>
public abstract class Widget {
   #region Constructor ----------------------------------------------
   public Widget (InkPad eventSource) { mEventSource = eventSource; }
   protected readonly InkPad mEventSource;
   #endregion

   #region Widget Methods -------------------------------------------
   public void Attach () {
      mEventSource.MouseLeftButtonUp += OnMouseUp;
      mEventSource.MouseMove += OnMouseMove;
      mEventSource.MouseLeftButtonDown += OnMouseLeftButtonDown;
   }

   public void Detach () {
      mEventSource.MouseLeftButtonUp -= OnMouseUp;
      mEventSource.MouseMove -= OnMouseMove;
      mEventSource.MouseLeftButtonDown -= OnMouseLeftButtonDown;
   }
   #endregion

   #region Abstract Methods -----------------------------------------
   protected abstract void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e);
   protected abstract void OnMouseMove (object sender, MouseEventArgs e);
   protected abstract void OnMouseUp (object sender, MouseEventArgs e);
   #endregion

   #region Enum MouseState -------------------------------------------
   protected enum EMouseState { MouseDown, MouseMove, MouseDrag, MouseUp }
   #endregion

   #region Protected Data -------------------------------------------
   protected EMouseState? mMouseState;
   protected IDrawable? mShape;
   protected string mStartPrompt = "Select Mode";
   #endregion
}

/// <summary>Actions that last (For eg: Drawing)</summary>
public abstract class TransientWidget : Widget {
   public TransientWidget (InkPad eventSource) : base (eventSource) { }
}

/// <summary>Actions that are seen only for the time being they are being performed (For eg: Selection)</summary>
public abstract class IntransientWidget : Widget {
   #region Constructor ----------------------------------------------
   public IntransientWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt = "Click on StartPoint to start drawing.";
   }
   #endregion

   #region Abstract Methods -----------------------------------------
   protected abstract void AddPoint (Point end);

   protected abstract void Initialise (Point start);
   #endregion

   #region Widget Methods -------------------------------------------
   protected override void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e) {
      if (mMouseState == EMouseState.MouseDown) return;
      switch (mCount) {
         case 0:
            if (mShape is not null) return;
            mCount++;
            var pt = e.GetPosition (mEventSource);
            Initialise (new (pt.X, pt.Y));
            mEventSource.FeedBack = mShape;
            mMouseState = EMouseState.MouseUp;
            mEventSource.Prompt = "Click on EndPoint to stop.";
            break;
         case 1:
            if (mShape is null) return;
            pt = e.GetPosition (mEventSource);
            AddPoint (new (pt.X, pt.Y));
            mEventSource.AddDrawing (mShape);
            mEventSource.FeedBack = mShape = null;
            mMouseState = EMouseState.MouseUp;
            mEventSource.InvalidateVisual ();
            mCount = 0;
            mEventSource.Prompt = mStartPrompt;
            break;
         default: break;
      }
   }
   int mCount;

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mShape is null) return;
      mMouseState = EMouseState.MouseMove;
      var pt = e.GetPosition (mEventSource);
      AddPoint (new (pt.X, pt.Y));
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mMouseState == EMouseState.MouseMove || mShape is null) return;
      mMouseState = EMouseState.MouseUp;
   }
   #endregion
}

public class SelectionWidget : TransientWidget {
   #region Constructor ----------------------------------------------
   public SelectionWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt = "Drag over area to Select";
   }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e) {
      mSB = new ();
      mDragState = EMouseState.MouseDown;
      var pt = e.GetPosition (mEventSource);
      mSB.TopLeft = new (pt.X, pt.Y);
      mEventSource.FeedBack = mSB;
   }

   protected override void OnMouseMove (object sender, MouseEventArgs e) {
      if (mDragState == EMouseState.MouseUp || mSB is null) return;
      mDragState = EMouseState.MouseDrag;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) {
      if (mDragState == EMouseState.MouseUp || mSB is null) return;
      mDragState = EMouseState.MouseUp;
      var pt = e.GetPosition (mEventSource);
      mSB.BottomRight = new (pt.X, pt.Y);
      mEventSource.FeedBack = null;
      if (mSB.Select (mEventSource.Drawing)) { }
      mEventSource.Prompt = "Items Selected";
      mEventSource.InvalidateVisual ();
   }
   #endregion

   #region Private Data ---------------------------------------------
   SelectionBox? mSB;
   EMouseState? mDragState;
   #endregion
}

public class LineWidget : IntransientWidget {
   #region Constructor ----------------------------------------------
   public LineWidget (InkPad eventSource) : base (eventSource) { }
   #endregion

   #region Methods --------------------------------------------------
   protected override void Initialise (Point start)
      => mShape = new Line () { Start = start };

   protected override void AddPoint (Point end) {
      if (mShape is not null && mShape is Line line)
         line.End = end;
   }
   #endregion

   #region Private Data ---------------------------------------------
   #endregion
}

public class RectWidget : IntransientWidget {
   #region Constructor ----------------------------------------------
   public RectWidget (InkPad eventSource) : base (eventSource) { }
   #endregion

   #region Methods --------------------------------------------------
   protected override void AddPoint (Point end) {
      if (mShape is not null && mShape is Rect rect)
         rect.BottomRight = end;
   }

   protected override void Initialise (Point start)
      => mShape = new Rect () { TopLeft = start };
   #endregion

   #region Private Data ---------------------------------------------
   #endregion
}