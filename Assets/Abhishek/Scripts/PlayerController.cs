using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public Transform viewPort;
    public float mouseSensitivity = 1f;
    private float verticalRotation;
    private Vector2 mouseInput;

    public float walkSpeed = 5f , runSpeed = 9f;
    private float activeSpeed;
    private Vector3 moveDir, movement;
    public CharacterController characterController;
    private Camera cam;
    public float jumpForce = 12f, gravityMod = 2.5f;
    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
   // public float timeBetweenShots = .1f;
    private float shotcounter;
    //public int maxAmmo = 25;
    //private int currAmmo;
    //private float reloadTime = 2f;
    private bool isReloading = false;
   /// <summary>
   public TMP_Text ammoText;
   /// </summary>

    public Gun[] allGuns;
    private int selectedGun;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam= Camera.main;
        //allGuns[selectedGun].currAmmo = allGuns[selectedGun].maxAmmoPerGun;
        SwitchGuns();
    }

    void Update()
    {   
        //Mouse
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        verticalRotation += mouseInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);
        viewPort.rotation = Quaternion.Euler(-verticalRotation, viewPort.rotation.eulerAngles.y, viewPort.rotation.eulerAngles.z);
        
        //Movement
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if(Input.GetKey(KeyCode.LeftShift)) {
            activeSpeed = runSpeed; 
        } else {
            activeSpeed = walkSpeed;
        }
        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeSpeed;
        movement.y = yVel;
        if(characterController.isGrounded) {
            movement.y = 0f;
        }
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
        if(Input.GetButtonDown("Jump") && isGrounded) {
            movement.y = jumpForce;
        }
        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
        characterController.Move(movement * Time.deltaTime);

        //Shoot
        ammoText.text = allGuns[selectedGun].currAmmo.ToString();
        if (isReloading)
            return;

        if(allGuns[selectedGun].currAmmo <=0 || Input.GetKey(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

       if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
        {
            shotcounter -= Time.deltaTime;
            if(shotcounter <= 0)
            {
                Shoot();
            }
        }

        //Switch Guns
        if (Input.GetAxisRaw("Mouse ScrollWheel")> 0f)
        {
            selectedGun++;
            if(selectedGun>= allGuns.Length) 
            {
                selectedGun = 0;
            }
            SwitchGuns();
        }else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun < 0)
            {
                selectedGun = allGuns.Length - 1;
            }
            SwitchGuns();
        }

    }

    IEnumerator Reload() 
    {   isReloading= true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(allGuns[selectedGun].reloadTime);
        allGuns[selectedGun].currAmmo = allGuns[selectedGun].maxAmmoPerGun;
        isReloading= false;
    }
    private void Shoot()
    {
        allGuns[selectedGun].currAmmo--;
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin= cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject BulletImpactObj = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(BulletImpactObj, 3f);
        }
        shotcounter = allGuns[selectedGun].timeBetweenShots;
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPort.position;
        cam.transform.rotation = viewPort.rotation;
    }

    void SwitchGuns()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);
    }
}
