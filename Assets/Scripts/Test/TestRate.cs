using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class TestRate : MonoBehaviour
{
    [SerializeField] private BaseMachine targetObject;
    [SerializeField] private int countClone;

    [SerializeField] private List<BaseMachine> _machines;
    
    void Awake()
    {
        for (int i = 0; i < countClone; i++)
        {
            Vector3 _transform = transform.position + new Vector3(Random.Range(2, 30), Random.Range(2, 30), 0);
            BaseMachine obj = Instantiate(targetObject, transform);
            obj.GetComponent<PlayerController>().enabled = false;
            obj.GetComponent<PlayerInput>().enabled = false;
            obj.GetComponentInChildren<Light2D>().enabled = false;
            obj.Areol.SetActive(false);
            obj.GetComponent<StateController>().enabled = true;
            obj.transform.position = _transform;

            _machines.Add(obj);
        }
    }
}
