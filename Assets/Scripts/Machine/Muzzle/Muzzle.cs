
using UnityEngine;

public class Muzzle : BaseMuzzle
{

    public override void Update()
    {
        base.Update();

        // if (
        //     data.timeBeforeShot <= 0
        //     && Tower
        //     && Tower.ObjectTarget
        //     && Tower.Data.isShot
        // )
        // {
        //     data.countShotSeria += 1;
        //     OnShot(Tower.ObjectTarget.gameObject);
        // }
    }

    public override void OnShot(GameObject target)
    {
        if (!Machine)
        {
            return;
        }

        // base.OnShot(target);

        // Addressables.InstantiateAsync(
        //     Machine.Config.bullet.prefab,
        //     Machine.transform.position,
        //     Quaternion.identity,
        //     Machine.transform.parent
        // ).Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle);
        // GameObject obj = Lean.Pool.LeanPool.Spawn(Config.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);
        // GameObject obj = Instantiate(Config.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);
        GameObject obj = Machine.LevelManager.poolBullet.GetObject();
        obj.transform.position = pointEffects.transform.position;
        // obj.transform.parent = Machine.LevelManager.objectSpawnEffect.transform;
        BaseBullet objBullet = obj.GetComponent<BaseBullet>();
        if (objBullet != null)
        {
            // // Преобразуем угол в радианы
            // float angleRad = Machine.Tower.transform.rotation.z * Mathf.Deg2Rad;

            // // Рассчитываем вектор направления (x, y)
            // float x = Mathf.Cos(angleRad) * .5f;
            // float y = Mathf.Sin(angleRad) * .5f;

            // // Создаем вектор направления
            // Vector3 direction = new Vector2(x, y);
            // Vector3 rotatedOffset = Machine.Tower.transform.rotation * direction; // Преобразуем локальный сдвиг в мировой

            // obj.transform.localPosition = Machine.Tower.transform.position + rotatedOffset;
            objBullet.OnInit(Machine, Tower, this, Config);
            // Lean.Pool.LeanPool.Despawn(obj, 2);
        }
    }
    
    
}
