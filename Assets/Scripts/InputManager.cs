using UnityEngine;
using UnityEngine.EventSystems;

namespace SapperChain
{
    public class InputManager : MonoBehaviour
    {
        private bool mobile;

        private Vector2 touchStartPos;
        private float minMouseWheelDeltaForPinch = 0.1f;
        private bool isDragging = false;
        private float initialDistance;

        public delegate void ClickHandler(bool hold, Vector2 pos);
        public static event ClickHandler Click;

        private void Awake()
        {
            RuntimePlatform platform = Application.platform;
            mobile = platform == RuntimePlatform.Android;
        }

        private void Update()
        {
            if (!isDragging && _holdTimer != -1)
            {
                _holdTimer += Time.deltaTime;
            }
            if (!mobile)
            {
                float mouseWheelInput = Input.GetAxis("Mouse ScrollWheel");

                if (Mathf.Abs(mouseWheelInput) >= minMouseWheelDeltaForPinch)
                {
                    if (mouseWheelInput > 0)
                    {
                        Resize(-0.2f);
                    }
                    else if (mouseWheelInput < 0)
                    {
                        Resize(0.2f);
                    }
                }

                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                if (Input.GetMouseButtonDown(0))
                {
                    HandleClickOrDragStart(Input.mousePosition);
                }
                else if (Input.GetMouseButton(0))
                {
                    HandleDrag(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    HandleClickOrDragEnd();
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);

                        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            switch (touch.phase)
                            {
                                case TouchPhase.Began:
                                    HandleClickOrDragStart(touch.position);
                                    break;

                                case TouchPhase.Moved:
                                case TouchPhase.Stationary:
                                    HandleDrag(touch.position);
                                    break;

                                case TouchPhase.Ended:
                                case TouchPhase.Canceled:
                                    HandleClickOrDragEnd();
                                    break;
                            }
                        }
                    }
                    else
                    {

                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);

                        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                        {
                            initialDistance = Vector2.Distance(touch1.position, touch2.position);
                        }
                        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                            Resize((initialDistance - currentDistance)* 0.005f);
                        }
                    }
                }
            }

            
        }
        private void Resize(float delta)
        {
            float size = Camera.main.orthographicSize + delta;
            size = Mathf.Clamp(size, 1, 7);
            Camera.main.orthographicSize = size;
        }

        private void HandleClickOrDragStart(Vector2 startPos)
        {
            touchStartPos = startPos;
            isDragging = false;
            _holdTimer = 0;
        }

        private float _holdTimer = -1;
        [SerializeField] private float _holdTime;

        private void HandleDrag(Vector2 currentPos)
        {
            if (!isDragging && Vector2.Distance(touchStartPos, currentPos) > 10f)
            {
                isDragging = true;
                _holdTimer = -1;
            }
            else if (!isDragging && _holdTimer > _holdTime)
            {
                Click?.Invoke(true, Camera.main.ScreenToWorldPoint(touchStartPos));
                _holdTimer = -1;
            }

            if (isDragging)
            {
                Vector2 dragDelta = currentPos - touchStartPos;
                Camera.main.transform.position += (Vector3)dragDelta.normalized * 5 * Time.deltaTime;
            }
        }

        private void HandleClickOrDragEnd()
        {
            if (!isDragging)
            {
                if (_holdTimer != -1)
                    Click?.Invoke(false, Camera.main.ScreenToWorldPoint(touchStartPos));
                _holdTimer = -1;
            }

            isDragging = false;
        }
    }
}