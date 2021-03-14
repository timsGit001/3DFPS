using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSContraler : MonoBehaviour
{
    #region 基本欄位
    [Header("血量"), Range(0, 100)]
    public float hp = 100f;
    private float hpMax;
    [Header("移動速度"), Range(0, 20000)]
    public float speed;
    [Header("轉動速度"), Range(0, 1000)]
    public float turnSpeed;
    [Header("跳躍力道"), Range(0, 10000)]
    public float jumpForce;
    [Header("地板偵測")]
    public Vector3 floorOffset;
    [Header("地板偵測半徑"), Range(0, 10)]
    public float floorRadius = 1;
    [Header("UI圖示: Hp")]
    public Image hpImg;
    [Header("UI文字: Hp數值")]
    public Text hpText;

    private Animator ani;
    private Rigidbody rig;
    private AudioSource aud;
    private Transform camMain;
    private Transform camSelf;
    private Transform crosshair;
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
    [Header("彈夾子彈數")]
    public int bulletClip = 30;
    [Header("UI文字: 子彈目前數量")]
    public Text textBulletCurrent;
    [Header("UI文字: 子彈總數")]
    public Text textBulletTotal;
    [Header("子彈攻擊力"), Range(0, 100)]
    public float pwr = 30;
    [Header("換彈夾花費時間(秒)")]
    public float reloadTime = 1.0f;
    [Header("開槍間隔時間(秒)")]
    public float fireIntervalTime = 0.1f;
    [Header("準心上下移動的速度"), Range(0, 10)]
    public float crosshairSpeed = 0.2f;
    [Header("準心上下移動的範圍")]
    public Vector2 crosshairMoveLimit = new Vector2(2f, 3.3f);
    [Header("音效: 開槍")]
    public AudioClip fireFsx;
    [Header("音效: 換彈夾")]
    public AudioClip reloadFsx;

    private float timer = 0.5f;
    private bool isReloading = false;
    #endregion

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();

        // 隱藏鼠標
        Cursor.visible = false;

        textBulletCurrent.text = bulletCurrent.ToString();
        textBulletTotal.text = bulletTotal.ToString();

        hpText.text = hp.ToString();
        hpMax = hp;

        camMain = transform.Find("Cameras").Find("Main Camera");
        camSelf = transform.Find("Cameras").Find("Self Camera");
        crosshair = transform.Find("Crosshair");
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

        // 取得 上下值
        float y = Input.GetAxis("Mouse Y");
        Vector3 crossPos = crosshair.localPosition;
        crossPos.y += y * Time.deltaTime * crosshairSpeed;
        crossPos.y = Mathf.Clamp(crossPos.y, crosshairMoveLimit.x, crosshairMoveLimit.y);
        crosshair.localPosition = crossPos;

    }

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {
        // 取 特定球體的碰撞結果(只對Layer8偵測)
        Collider[] hits = Physics.OverlapSphere(transform.position + floorOffset, floorRadius, 1 << 8);

        if (hits.Length > 0 && hits[0] && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(0, jumpForce, 0);
        }
    }

    /// <summary>
    /// 開槍
    /// </summary>
    private void Fire()
    {
        if (timer >= fireIntervalTime && !isReloading && Input.GetKey(KeyCode.Mouse0) && bulletCurrent > 0)
        {
            timer = 0;

            // 開槍動畫
            ani.SetTrigger("doShoot");

            // 消耗子彈
            bulletCurrent--;
            textBulletCurrent.text = bulletCurrent.ToString();

            // 生成子彈
            GameObject tempBullet = Instantiate<GameObject>(bullet, bulletSpawn.position, bulletSpawn.rotation);
            tempBullet.GetComponent<Bullets>().pwr = pwr;

            // 子彈飛出
            tempBullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.up * bulletSpeed);

            // 開槍聲響
            aud.PlayOneShot(fireFsx, Random.Range(0.8f, 1.1f));
        }
        else
        {
            timer += 1 * Time.deltaTime;
        }
    }

    /// <summary>
    /// 補充子彈
    /// </summary>
    private void AddBullet()
    {
        // 按下 "R" 且 非換彈夾中 且 子彈總數大於0 且 目前子彈數目 小於 一個彈夾
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletTotal > 0 && bulletCurrent < bulletClip)
        {
            StartCoroutine(AddBulletDelay());
        }
    }

    private IEnumerator AddBulletDelay()
    {
        isReloading = true;


        // 換彈夾動畫
        ani.SetTrigger("doReload");

        // 換彈夾聲響
        aud.PlayOneShot(reloadFsx, Random.Range(0.8f, 1.0f));

        yield return new WaitForSeconds(reloadTime);

        int difBullet = bulletClip - bulletCurrent;

        bulletTotal -= difBullet;
        if (bulletTotal < 0)
        {
            // 子彈不夠 => 不能扣超過
            difBullet += bulletTotal;
            bulletTotal = 0;
        }

        bulletCurrent += difBullet;

        // 顯示在 UI上
        textBulletCurrent.text = bulletCurrent.ToString();
        textBulletTotal.text = bulletTotal.ToString();

        isReloading = false;
    }


    private void Damage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            Death();
        }

        // 設定UI
        hpText.text = hp.ToString();
        float amount = hp / hpMax;
        hpImg.fillAmount = amount;
        if (amount < 0.5f)
        {
            hpText.color = Color.red;
            hpImg.color = Color.red;
        }
    }

    private void Death()
    {
        ani.SetBool("dieSwitch", true);
        rig.Sleep();
        rig.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<CapsuleCollider>().enabled = false;
        enabled = false;

        StartCoroutine(MoveCamFinal());
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Bullets")
        {
            Damage(obj.GetComponent<Bullets>().pwr);
        }
    }


    private IEnumerator MoveCamFinal()
    {
        // 攝影機 往玩家看
        camMain.LookAt(transform);
        camSelf.LookAt(transform);

        // 取得攝影機座標
        Vector3 camPos = camMain.position;
        // 取得攝影機的y
        float camPosY = camPos.y;
        float camNewPosY = camPos.y + 2;
        Vector3 finPos = new Vector3(camPos.x, camPos.y + 2, camPos.z);
        Vector3 newPos = Vector3.Lerp(camPos, finPos, 0.001f);

        print((finPos - newPos).magnitude);
        while ((finPos - newPos).magnitude > 0.05)
        {
            camMain.position = newPos;
            camSelf.position = newPos;

            yield return new WaitForSeconds(0.05f);
            newPos = Vector3.Lerp(camMain.position, finPos, 0.3f);
            print((finPos - newPos).magnitude);
        }

        camMain.position = finPos;
        camSelf.position = finPos;
    }

    private void Update()
    {
        Move();
        Jump();
        Fire();
        AddBullet();
    }
}
