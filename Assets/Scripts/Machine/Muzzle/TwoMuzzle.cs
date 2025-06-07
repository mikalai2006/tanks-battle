
using UnityEngine;

public class TwoMuzzle : BaseMuzzle
{

    public override void Update()
    {
        base.Update();
      
        if (data.timeBeforeShot <= 0 && Machine && Machine.ObjectTarget && Machine.Data.isShot)
        {
            data.countShotSeria += 1;
            OnShot(Machine.ObjectTarget.gameObject);
        }
    }

    public override void OnShot(GameObject target)
    {
        if (!Machine)
        {
            return;
        }

        base.OnShot(target);

        // Addressables.InstantiateAsync(
        //     Machine.Config.bullet.prefab,
        //     Machine.transform.position,
        //     Quaternion.identity,
        //     Machine.transform.parent
        // ).Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle);
        var obj = Instantiate(Config.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);
        //Lean.Pool.LeanPool.Spawn(Machine.Config.Muzzle.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);

        // Преобразуем угол в радианы
        float angleRad = Machine.Tower.transform.rotation.z * Mathf.Deg2Rad;

        // Рассчитываем вектор направления (x, y)
        float x = Mathf.Cos(angleRad) * .5f;
        float y = Mathf.Sin(angleRad) * .5f;

        // Создаем вектор направления
        Vector3 direction = new Vector2(x, y);
        Vector3 rotatedOffset = Machine.Tower.transform.rotation * direction; // Преобразуем локальный сдвиг в мировой

        // obj.transform.localPosition = Machine.Tower.transform.position + rotatedOffset;
        obj.transform.position = pointEffects.transform.position;
        obj.OnInit(Machine, Config);
    }
    
    
}