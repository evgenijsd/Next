using CoreGraphics;
using Foundation;
using Next2.ENums;
using Next2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Next2.Effects.iOS
{
    public class TouchRecognizer : UIGestureRecognizer
    {
        private Element element;
        private UIView view;
        private Next2.Effects.TouchEffect touchEffect;
        private bool capture;

        private static Dictionary<UIView, TouchRecognizer> viewDictionary = new Dictionary<UIView, TouchRecognizer>();

        private static Dictionary<long, TouchRecognizer> idToTouchDictionary = new Dictionary<long, TouchRecognizer>();

        public TouchRecognizer(Element element, UIView view, Next2.Effects.TouchEffect touchEffect)
        {
            this.element = element;
            this.view = view;
            this.touchEffect = touchEffect;

            viewDictionary.Add(view, this);
        }

        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                this.FireEvent(this, id, TouchActionType.Pressed, touch, true);

                if (!idToTouchDictionary.ContainsKey(id))
                {
                    idToTouchDictionary.Add(id, this);
                }
            }

            capture = touchEffect.Capture;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Moved, touch, true);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary.ContainsKey(id) && idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch, true);
                    }
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    FireEvent(this, id, TouchActionType.Released, touch, false);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary.ContainsKey(id) && idToTouchDictionary[id] != null)
                    {
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Released, touch, false);
                    }
                }

                idToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (capture)
                {
                    this.FireEvent(this, id, TouchActionType.Cancelled, touch, false);
                }
                else if (idToTouchDictionary.ContainsKey(id) && (idToTouchDictionary[id] != null))
                {
                    this.FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);
                }

                idToTouchDictionary.Remove(id);
            }
        }

        private void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            TouchRecognizer recognizerHit = null;

            foreach (UIView view in viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(default(CGPoint), view.Frame.Size).Contains(location))
                {
                    recognizerHit = viewDictionary[view];
                }
            }

            if (idToTouchDictionary.ContainsKey(id))
            {
                if (recognizerHit != idToTouchDictionary[id])
                {
                    if (idToTouchDictionary[id] != null)
                    {
                        this.FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch, true);
                    }

                    if (recognizerHit != null)
                    {
                        this.FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);
                    }

                    idToTouchDictionary[id] = recognizerHit;
                }
            }
        }

        private void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)
        {
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            Action<Element, TouchActionEventArgs> onTouchAction = recognizer.touchEffect.OnTouchAction;

            Console.WriteLine($"Event = {recognizer.element}, {actionType}");

            onTouchAction(recognizer.element, new TouchActionEventArgs(id, actionType, xfPoint, isInContact));
        }
    }
}