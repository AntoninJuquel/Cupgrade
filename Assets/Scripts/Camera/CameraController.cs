using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] Transform player;
    [SerializeField] float cameraMoveSpeed = 1f;
    Vector3 cameraFollowPosition;
    Camera viewCamera;

    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
        viewCamera = GetComponent<Camera>();
        Instance = this;
    }

    private void Update()
    {
        Vector3 cursorPosition = viewCamera.ScreenToWorldPoint(Input.mousePosition);

        cameraFollowPosition = (cursorPosition + player.transform.position) / 2f;
        cameraFollowPosition.z = transform.position.z;

        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        if (distance > 0)
        {
            Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;
            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);
            if (distanceAfterMoving > distance)
            {
                newCameraPosition = cameraFollowPosition;
            }
            transform.position = newCameraPosition;
        }
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            if (this!=null)
                transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    /*[Header("Specs")]
    [SerializeField] float damping = 12.0f;
    [SerializeField] float height = 13.0f;
    [SerializeField] float viewDistance = 3.0f;

    Camera viewCamera;
    Vector3 center;

    private void Start()
    {
        viewCamera = GetComponent<Camera>();
        transform.position = player.position;
    }

    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = viewDistance; // Control max distance

        Vector3 cursorPosition = viewCamera.ScreenToWorldPoint(mousePos);

        center = new Vector3((player.position.x + cursorPosition.x) / 2, (player.position.y + cursorPosition.y) / 2,player.position.z);

        transform.position = Vector3.Lerp(transform.position, center + new Vector3(0, 0, height), Time.deltaTime * damping);
    }

    */
}
