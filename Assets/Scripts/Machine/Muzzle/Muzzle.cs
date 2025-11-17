
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Muzzle : BaseMuzzle
{

    // public override void Update()
    // {
    //     base.Update();

    //     // if (
    //     //     data.timeBeforeShot <= 0
    //     //     && Tower
    //     //     && Tower.ObjectTarget
    //     //     && Tower.Data.isShot
    //     // )
    //     // {
    //     //     OnShot(Tower.ObjectTarget.gameObject);
    //     // }
    // }

    async public override UniTask OnGoShot(System.Threading.CancellationTokenSource cancelToken)
    {
        if (!cancelToken.IsCancellationRequested)
        {
            if (!Machine)
            {
                return;
            }

            await base.OnGoShot(cancelToken);

            // base.OnShot(target);

            // Addressables.InstantiateAsync(
            //     Machine.Config.bullet.prefab,
            //     Machine.transform.position,
            //     Quaternion.identity,
            //     Machine.transform.parent
            // ).Completed += (AsyncOperationHandle<GameObject> handle) => LoadedAsset(handle);
            // GameObject obj = Lean.Pool.LeanPool.Spawn(Config.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);
            // GameObject obj = Instantiate(Config.Bullet.prefab, Machine.LevelManager.objectSpawnEffect.transform, false);

            RecoilEffect(cancelToken).Forget();

            GameObject obj = Machine.LevelManager.poolBullet.GetObject();
            obj.transform.position = PointEffects.transform.position;
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

            // ждем время до установки статуса на свободный
            var time = Mathf.Max(0, Data.timeBetweenShot - (_data.index * Delay));
            OnSetTimeBetweenShot(time);
            await UniTask.Delay(System.TimeSpan.FromSeconds(time), cancellationToken: cancelToken.Token);
            SetBusy(false);
        }
    }

    /// <summary>
    /// Функция эффекта отдачи при выстреле.
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    async private UniTask RecoilEffect(System.Threading.CancellationTokenSource cancelToken)
    {
        Vector3 startPosition = Wrapper.transform.localPosition;
        Vector3 forw = transform.InverseTransformDirection(transform.forward);
        Vector3 endPoint = startPosition - forw * 3f;
        // Debug.DrawLine(startPosition, endPoint, Color.blue, 3);

        // Debug.Log($"startPosition={startPosition}, forw={forw}<{transform.forward}>[tower fw={Tower.transform.forward}], endPoint={endPoint}");

        while (Vector3.Distance(Wrapper.transform.localPosition, endPoint) > 0.1f)
        {
            // Debug.Log($"dist1={Vector3.Distance(Wrapper.transform.localPosition, endPoint)}");
            Wrapper.transform.localPosition = Vector3.MoveTowards(Wrapper.transform.localPosition, endPoint, 100 * Time.deltaTime);
            await UniTask.Yield(cancellationToken: cancelToken.Token); // Await the next frame
        }

        // await UniTask.Delay(System.TimeSpan.FromSeconds(.5f), cancellationToken: cancelToken.Token);

        while (Vector3.Distance(Wrapper.transform.localPosition, startPosition) > 0.1f) // Move until very close to target
        {
            // Debug.Log($"dist2={Vector3.Distance(Wrapper.transform.localPosition, startPosition)}");
            Wrapper.transform.localPosition = Vector3.MoveTowards(Wrapper.transform.localPosition, startPosition, 10 * Time.deltaTime);
            await UniTask.Yield(cancellationToken: cancelToken.Token); // Await the next frame
        }

        Wrapper.transform.localPosition = Vector3.zero;
    }
}
