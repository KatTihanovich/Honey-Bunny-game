using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour
{
    private Vector3 originalPos;
    private Quaternion originalRot;
    private bool focusing = false;

    public float moveSpeed = 3f;
    public float returnDelay = 1.5f;

    public PlayerController player;
    public CameraFollow cameraFollow;

    private float zoomZ = -25f; // must be negative

    public IEnumerator FocusOnObject(Transform target, float offsetX, float offsetY)
    {
        if (focusing) yield break;
        focusing = true;

        if (player != null)
            player.enabled = false;

        if (cameraFollow != null)
            cameraFollow.enabled = false;

        originalPos = transform.position;
        originalRot = transform.rotation; 

        Vector3 targetPos = new Vector3(
            target.position.x + offsetX,
            target.position.y + offsetY,
            zoomZ
        );

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(originalPos, targetPos, t);

            transform.rotation = originalRot;

            yield return null;
        }

        yield return new WaitForSeconds(returnDelay);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(targetPos, originalPos, t);

            transform.rotation = originalRot;

            yield return null;
        }

        if (cameraFollow != null)
            cameraFollow.enabled = true;

        if (player != null)
            player.enabled = true;

        focusing = false;
    }
}
