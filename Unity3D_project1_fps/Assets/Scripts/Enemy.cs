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
    private Rigidbody rig;

    [Header("移動速度"), Range(0f, 30f)]
    public float speed = 2.5f;
    [Header("攻擊範圍"), Range(2f, 100f)]
    public float atkRange = 10f;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Soldier").transform;

        nav = GetComponent<NavMeshAgent>();
        nav.speed = speed;
        nav.stoppingDistance = atkRange;

        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Tracking();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.3f);
        Gizmos.DrawSphere(transform.position, atkRange);
    }

    void Tracking()
    {
        nav.SetDestination(player.position);
        // 移動動畫
        ani.SetBool("runSwitch", rig.velocity.magnitude > 0);
    }
}
