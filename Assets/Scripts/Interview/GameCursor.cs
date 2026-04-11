using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//script assisted with AI
public class GameCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private Camera renderCamera;

    [Header("Movement")]
    [Range(0f, 1f)]
    [SerializeField] private float cursorSpeed = 1f;

    private Vector2 screenPosition;
    private GameObject pressedObject;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 

        screenPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        ApplyCursorPosition();
    }

    void Update()
    {
        MoveCursor();
        HandleClick();
    }

    void MoveCursor()
    {
        Vector2 mouseDelta = Input.mousePositionDelta;

        screenPosition += mouseDelta * cursorSpeed;

        screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

        ApplyCursorPosition();
    }

    void ApplyCursorPosition()
    {
        Ray ray = renderCamera.ScreenPointToRay(screenPosition);
        Plane canvasPlane = new Plane(-canvas.transform.forward, canvas.transform.position);
        if (canvasPlane.Raycast(ray, out float distance))
            cursorTransform.position = ray.GetPoint(distance);
    }

    PointerEventData CreatePointerData()
    {
        return new PointerEventData(EventSystem.current) { position = screenPosition };
    }

    List<RaycastResult> Raycast(PointerEventData pointerData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        return results;
    }

    void HandleClick()
    {
        PointerEventData pointerData = CreatePointerData();

        if (Input.GetMouseButtonDown(0))
        {
            var results = Raycast(pointerData);
            if (results.Count > 0)
            {
                pressedObject = ExecuteEvents.GetEventHandler<IPointerDownHandler>(results[0].gameObject);
                if (pressedObject != null)
                    ExecuteEvents.Execute(pressedObject, pointerData, ExecuteEvents.pointerDownHandler);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pressedObject != null)
            {
                ExecuteEvents.Execute(pressedObject, pointerData, ExecuteEvents.pointerUpHandler);

                var results = Raycast(pointerData);
                if (results.Count > 0)
                {
                    GameObject clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(results[0].gameObject);
                    if (clickHandler == pressedObject)
                        ExecuteEvents.Execute(pressedObject, pointerData, ExecuteEvents.pointerClickHandler);
                }

                pressedObject = null;
            }
        }
    }

    void OnDestroy()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}


