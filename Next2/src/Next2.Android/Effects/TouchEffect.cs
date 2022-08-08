using Android.Views;
using Next2.Effects;
using Next2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(Next2.Droid.Effects.TouchEffect), nameof(Next2.Droid.Effects.TouchEffect))]
namespace Next2.Droid.Effects
{
    public class TouchEffect : PlatformEffect
    {
        private static readonly Dictionary<Android.Views.View, TouchEffect> _viewDictionary = new Dictionary<Android.Views.View, TouchEffect>();
        private static readonly Dictionary<int, TouchEffect> _idToEffectDictionary = new Dictionary<int, TouchEffect>();

        private Android.Views.View _view;
        private Element _formsElement;
        private Next2.Effects.TouchEffect _libTouchEffect;
        private bool _capture;
        private Func<double, double> _fromPixels;
        private int[] _twoIntArray = new int[2];

        #region -- Overrides --

        protected override void OnAttached()
        {
            _view = Control == null ? Container : Control;

            Next2.Effects.TouchEffect touchEffect = (Next2.Effects.TouchEffect)Element.Effects.FirstOrDefault(e => e is Next2.Effects.TouchEffect);

            if (touchEffect != null && _view != null)
            {
                _viewDictionary.Add(_view, this);

                _formsElement = Element;

                _libTouchEffect = touchEffect;

                _fromPixels = _view.Context.FromPixels;

                _view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (_viewDictionary.ContainsKey(_view))
            {
                _viewDictionary.Remove(_view);
                _view.Touch -= OnTouch;
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            int pointerIndex = motionEvent.ActionIndex;

            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(_twoIntArray);

            Point screenPointerCoords = new Point(
                _twoIntArray[0] + motionEvent.GetX(pointerIndex),
                _twoIntArray[1] + motionEvent.GetY(pointerIndex));

            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, ETouchActionType.Pressed, screenPointerCoords, true);

                    _idToEffectDictionary.Add(id, this);

                    _capture = _libTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_capture)
                        {
                            senderView.GetLocationOnScreen(_twoIntArray);

                            screenPointerCoords = new Point(
                                _twoIntArray[0] + motionEvent.GetX(pointerIndex),
                                _twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, ETouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (_idToEffectDictionary[id] != null)
                            {
                                FireEvent(_idToEffectDictionary[id], id, ETouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (_capture)
                    {
                        FireEvent(this, id, ETouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (_idToEffectDictionary[id] != null)
                        {
                            FireEvent(_idToEffectDictionary[id], id, ETouchActionType.Released, screenPointerCoords, false);
                        }
                    }

                    _idToEffectDictionary.Remove(id);

                    break;

                case MotionEventActions.Cancel:
                    if (_capture)
                    {
                        FireEvent(this, id, ETouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (_idToEffectDictionary[id] != null)
                        {
                            FireEvent(_idToEffectDictionary[id], id, ETouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }

                    _idToEffectDictionary.Remove(id);

                    break;
            }
        }

        private void CheckForBoundaryHop(int id, Point pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in _viewDictionary.Keys)
            {
                try
                {
                    view.GetLocationOnScreen(_twoIntArray);
                }
                catch
                {
                    continue;
                }

                Rectangle viewRect = new Rectangle(_twoIntArray[0], _twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = _viewDictionary[view];
                }
            }

            if (touchEffectHit != _idToEffectDictionary[id])
            {
                if (_idToEffectDictionary[id] != null)
                {
                    FireEvent(_idToEffectDictionary[id], id, ETouchActionType.Exited, pointerLocation, true);
                }

                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, ETouchActionType.Entered, pointerLocation, true);
                }

                _idToEffectDictionary[id] = touchEffectHit;
            }
        }

        private void FireEvent(TouchEffect touchEffect, int id, ETouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect._libTouchEffect.OnTouchAction;

            touchEffect._view.GetLocationOnScreen(_twoIntArray);

            double x = pointerLocation.X - _twoIntArray[0];
            double y = pointerLocation.Y - _twoIntArray[1];
            Point point = new Point(_fromPixels(x), _fromPixels(y));

            onTouchAction(touchEffect._formsElement, new TouchActionEventArgs(id, actionType, point, isInContact));
        }

        #endregion
    }
}