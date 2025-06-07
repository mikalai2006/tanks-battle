// using System.Linq;
// using UnityEngine;

// public class TestRay : MonoBehaviour
// {
//     [SerializeField] private BaseMachine[] _machines;
//     [SerializeField] private BaseMachine player;

//     void Start()
//     {
//         Init();
//     }

//     // Update is called once per frame
//     void OnDrawGizmos()
//     {
//         if (_machines.Length == 0)
//         {
//             Init();
//         }

//         foreach (BaseMachine machine in _machines)
//         {
//             float dist = Vector3.Distance(machine.transform.position, player.transform.position);
//             if (dist < 4)
//             {
//                 Gizmos.DrawLine(player.transform.position, machine.transform.position);
//                 player.OnSetTarget(machine);
//                 machine.OnSetTarget(player);
//             }
//         }
//     }

//     void Init()
//     {
//         var m = transform.GetComponentsInChildren<BaseMachine>().Where(m => m.MachineLevelData.isBot == false);
//         if (m.Count() > 0)
//         {
//             player = m.First();
//         }
//         else
//         {
//             Debug.LogWarning($"Not found player in {name}");
//         }
//         _machines = transform.GetComponentsInChildren<BaseMachine>().Where(m => m.MachineLevelData.isBot == true).ToArray();
//     }
// }
