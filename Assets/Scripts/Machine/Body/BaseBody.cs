using Mikalai2006.Voxel;
using UnityEngine;

// [RequireComponent(typeof(GPUInstanceEnabler))]
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

    public void OnCollision(Vector3 _pointCollision, bool isDrawMesh, GameObject explodeGameObject)
    {
        for (int i = 0; i < voxelMeshRender.Containers.Length; i++)
        {
            voxelMeshRender.Containers[i].ExposionVoxels(_pointCollision, isDrawMesh, explodeGameObject);
        }
    }

    public void OnSetSpriteGerb(Sprite sprite)
    {
        // _bodyGerbSprite.sprite = sprite;
    }
}
