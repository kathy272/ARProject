using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
    /// <summary>
    /// Behavior with an API for spawning objects from a given set of prefabs.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private GameObject m_SpawnVisualizationPrefab;
        [SerializeField] private bool m_OnlySpawnInView = true;
        [SerializeField] private float m_ViewportPeriphery = 0.15f;
        [SerializeField] private bool m_SpawnAsChildren;

        public bool onlySpawnInView
        {
            get => m_OnlySpawnInView;
            set => m_OnlySpawnInView = value;
        }

        public float viewportPeriphery
        {
            get => m_ViewportPeriphery;
            set => m_ViewportPeriphery = value;
        }

        public bool spawnAsChildren
        {
            get => m_SpawnAsChildren;
            set => m_SpawnAsChildren = value;
        }

        public event Action<GameObject> objectSpawned;

        [SerializeField]
        private float m_SpawnedObjectScale = 30f; // Add this field to control the scale of the spawned object

        public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
        {
            if (_gameObject == null)
            {
                Debug.LogError("Object prefab is not assigned.");
                return false;
            }

            var newObject = Instantiate(_gameObject);

            // Scale the new object
            newObject.transform.localScale = new Vector3(m_SpawnedObjectScale, m_SpawnedObjectScale, m_SpawnedObjectScale);

            if (m_SpawnAsChildren)
                newObject.transform.parent = transform;

            newObject.transform.position = spawnPoint;

            if (m_SpawnVisualizationPrefab != null)
            {
                var visualizationTrans = Instantiate(m_SpawnVisualizationPrefab).transform;
                visualizationTrans.position = spawnPoint;
                visualizationTrans.rotation = newObject.transform.rotation;
            }

            objectSpawned?.Invoke(newObject);
            return true;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    TrySpawnObject(hit.point, hit.normal);
                }
            }
#endif
        }
    }
}