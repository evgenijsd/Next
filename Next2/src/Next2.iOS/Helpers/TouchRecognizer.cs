using CoreGraphics;
using Foundation;
using Next2.Effects;
using Next2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace Next2.iOS.Helpers
{
    public class TouchRecognizer : UIGestureRecognizer
    {
        private static readonly Dictionary<UIView, TouchRecognizer> _viewDictionary = new Dictionary<UIView, TouchRecognizer>();
        private static readonly Dictionary<long, TouchRecognizer> _idToTouchDictionary = new Dictionary<long, TouchRecognizer>();

        private Element _element;
        private UIView _view;
        private TouchEffect _touchEffect;
        private bool _capture;

        public TouchRecognizer(Element element, UIView view, TouchEffect touchEffect)
        {
            _element = element;
            _view = view;
            _touchEffect = touchEffect;

            _viewDictionary.Add(view, this);
        }

        public void Detach()
        {
            _viewDictionary.Remove(_view);
        }

        #region -- Overrides --

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();
                FireEvent(this, id, ETouchActionType.Pressed, touch, true);

                if (!_idToTouchDictionary.ContainsKey(id))
                {
                    _idToTouchDictionary.Add(id, this);
                }
            }

            _capture = _touchEffect.Capture;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (_capture)
                {
                    FireEvent(this, id, ETouchActionType.Moved, touch, true);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (_idToTouchDictionary[id] != null)
                    {
                        FireEvent(_idToTouchDictionary[id], id, ETouchActionType.Moved, touch, true);
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

                if (_capture)
                {
                    FireEvent(this, id, ETouchActionType.Released, touch, false);
                }
                else
                {
                    CheckForBoundaryHop(touch);

                    if (_idToTouchDictionary[id] != null)
                    {
                        FireEvent(_idToTouchDictionary[id], id, ETouchActionType.Released, touch, false);
                    }
                }

                _idToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                long id = touch.Handle.ToInt64();

                if (_capture)
                {
                    FireEvent(this, id, ETouchActionType.Cancelled, touch, false);
                }
                else if (_idToTouchDictionary[id] != null)
                {
                    FireEvent(_idToTouchDictionary[id], id, ETouchActionType.Cancelled, touch, false);
                }

                _idToTouchDictionary.Remove(id);
            }
        }

        #endregion

        #region -- Private helpers --

        private void CheckForBoundaryHop(UITouch touch)
        {
            long id = touch.Handle.ToInt64();

            TouchRecognizer recognizerHit = null;

            foreach (UIView view in _viewDictionary.Keys)
            {
                CGPoint location = touch.LocationInView(view);

                if (new CGRect(default(CGPoint), view.Frame.Size).Contains(location))
                {
                    recognizerHit = _viewDictionary[view];
                }
            }

            if (recognizerHit != _idToTouchDictionary[id])
            {
                if (_idToTouchDictionary[id] != null)
                {
                    FireEvent(_idToTouchDictionary[id], id, ETouchActionType.Exited, touch, true);
                }

                if (recognizerHit != null)
                {
                    FireEvent(recognizerHit, id, ETouchActionType.Entered, touch, true);
                }

                _idToTouchDictionary[id] = recognizerHit;
            }
        }

        private void FireEvent(TouchRecognizer recognizer, long id, ETouchActionType actionType, UITouch touch, bool isInContact)
        {
            CGPoint cgPoint = touch.LocationInView(recognizer.View);
            Point xfPoint = new Point(cgPoint.X, cgPoint.Y);

            Action<Element, TouchActionEventArgs> onTouchAction = recognizer._touchEffect.OnTouchAction;

            onTouchAction(recognizer._element, new TouchActionEventArgs(id, actionType, xfPoint, isInContact));
        }

        #endregion
    }
}