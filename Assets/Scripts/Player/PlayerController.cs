using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //Movement speed
    [Header("Movement")]
    public float playerSpeed = 2;
    public float playerAccel = 20;
    public float playerJump = 10;

    //Ground detection
    public float groundDetectDistance = 0.1f;
    public bool onGround;
    int groundDetectMask;

    [Header("Camera")]
    //Aditional movement parts
    public Transform camTransform;
    public Camera viewCam;
    Rigidbody playerRigid;

    [Header("Steam tank")]
    //Steam amount
    public float steamRechargeRate;
    public float steamCapacity;
    float steamReserve;

    [Header("Machinegun")]
    //Machinegun
    //public int clipSize;
    public float fireRateGun;
    float fireTimerGun;
    public float steamCostGun;
    public float spread = 0.1f;
    public float machinegunDamage = 1;
    Ray machineGunRay;
    RaycastHit machinegunHit;

    [Header("Cannon")]
    //Cannon
    public float fireRateCannon;
    float fireTimerCannon;
    public float steamCostCannon;
    public GameObject cannonBall;
    public float cannonForce;
    public float yCorrection;

    [Header("Hud")]
    //Visualisation
    public Slider cannonReload;
    public Slider steamAmmount;

    //Aiming
    Vector3 aimPoint;
    Ray aimRay;
    RaycastHit aimHit;

    // Use this for initialization
    void Start () {
        playerRigid = gameObject.GetComponent<Rigidbody>();
        groundDetectMask = ~LayerMask.GetMask("Player");
	}

    // Update is called once per frame
    void FixedUpdate() {
        //Detect ground
        Collider[] hits = Physics.OverlapSphere(transform.position + (Vector3.down * groundDetectDistance), 0.49f, groundDetectMask);
        onGround = hits.Length > 0;

        //Get input
        Vector3 inputVelocity = Vector3.zero;
        inputVelocity.x = Input.GetAxisRaw("Horizontal") * playerAccel;
        inputVelocity.z = Input.GetAxisRaw("Vertical") * playerAccel;

        //Rotate to follow camera
        Quaternion inputRotator = Quaternion.EulerAngles(0, camTransform.rotation.ToEulerAngles().y, 0);
        inputVelocity = inputRotator * inputVelocity;

        //Apply movement
        playerRigid.AddForce(inputVelocity);

        //Clamp velocity in the z and x axis
        inputVelocity = playerRigid.velocity;
        inputVelocity.y = 0;
        if(inputVelocity.magnitude > playerSpeed) {
            inputVelocity = inputVelocity.normalized * playerSpeed;
            inputVelocity.y = playerRigid.velocity.y;
            playerRigid.velocity = inputVelocity;
        }


        //Jump
        if (Input.GetButton("Jump") && onGround) {
            playerRigid.AddForce(0, playerJump, 0, ForceMode.Impulse);
        }

    }

    void Update() {
        //Aiming
        aimRay = viewCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(aimRay, out aimHit, 100, groundDetectMask)) {
            aimPoint = aimHit.point;
        }

        //Steam recharge
        steamReserve += steamRechargeRate * Time.deltaTime;
        if (steamReserve > steamCapacity) {
            steamReserve = steamCapacity;
        }

        //weapons
        fireGun();
        fireCannon();

        //Visualization
        steamAmmount.value = steamReserve / steamCapacity;
        cannonReload.value = fireTimerCannon / fireRateCannon;
        
    }

    void fireGun() {
        if (Input.GetButton("Fire1")) {
            fireTimerGun += Time.deltaTime;
            machineGunRay.origin = transform.position;
            while(fireTimerGun >= fireRateGun && steamReserve >= steamCostGun) {
                fireTimerGun -= fireRateGun;
                steamReserve -= steamCostGun;
                //Aim and shoot
                machineGunRay.direction = (aimPoint - transform.position).normalized;
                float spreadRotation = Random.Range(0, 360);
                float randomSpread = Random.Range(0, spread);
                Vector3 randomOffset;
                randomOffset.z = 0;
                randomOffset.x = Mathf.Sin(spreadRotation) * randomSpread;
                randomOffset.y = Mathf.Cos(spreadRotation) * randomSpread;
                Quaternion offsetRotator = Quaternion.LookRotation(machineGunRay.direction);
                machineGunRay.direction = ((machineGunRay.direction * 10) + (offsetRotator * randomOffset)).normalized;

                if(Physics.Raycast(machineGunRay, out machinegunHit, 100, groundDetectMask)) {
                    EnemyHealth enemy = machinegunHit.collider.gameObject.GetComponent<EnemyHealth>();
                    if (enemy) {
                        enemy.damage(1);
                    }
                }

                Debug.DrawRay(machineGunRay.origin, machineGunRay.direction * 100, Color.black);

                //Debug.Log("TA!");
            }
        } else {
            fireTimerGun = 0;
        }
    }

    void fireCannon() {
        if (fireTimerCannon >= fireRateCannon) {
            if (Input.GetButtonDown("Fire2") && steamReserve >= steamCostCannon) {
                fireTimerCannon = 0;
                steamReserve -= steamCostCannon;
                //Aim and shoot
                Vector3 aimDirection = (aimPoint - transform.position).normalized;
                aimDirection.y += yCorrection;
                aimDirection.Normalize();
                GameObject newBall = Instantiate(cannonBall, transform.position, transform.rotation);
                Rigidbody ballRigid = newBall.GetComponent<Rigidbody>();

                ballRigid.AddForce(aimDirection * cannonForce, ForceMode.Impulse);

                Debug.Log("BOOOOOM!");
            }
        } else {
            fireTimerCannon += Time.deltaTime;
        }
    }
}
