using UnityEngine;

public class FPSContraler : MonoBehaviour
{
    #region 基本欄位
    [Header("移動速度"), Range(0, 200)]
    public float speed;
    [Header("轉動速度"), Range(0, 1000)]
    public float turnSpeed;
    [Header("跳躍力道"), Range(0, 1000)]
    public float jumpForce;
    [Header("地板偵測")]
    public Vector3 floorOffset;
    [Header("地板偵測半徑"), Range(0, 10)]
    public float floorRadius = 1;

    private Animator ani;
    private Rigidbody rig;
    #endregion

    #region 開槍欄位
    [Header("子彈")]
    public GameObject bullet;
    [Header("子彈生成點")]
    public Transform bulletSpawn;
    [Header("子彈目前數量")]
    public int bulletCurrent;
    [Header("子彈總數")]
    public int bulletTotal;
    [Header("子彈速度")]
    public int bulletSpeed;
    #endregion

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();

        // 隱藏鼠標
        Cursor.visible = false;
    }

    private void OnDrawGizmos()
    {
        // 畫出 物理地板偵測範圍 (debug用 不會顯示在Game畫面)
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position + floorOffset, floorRadius);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        // 取 前後值
        float v = Input.GetAxis("Vertical");
        // 取 左右值
        float h = Input.GetAxis("Horizontal");

        // 前後移動
        rig.MovePosition(transform.position + transform.forward * v * speed * Time.deltaTime + transform.right * h * speed * Time.deltaTime);

        // 移動動畫
        ani.SetBool("runSwitch", (v != 0 || h != 0));

        // 取左右的值
        float x = Input.GetAxis("Mouse X");
        transform.Rotate(0, x * turnSpeed * Time.deltaTime, 0);
        
    }

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        // 取 特定球體的碰撞結果(只對Layer8偵測)
        Collider[] hits = Physics.OverlapSphere(transform.position + floorOffset, floorRadius, 1<<8);

        if (hits.Length > 0 && hits[0] && Input.GetKeyDown(KeyCode.Space)) {
            rig.AddForce(0, jumpForce, 0);
        }
    }

    /// <summary>
    /// 開槍
    /// </summary>
    private void Fire()
    {
        if ( Input.GetKeyDown(KeyCode.Mouse0) )
        {
            // 生成子彈
            GameObject tempBullet = Instantiate<GameObject>(bullet, bulletSpawn.position, bulletSpawn.rotation);

            // 子彈飛出
            tempBullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.up * bulletSpeed);
        }
    }


    private void Update()
    {
        Move();
        Jump();
        Fire();
    }
}
