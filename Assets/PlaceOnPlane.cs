using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject m_PlacedPrefab;

    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    public GameObject spawnedObject { get; private set; }

    private ARRaycastManager m_RaycastManager;

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Perform a raycast from the touch position
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Get the hit pose
                    var hitPose = s_Hits[0].pose;

                    if (m_PlacedPrefab != null)
                    {
                        // Destroy the existing object if any
                        if (spawnedObject != null)
                        {
                            Destroy(spawnedObject);
                        }

                        // Instantiate the new object at the hit position
                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    }
                }
            }
        }

        if (spawnedObject != null)
        {
            HandlePinchToScale(); // Handle scaling if the object is spawned
        }
    }

    private void HandlePinchToScale()
    {
        if (Input.touchCount < 2)
            return;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
        float pinchAmount = deltaMagnitudeDiff * 0.01f; // Adjust sensitivity

        spawnedObject.transform.localScale -= new Vector3(pinchAmount, pinchAmount, pinchAmount);

        // Clamp the scale values
        float Min = 0.005f;
        float Max = 3f;
        spawnedObject.transform.localScale = new Vector3(
            Mathf.Clamp(spawnedObject.transform.localScale.x, Min, Max),
            Mathf.Clamp(spawnedObject.transform.localScale.y, Min, Max),
            Mathf.Clamp(spawnedObject.transform.localScale.z, Min, Max)
        );
    }

    public void SetPrefab(GameObject prefab)
    {
        m_PlacedPrefab = prefab;
    }

    public void ClearSpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
}
