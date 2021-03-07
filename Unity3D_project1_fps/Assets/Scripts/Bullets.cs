using UnityEngine;

public class Bullets : MonoBehaviour
{
    /// <summary>
    /// 該發子彈攻擊力
    /// </summary>
    public float pwr;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
