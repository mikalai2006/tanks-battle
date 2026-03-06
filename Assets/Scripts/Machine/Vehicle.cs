
using UnityEngine;

public class Vehicle
{
    public DataVehicle Data {get; private set; }
    public GameMachine Config {get; private set; }
    public BaseMachine BaseMachine {get; private set; }

    public Vehicle(GameMachine config, Vector3 pointSpawn, Transform parent)
    {
        Config = config;

        Data = new();

        
    }

    public void SpawnGameObject(Vector3 pointSpawn, Transform parent)
    {
        var gObject = Object.Instantiate(
            Config.machinePrefab,
            pointSpawn,
            //new Vector3(data.isBot ? 30 : 241, 0.5f, data.isBot ? 30 : 22),
            // new Vector3(node.position.x, 0.5f, node.position.y),
            Quaternion.identity,
            parent
        );
        // gObject.name = $"{Config.name}_{Data.id}";
        

        BaseMachine = gObject.GetComponent<BaseMachine>();
    }

    public void DestroyGameObject()
    {
        
    }
}