using UnityEditor.Experimental.GraphView;
using UnityEngine;  

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public int life;
    public int score;
    public float speed;
    public int boom;
    public int maxBoom;
    public int power;
    public int maxPower;
    public float curShotDelay;
    public float maxShotDelay;

    public GameObject bulletA;
    public GameObject bulletB;
    public GameObject boomEffect;
    public GameManager manager;
    public bool isHit;
    public bool isBoomTime;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            animator.SetInteger("Input", (int)h);
        }

    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                CreateBullet(bulletA, transform.position, transform.rotation);
                break;

            case 2:
                CreateBullet(bulletA, transform.position + Vector3.left * 0.1f, transform.rotation);
                CreateBullet(bulletA, transform.position + Vector3.right * 0.1f, transform.rotation);
                break;

            case 3:
                CreateBullet(bulletA, transform.position + Vector3.left * 0.35f, transform.rotation);
                CreateBullet(bulletB, transform.position, transform.rotation);
                CreateBullet(bulletA, transform.position + Vector3.right * 0.35f, transform.rotation);
                break;
        }

        curShotDelay = 0;
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))
            return;
        if (isBoomTime)
            return;
        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        manager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemyLogic = enemies[i].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
        }
    }

    void CreateBullet(GameObject bulletType, Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletType, position, rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit)
                return;

            isHit = true;
            life--;
            manager.UpdateLifeIcon(life);

            if (life == 0)
            {
                manager.GameOver();
            }
            else
            {
                manager.RespawnPlayer();
            }

            manager.RespawnPlayer();
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                        power++;
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        manager.UpdateBoomIcon(boom);
                    }

                    break;
            }
            Destroy(collision.gameObject);
        }
    }
    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
}



// Power에 따라 발사되는 총알이 변경되는 기존 코드 

/* switch (power)
{
    case 1:
        GameObject bullet = Instantiate(bulletA, transform.position, transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        break;

    case 2:
        GameObject bulletL = Instantiate(bulletA, transform.position 
            + Vector3.left * 0.1f, transform.rotation);

        GameObject bulletR = Instantiate(bulletA, transform.position 
            + Vector3.right * 0.1f, transform.rotation);

        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        break;

    case 3:
        GameObject bulletLL = Instantiate(bulletA, transform.position
            + Vector3.left * 0.35f, transform.rotation);

        GameObject bulletCC = Instantiate(bulletB, transform.position, transform.rotation);

        GameObject bulletRR = Instantiate(bulletA, transform.position
            + Vector3.right * 0.35f, transform.rotation);

        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
        rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        break;
}
*/
// 이것으로 코드가 더 간결해지고 유지보수가 더 쉬워짐
// 또한 총알 생성 로직을 변경해야 하는 경우에 각 case에 중복으로 작성하는 대신 CreateBullet 메서드에서만 변경하면 됨