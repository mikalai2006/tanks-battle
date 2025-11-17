using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraRemoveBackObjects : MonoBehaviour
{
    private GameManager _gameManager => GameManager.Instance;
    private CinemachineCamera cinemachineCamera;
    // private Transform Obstruction;
    [SerializeField] private List<GameObject> prevHideObjects;
    [SerializeField] private List<Material> cacheMaterial;
    [SerializeField] public LayerMask layerMask;
    void Start()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
        prevHideObjects = new();
    }

    void Update()
    {
        if (prevHideObjects.Count > 0)
        {
            for (int i = 0; i < prevHideObjects.Count(); i++)
            {
                // prevHideObjects[i].gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                // var gpuIE = prevHideObjects[i].gameObject.GetComponentInChildren<GPUInstanceEnabler>();
                // if (gpuIE != null)
                // {
                //     // go.gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                //     Color _color = gpuIE.color1;
                //     _color.a = 1f;
                //     gpuIE.SetColor(_color);
                // }

                if (prevHideObjects[i] && prevHideObjects[i].gameObject) {
                    prevHideObjects[i].layer = LayerMask.NameToLayer("Wall");

                    var goRenderer = prevHideObjects[i].gameObject.GetComponent<Renderer>();
                    if (goRenderer != null)
                    {
                        goRenderer.material = cacheMaterial[i];
                    }
                }
                prevHideObjects.RemoveAt(i);
                cacheMaterial.RemoveAt(i);
                // for (int i = 0; i < prevHideObjects.Count; i++)
                // {
                //     prevHideObjects[i].SetActive(true);
                //     prevHideObjects.RemoveAt(i);
                // }
            }
        }

        if (cinemachineCamera.Target.TrackingTarget != null)
        {
            Vector3 targetPos = cinemachineCamera.Target.TrackingTarget.position;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, targetPos - transform.position, Vector3.Distance(targetPos, transform.position), layerMask);
            // Debug.DrawRay(transform.position, targetPos - transform.position, Color.red);
            for (int i = 0; i < hits.Count(); i++)
            {
                GameObject go = hits[i].collider.gameObject;
                go.layer = LayerMask.NameToLayer("TransparentFX");
                // var gpuIE = go.gameObject.GetComponentInChildren<GPUInstanceEnabler>();
                var goRenderer = go.gameObject.GetComponent<Renderer>();
                if (goRenderer != null)
                {
                    // // go.gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    // Color _color = gpuIE.color1;
                    // _color.a = 0.1f;
                    // gpuIE.SetColor(_color);
                    if (!prevHideObjects.Contains(go))
                    {
                        prevHideObjects.Add(go);
                        cacheMaterial.Add(goRenderer.material);
                    }
                    goRenderer.material = _gameManager.Settings.materialTransparent;
                }

                // if (Vector3.Distance(Obstruction.position, transform.position) >= 3f && Vector3.Distance(transform.position, ))
                // Debug.Log($"Linecast::: {hits[i].collider.name}");
                // prevHideObjects.Add(hit.collider.gameObject);
                // hit.collider.gameObject.SetActive(false);
            }
        }
    }
    

    //     void Update()
    // {   
    //         if (prevHideObjects.Count > 0)
    //         {
    //         for (int i = 0; i < prevHideObjects.Count(); i++)
    //         {
    //             // prevHideObjects[i].gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    //             var gpuIE = prevHideObjects[i].gameObject.GetComponentInChildren<GPUInstanceEnabler>();
    //             if (gpuIE != null)
    //             {
    //                 // go.gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //                 Color _color = gpuIE.color1;
    //                 _color.a = 1f;
    //                 gpuIE.SetColor(_color);
    //             }
    //             prevHideObjects.RemoveAt(i);
    //                 // for (int i = 0; i < prevHideObjects.Count; i++)
    //             // {
    //             //     prevHideObjects[i].SetActive(true);
    //             //     prevHideObjects.RemoveAt(i);
    //             // }
    //         }
    //         }

    //     Vector3 targetPos = cinemachineCamera.Target.TrackingTarget.position;
    //     RaycastHit[] hits = Physics.RaycastAll(transform.position, targetPos - transform.position, 999f, layerMask);
    //     Debug.DrawRay(transform.position, targetPos - transform.position,Color.red);
    //     for (int i = 0; i < hits.Count(); i++)
    //     {
    //         GameObject go = hits[i].collider.gameObject;
    //         var gpuIE = go.gameObject.GetComponentInChildren<GPUInstanceEnabler>();
    //         if (gpuIE != null)
    //         {
    //             // go.gameObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //             Color _color = gpuIE.color1;
    //             _color.a = 0.1f;
    //             gpuIE.SetColor(_color);
    //             if (!prevHideObjects.Contains(go))
    //             {
    //                 prevHideObjects.Add(go);
    //             }
    //         }

    //         // if (Vector3.Distance(Obstruction.position, transform.position) >= 3f && Vector3.Distance(transform.position, ))
    //         // Debug.Log($"Linecast::: {hits[i].collider.name}");
    //         // prevHideObjects.Add(hit.collider.gameObject);
    //         // hit.collider.gameObject.SetActive(false);
    //     }
    // }
}


[System.Serializable]
public struct ObjectCache
{
    public GameObject obj;
    public Material mat;
}