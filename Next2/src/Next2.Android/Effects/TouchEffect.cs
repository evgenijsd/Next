using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Next2.ENums;
using Next2.Helpers;

[assembly: ExportEffect(typeof(Next2.Effects.Droid.TouchEffect), "TouchEffect")]

namespace Next2.Effects.Droid
{
    public class TouchEffect : PlatformEffect
    {
        private Android.Views.View view;
        private Element formsElement;
        private Next2.Effects.TouchEffect libTouchEffect;
        private bool capture;
        private Func<double, double> fromPixels;
        private int[] twoIntArray = new int[2];

        private static Dictionary<Android.Views.View, TouchEffect> viewDictionary = new Dictionary<Android.Views.View, TouchEffect>();

        private static Dictionary<int, TouchEffect> idToEffectDictionary = new Dictionary<int, TouchEffect>();

        protected override void OnAttached()
        {
            view = Control ?? Container;

            var touchEffect = (Next2.Effects.TouchEffect)Element.Effects.FirstOrDefault(e => e is Next2.Effects.TouchEffect);

            if (touchEffect != null && view != null)
            {
                viewDictionary.Add(view, this);

                formsElement = Element;

                libTouchEffect = touchEffect;

                fromPixels = view.Context.FromPixels;

                view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (viewDictionary.ContainsKey(view))
            {
                viewDictionary.Remove(view);
                view.Touch -= OnTouch;
            }
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            int pointerIndex = motionEvent.ActionIndex;
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(twoIntArray);
            Point screenPointerCoords = new Point(twoIntArray[0] + motionEvent.GetX(pointerIndex), twoIntArray[1] + motionEvent.GetY(pointerIndex));

            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    this.FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    idToEffectDictionary.Add(id, this);

                    capture = libTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:

                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (capture)
                        {
                            senderView.GetLocationOnScreen(twoIntArray);

                            screenPointerCoords = new Point(twoIntArray[0] + motionEvent.GetX(pointerIndex), twoIntArray[1] + motionEvent.GetY(pointerIndex));

                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (idToEffectDictionary.Count > 0)
                            {
                                if (idToEffectDictionary[id] != null)
                                {
                                    this.FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                                }
                            }
                        }
                    }

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:

                    if (capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (idToEffectDictionary.Count > 0)
                        {
                            if (idToEffectDictionary[id] != null)
                            {
                                this.FireEvent(idToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                            }
                        }
                    }

                    idToEffectDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:

                    if (capture)
                    {
                        this.FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (idToEffectDictionary.Count > 0)
                        {
                            if (idToEffectDictionary[id] != null)
                            {
                                this.FireEvent(idToEffectDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                            }
                        }
                    }

                    idToEffectDictionary.Remove(id);
                    break;
            }
        }

        private void CheckForBoundaryHop(int id, Point pointerLocation)
        { 
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in viewDictionary.Keys)
            {
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch
                {
                    continue;
                }

                Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = viewDictionary[view];
                }
            }

            if (idToEffectDictionary.Count > 0)
            {
                if (touchEffectHit != idToEffectDictionary[id])
                {
                    if (idToEffectDictionary[id] != null)
                    {
                        this.FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                    }

                    if (touchEffectHit != null)
                    {
                        this.FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                    }

                    idToEffectDictionary[id] = touchEffectHit;
                }
            }
        }

        private void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        {
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

            touchEffect.view.GetLocationOnScreen(twoIntArray);

            double x = pointerLocation.X - twoIntArray[0];
            double y = pointerLocation.Y - twoIntArray[1];

            Point point = new Point(fromPixels(x), fromPixels(y));

            onTouchAction(touchEffect.formsElement, new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}
