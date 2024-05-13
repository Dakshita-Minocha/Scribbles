using Lib;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
namespace Scribbles;

/// <summary>An application, or a component of an interface, that enables a user to perform a function or access a service.</summary>
public abstract class Widget {
   #region Constructor ----------------------------------------------
   public Widget (InkPad eventSource) { mEventSource = eventSource; mInvXfm = mEventSource.Xfm; mInvXfm.Invert (); }
   protected readonly InkPad mEventSource;
   protected readonly Matrix mInvXfm;
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
   public abstract void CreateNew ();
   #endregion

   #region Interface ------------------------------------------------
   #endregion

   #region Enum MouseState -------------------------------------------
   protected enum EMouseState { MouseDown, MouseMove, MouseDrag, MouseUp }
   #endregion

   #region Protected Data -------------------------------------------
   protected EMouseState? mMouseState;
   protected PLine? mShape;
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
   protected abstract void AddEndPoint (Point end);

   protected abstract void Initialise (Point start);
   #endregion

   #region Widget Methods -------------------------------------------
   protected override void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e) {
      if (mMouseState == EMouseState.MouseDown) return;
      switch (mCount) {
         case 0:
            if (mShape is not null) return;
            mCount++;
            var pt = mInvXfm.Transform (e.GetPosition (mEventSource));
            Initialise (new (pt.X, pt.Y));
            mEventSource.FeedBack = mShape;
            mMouseState = EMouseState.MouseDown;
            mEventSource.Prompt = "Click on EndPoint to stop.";
            break;
         case 1:
            if (mShape is null) return;
            pt = mInvXfm.Transform (e.GetPosition (mEventSource));
            AddEndPoint (new (pt.X, pt.Y));
            mEventSource.AddToDrawing (mShape);
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
      if (mShape is null || mMouseState == EMouseState.MouseUp) return;
      mMouseState = EMouseState.MouseMove;
      var pt = mInvXfm.Transform (e.GetPosition (mEventSource));
      AddEndPoint (new (pt.X, pt.Y));
      mEventSource.InvalidateVisual ();
   }

   protected override void OnMouseUp (object sender, MouseEventArgs e) { }
   #endregion
}

public class SelectionWidget : TransientWidget {
   #region Constructor ----------------------------------------------
   public SelectionWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.Prompt = mStartPrompt = "Drag over area to Select";
      mEventSource.InputBar = new InputBar (this);
   }
   #endregion

   #region Methods --------------------------------------------------
   protected override void OnMouseLeftButtonDown (object sender, MouseButtonEventArgs e) {
      mSB = new ();
      mDragState = EMouseState.MouseDown;
      var pt = e.GetPosition (mEventSource);
      mSB.TopLeft = new (pt.X, pt.Y);
      //mEventSource.FeedBack = mSB;
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

   public override void CreateNew () {
      throw new NotImplementedException ();
   }
   #endregion

   #region Private Data ---------------------------------------------
   SelectionBox? mSB;
   EMouseState? mDragState;
   #endregion
}

public class LineWidget : IntransientWidget, INotifyPropertyChanged {
   #region Constructor ----------------------------------------------
   public LineWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.InputBar = new InputBar (this);
   }
   #endregion

   #region Properties -----------------------------------------------
   public double X {
      get => mX;
      set {
         mX = value;
         if (mShape is not null)
            mShape.Points[0] = new Point (value, Y);
         OnPropertyChanged (nameof (X));
      }
   }
   double mX;

   public double Y {
      get => mY;
      set {
         mY = value;
         if (mShape is not null)
            mShape.Points[0] = new Point (X, value);
         OnPropertyChanged (nameof (Y));
      }
   }
   double mY;

   public double DX {
      get => mDX;
      set {
         mDX = value;
         if (mShape is not null)
            mShape.Points[1] = new Point (X + value, Y);
         OnPropertyChanged (nameof (DX));
      }
   }
   double mDX;

   public double DY {
      get => mDY;
      set {
         mDY = value;
         if (mShape is not null)
            mShape.Points[1] = new Point (X + DX, Y + value);
         OnPropertyChanged (nameof (DY));
      }
   }
   double mDY;

   public double Length {
      get => mLength;
      set {
         mLength = value;
         //DX = value * Math.Cos (Angle);
         //DY = value * Math.Sin (Angle);
         OnPropertyChanged (nameof (Length));
      }
   }
   double mLength;

   public double Angle {
      get => mAngle;
      set {
         mAngle = value; // * mPI / 180;
         //DX = Length * Math.Cos (value);
         //DY = Length * Math.Sin (value);
         OnPropertyChanged (nameof (Angle));
      }
   }
   double mAngle = 0;
   const double mPI = Math.PI;
   #endregion

   #region Methods --------------------------------------------------
   public override void CreateNew () {
      var start = new Point (X, Y);
      var end = new Point (X + DX, Y + DY);
      mEventSource.AddToDrawing (PLine.CreateLine (start, end));
      Length = end.DistanceTo (new Point (X, Y));
      Angle = Math.Atan2 (end.Y - Y, end.X - X);
      mEventSource.InvalidateVisual ();
   }

   protected override void Initialise (Point start) {
      (X, Y) = (start.X, start.Y);
      mShape = PLine.CreateLine (start, start);
   }

   protected override void AddEndPoint (Point end) {
      Length = end.DistanceTo (new Point (X, Y));
      Angle = Math.Atan2 (end.Y - Y, end.X - X);
   }

   void OnPropertyChanged (string info)
      => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (info));
   #endregion

   #region Interface ------------------------------------------------
   public event PropertyChangedEventHandler? PropertyChanged;
   #endregion
}

public class RectWidget : IntransientWidget, INotifyPropertyChanged {
   #region Constructor ----------------------------------------------
   public RectWidget (InkPad eventSource) : base (eventSource) {
      mEventSource.InputBar = new InputBar (this);
   }
   #endregion

   #region Properties -----------------------------------------------
   public double X {
      get => mX;
      set {
         mX = value;
         if (mShape is not null)
            mShape.Points[0] = new Point (value, Y);
         OnPropertyChanged (nameof (X));
      }
   }
   double mX;

   public double Y {
      get => mY;
      set {
         mY = value;
         if (mShape is not null)
            mShape.Points[0] = new Point (X, value);
         OnPropertyChanged (nameof (Y));
      }
   }
   double mY;

   public double Width {
      get => mWidth;
      set {
         mWidth = value;
         if (mShape is not null) mShape.Points[1] = new Point (X + value, Y);
         OnPropertyChanged (nameof (Width));
      }
   }
   double mWidth;

   public double Height {
      get => mHeight;
      set {
         mHeight = value;
         if (mShape is not null) mShape.Points[1] = new Point (X + Width, Y + value);
         OnPropertyChanged (nameof (Height));
      }
   }
   double mHeight;
   #endregion

   #region Methods --------------------------------------------------
   public override void CreateNew () {
      mEventSource.AddToDrawing (PLine.CreateRect (new Point (X, Y), new Point (X + Width, Y + Height)));
      mEventSource.InvalidateVisual ();
   }

   protected override void AddEndPoint (Point end) {
      Width = end.X - X; Height = end.Y - Y;
   }

   protected override void Initialise (Point start) {
      (X, Y) = (start.X, start.Y);
      mShape = PLine.CreateRect (start, start);
   }

   void OnPropertyChanged (string info)
      => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (info));
   #endregion

   #region Interface ------------------------------------------------
   public event PropertyChangedEventHandler? PropertyChanged;
   #endregion
}