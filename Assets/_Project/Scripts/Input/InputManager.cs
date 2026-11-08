using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;



public class InputManager : MonoBehaviour
{
    public event Action<Vector2> OnFingerMoved;
    public event Action<Vector2> OnFingerBegan;
    public event Action<Vector2> OnFingerEnded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var touch in Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnFingerBegan?.Invoke(TrackFingerStartPosition(touch));
                    break;

                case TouchPhase.Moved:
                    OnFingerMoved?.Invoke(TrackFingerPosition(touch));
                    break;

                case TouchPhase.Ended:
                    OnFingerEnded?.Invoke(TrackFingerEndPosition(touch));
                    break;

                case TouchPhase.Canceled:
                    OnFingerEnded?.Invoke(TrackFingerEndPosition(touch)); 
                    break;
            }
        }
    }

    public Vector2 TrackFingerPosition(Touch touch)
    {
        return touch.screenPosition;
    }

    public Vector2 TrackFingerStartPosition(Touch touch)
    {
        return touch.startScreenPosition;
    }

    public Vector2 TrackFingerEndPosition(Touch touch)
    {
        return touch.screenPosition;
    }

    public void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}
