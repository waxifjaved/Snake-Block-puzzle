using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSnake : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3[] prevPosition = new Vector3[3];
    float walkSpeed;
    float inputHorizontal;

    float timeSc = 0;

    Vector2 touchStartPosition;
    Vector2 touchEndPosition;
    bool isDragging = false;

    float minX; // Left screen boundary
    float maxX; // Right screen boundary

    public Vector3[] PrevPosition
    {
        get
        {
            return prevPosition;
        }
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        walkSpeed = 10.0f;
        timeSc = Time.timeScale;
        prevPosition[0] = new Vector3(0.0f, 0.0f, 0.0f);
        prevPosition[1] = new Vector3(0.0f, 0.0f, 0.0f);
        prevPosition[2] = transform.position;

        // Calculating the world-space boundaries of the screen
        Vector3 screensBoundsLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screensBoundsRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));

        // Minimum and Maximum values for Horizontal Limits
        minX = screensBoundsLeft.x;
        maxX = screensBoundsRight.x;
    }

    void Update()
    {
        // Update previous positions for snake movement
        prevPosition[0] = prevPosition[1];
        prevPosition[1] = prevPosition[2];
        prevPosition[2] = transform.position;

        // Increase speed over time
        walkSpeed += (Time.timeScale - timeSc) * 2.0f;
        timeSc = Time.timeScale;

        // Handle input - either keyboard (for testing in the editor) or touch input (for mobile)
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleKeyboardInput();
#elif UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
#endif




        // Clamp the snake's position
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, minX, maxX);
    }



    // Keyboard input for testing in the Unity editor
    void HandleKeyboardInput()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        if (inputHorizontal != 0)
        {
            rb.AddForce(new Vector2(inputHorizontal * walkSpeed, 0f));
        }
    }

    // Mobile touch input handling for swiping and dragging
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Store the start position of the touch
                    touchStartPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        // Calculate movement based on touch movement
                        Vector2 touchDelta = touch.deltaPosition;
                        rb.AddForce(new Vector2(touchDelta.x * walkSpeed * Time.deltaTime, 0f));
                    }
                    break;

                case TouchPhase.Ended:
                    // Reset dragging state when touch ends
                    touchEndPosition = touch.position;
                    isDragging = false;
                    break;
            }
        }
    }
}
