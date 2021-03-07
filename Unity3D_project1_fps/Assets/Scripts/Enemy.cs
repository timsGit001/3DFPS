using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    [Header("子彈目前數量")]
    public int bulletCurrent = 30;
    [Header("彈夾子彈數")]
    public int bulletClip = 30;
    [Header("子彈攻擊力"), Range(0, 100)]
    public float pwr = 30;

    [Header("轉身速度"), Range(0f, 50f)]
    public float turnSpeed;
    private float timer;
    private bool isReloading;
    private float hp = 100f;

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
        if (preparingAtk && !isReloading && timer > cdTime)
        {
            if (EmptyBullets())
            {
                // 換彈夾
            }
            else
            {
                timer = 0f;
                // 開槍動畫
                ani.SetTrigger("doShoot");
                // 生成子彈
                GameObject tempBullet = Instantiate<GameObject>(bullet, bulletSpawn.position, bulletSpawn.rotation);
                tempBullet.GetComponent<Bullets>().pwr = pwr;

                // 子彈飛出
                tempBullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.up * bulletSpeed);

                // 開槍聲響
                // aud.PlayOneShot(fireFsx, Random.Range(0.8f, 1.1f));
            }
        }
        else
        {
            timer += Time.deltaTime;
            FaceToTarget();
        }

    }

    void FaceToTarget()
    {
        Quaternion faceAngle = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, faceAngle, Time.deltaTime * turnSpeed);
        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);// 發現會晃 所以只好把x軸定住!
    }

    bool EmptyBullets()
    {
        if (bulletCurrent <= 0)
        {
            print("run out of bullets");
            StartCoroutine(AddBulletDelay());
            return true;
        }

        bulletCurrent--;
        return false;
    }


    private IEnumerator AddBulletDelay()
    {

        isReloading = true;
        // 換彈夾動畫
        ani.SetTrigger("doReload");

        yield return new WaitForSeconds(3.5f);

        bulletCurrent = bulletClip;

        isReloading = false;
    }

    private void Damage(float dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        ani.SetBool("dieSwitch", true);
        Rigidbody rig = GetComponent<Rigidbody>();
        rig.Sleep();
        rig.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Bullets")
        {
            Damage(obj.GetComponent<Bullets>().pwr);
        }
    }
}
