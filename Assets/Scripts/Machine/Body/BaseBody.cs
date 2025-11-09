using Cysharp.Threading.Tasks;
using Mikalai2006.Voxel;
using UnityEngine;

public class BaseBody : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _bodyGerbSprite;
    [SerializeField] private SpriteRenderer _damageSprite;
    protected BaseMachine Machine;
    protected GameBody Config;
    [SerializeField] protected VoxelMeshRender voxelMeshRender;

    public void Init(BaseMachine _machine)
    {
        Machine = _machine;

        Config = Machine.Config.body;

        OnChangeData();

        voxelMeshRender.OnSetConfigMeshGenerator(Config.MeshConfig);

        // _bodySprite.color = Machine.Config.colorBody;
        // _bodySprite.sprite = Machine.Config.body.spriteBody;
    }
    
    public void OnChangeData()
    {
        // Color col = Color.white;
        // col.a = 1f - Mathf.Min(1f, Machine.Data.hp * 100f / Machine.Config.hp * 0.01f);

        // _damageSprite.color = col;
    }

    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject, int damageRadius, Vector3 direction)
    {
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            if (voxelMeshRender.Containers[i].IsDestructible())
            {
                Vector3 localPoint = voxelMeshRender.Containers[i].transform.InverseTransformPoint(_pointCollision);
                // Debug.Log($"<color=green>Body OnCollision: {_pointCollision} / {localPoint}</color>");
                voxelMeshRender.Containers[i].ExposionVoxels(localPoint, isDrawMesh, explodeGameObject, damageRadius, direction).Forget();
            }
        }
    }

    public void OnSetSpriteGerb(Sprite sprite)
    {
        // _bodyGerbSprite.sprite = sprite;
    }
}
