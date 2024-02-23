using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    [SerializeField] private Transform hand;
    Vector2 rotation = Vector2.zero;

    public float interactDist = 5f;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        rotation += InputManager.Instance.LookInput * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;

        if (InputManager.Instance.InteractPressedThisFrame)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Debug.DrawRay(ray.origin, ray.direction * interactDist, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDist))
            {
                if (hit.transform.TryGetComponent<InteractableItem>(out var interactable))
                {
                    interactable.Interact(hand);
                }
            }
            else
            {
                if (hand.childCount > 0)
                {
                    hand.GetChild(0).TryGetComponent<InteractableItem>(out var obj);
                    Debug.Log("Trying to detach " + obj);
                    obj.DetachFromHand();
                }

            }
        }
    }

}
