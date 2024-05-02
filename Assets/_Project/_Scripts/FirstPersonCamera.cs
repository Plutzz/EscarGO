using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCamera : NetworkBehaviour
{
    private InputManager inputManager;

    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }

    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    [SerializeField] private Transform hand;
    public Vector2 rotation = Vector2.zero;

    public float interactDist = 5f;
    [SerializeField]
    private Camera cam;

    [SerializeField] private Slider sensitivitySlider;
    private const string SensitivityPrefsKey = "Sensitivity";

    public override void OnNetworkSpawn()
    {
        // If this script is not owned by the client
        // Delete it so no input is picked up by it
        if (!IsOwner)
            Destroy(gameObject);

        inputManager = transform.parent.parent.GetComponent<InputManager>();

        sensitivity = PlayerPrefs.GetFloat(SensitivityPrefsKey, sensitivity);

        if(sensitivitySlider != null)
        {
            sensitivitySlider.value = sensitivity;
        }
    }

    void Update()
    {
        rotation += inputManager.LookInput * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;

        if (sensitivitySlider != null && sensitivitySlider.value != sensitivity)
        {
            UpdateSensitivity(sensitivitySlider.value);
        }

        //if (InputManager.Instance.InteractPressedThisFrame)
        //{
        //    Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //    Debug.DrawRay(ray.origin, ray.direction * interactDist, Color.red);

        //    if (hand.childCount > 0)
        //    {
        //        hand.GetChild(0).TryGetComponent<InteractableItem>(out var obj);
        //        Debug.Log("Trying to detach " + obj);
        //        obj.DetachFromHand();
        //    }
        //    if (Physics.Raycast(ray, out RaycastHit hit, interactDist))
        //    {
        //        if (hit.transform.TryGetComponent<InteractableItem>(out var interactable))
        //        {
        //            interactable.Interact(hand);
        //        }
        //    }

        //}

        void UpdateSensitivity(float value)
        {
            sensitivity = value;

            PlayerPrefs.SetFloat(SensitivityPrefsKey, sensitivity);
            PlayerPrefs.Save();
        }

    }

}
