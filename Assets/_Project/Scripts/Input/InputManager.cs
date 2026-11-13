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
    public event Action<bool> SwipeDetected;
    public event Action<Vector2> OnTap;

    [SerializeField] private float swipeRange = 20f;

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
                    SwipeDetected?.Invoke(IsSwipe(touch));
                    break;

                case TouchPhase.Ended:
                    OnFingerEnded?.Invoke(TrackFingerEndPosition(touch));

                    if (touch.isTap == true)
                    {
                        OnTap?.Invoke(touch.screenPosition);
                    }
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

    public bool IsSwipe(Touch touch)
    {
        Vector2 startPos = touch.startScreenPosition;
        Vector2 endPos = touch.screenPosition;

        float distance = Vector2.Distance(startPos, endPos);

        if(distance >= swipeRange)
        {
            return true;
        }
        return false;
    }

    public void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
}
