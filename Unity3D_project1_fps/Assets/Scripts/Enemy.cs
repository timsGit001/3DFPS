using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// 玩家
    /// </summary>
    private Transform player;

    /// <summary>
    /// 代理
    /// </summary>
    private NavMeshAgent nav;

    private Animator ani;

    [Header("移動速度"), Range(0f, 30f)]
    public float speed = 2.5f;
    [Header("攻擊範圍"), Range(2f, 100f)]
    public float atkRange = 10f;
    [Header("子彈")]
    public GameObject bullet;
    [Header("子彈生成點")]
    public Transform bulletSpawn;
    [Header("子彈速度"), Range(0f, 5000f)]
    public float bulletSpeed;
    [Header("開槍間隔"), Range(0f, 10f)]
    public float cdTime;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Soldier").transform;

        nav = GetComponent<NavMeshAgent>();
        nav.speed = speed;
        nav.stoppingDistance = atkRange;

        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Tracking();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, atkRange);
    }

    void Tracking()
    {
        nav.SetDestination(player.position);

        bool preparingAtk = nav.remainingDistance <= atkRange;
        // 移動動畫
        ani.SetBool("runSwitch", !preparingAtk);
        Fire(preparingAtk);
    }

    void Fire(bool preparingAtk)
    {
        if (preparingAtk)
        {
            // 開槍動畫
            ani.SetTrigger("doShoot");
            // 生成子彈
            GameObject tempBullet = Instantiate<GameObject>(bullet, bulletSpawn.position, bulletSpawn.rotation);

            // 子彈飛出
            tempBullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.up * bulletSpeed);

            // 開槍聲響
            // aud.PlayOneShot(fireFsx, Random.Range(0.8f, 1.1f));
        }
    }
}
