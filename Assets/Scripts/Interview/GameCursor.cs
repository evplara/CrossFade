using UnityEngine;
using UnityEngine.EventSystems;

public class GameCursor : MonoBehaviour
{

    [SerializeField] private float lethargyMultiplier = 0.5f;
    [SerializeField] private RectTransform cursorReference;
    [SerializeField] private Canvas canvas;

    private Vector2 cursorPosition;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2); 
    }

    private void Update()
    {
        Vector2 delta = new Vector2(
             Input.GetAxis("Mouse X"),
             Input.GetAxis("Mouse Y")
         );

        cursorPosition += delta * lethargyMultiplier * 10f;

        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);

        // Correctly convert screen position to canvas local position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            cursorPosition,
            null, // null = Screen Space Overlay, use camera ref if Screen Space Camera
            out localPoint
        );
        cursorReference.localPosition = localPoint;

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = cursorPosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
            }

        }
    }
}


