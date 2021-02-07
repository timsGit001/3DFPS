using UnityEngine;

public class FPSContraler : MonoBehaviour
{
    [Header("移動速度"), Range(0, 2000)]
    public float speed;
    [Header("轉動速度"), Range(0, 2000)]
    public float turnSpeed;

    private Animator ani;
    private Rigidbody rig;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

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
    }

    private void Update()
    {
        Move();
    }
}
