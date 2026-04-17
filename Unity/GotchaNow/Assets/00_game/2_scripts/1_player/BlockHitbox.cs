using UnityEngine;
using UnityEngine.VFX;

public class BlockHitbox : MonoBehaviour
{
    public BlockScript blockScript;
    public VisualEffect parryVFX;
    
    public void ParryVFX()
    {
        Instantiate(parryVFX, transform.position, Quaternion.identity);
    }
}
