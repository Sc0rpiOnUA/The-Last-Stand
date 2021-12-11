using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int speed;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletDirection, bulletCollector, enemyCollector;
    [SerializeField]
    private TMP_Text scoreText, healthText;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private Camera mainCamera;
    private GUIStyle guiStyle = new GUIStyle();
    private Plane infinitePlane = new Plane(Vector3.up, 0);
    private int health = 100;

    private bool canShoot = true;

    public AudioSource source;
    public AudioClip clip;
    public EnemySpawner enemySpawner;
    public float shotDelay = .5f;
    public int score = 0;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 skewedMovement;
    bool isMovementPressed;

    private void Awake()
    {
        playerInput = new PlayerInput();
        try
        {
            characterController = GetComponent<CharacterController>();
        }
        catch (MissingReferenceException ex)
        {
            Debug.Log(ex);
            return;
        }

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
    }

    void Start()
    {
        mainCamera = Camera.main;
        guiStyle.fontSize = 20;

        playerInput.CharacterControls.Exit.performed += _ => handleDeath();
        playerInput.CharacterControls.Shoot.performed += _ => PlayerShoot();
    }

    // Update is called once per frame
    void Update()
    {
        handleGravity();
        handleRotation();
        handleHealth();
        TextUpdate(); 

        characterController.Move(skewedMovement * speed * Time.deltaTime);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyBullet")) health -= 5;
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    //private void OnGUI()
    //{
    //    Event currentEvent = Event.current;
    //    Vector2 mousePos = new Vector2();

    //    // Get the mouse position from Event.
    //    // Note that the y position from Event is inverted.
    //    mousePos.x = currentEvent.mousePosition.x;
    //    mousePos.y = mainCamera.pixelHeight - currentEvent.mousePosition.y;
        
    //    GUILayout.BeginArea(new Rect(20, 20, 300, 150));
    //    GUILayout.Label("Screen pixels: " + mainCamera.pixelWidth + ":" + mainCamera.pixelHeight, guiStyle);
    //    GUILayout.Label("Mouse position: " + mousePos, guiStyle);
    //    GUILayout.Label("Health: " + health, guiStyle);
    //    GUILayout.Label("Score: " + score, guiStyle);
    //    GUILayout.EndArea();
    //}

    void onMovementInput(InputAction.CallbackContext context)
    {
        float cameraOffsetY = mainCamera.transform.rotation.eulerAngles.y;
        Matrix4x4 matrix = Matrix4x4.Rotate(Quaternion.Euler(0, cameraOffsetY, 0));
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        skewedMovement = matrix.MultiplyPoint3x4(currentMovement);

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleRotation()
    {     
        float distance;
        Vector2 mouseScreenPositon = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = new Vector3();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPositon);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000))
        {
            mouseWorldPosition = hitData.point;
        }
        else
        {
            infinitePlane.Raycast(ray, out distance);
            mouseWorldPosition = ray.GetPoint(distance);
        }

        Vector3 targetDirection = mouseWorldPosition - transform.position;
        float angle = Mathf.Atan2(-targetDirection.z, targetDirection.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

        //transform.LookAt(new Vector3(mouseWorldPosition.x, transform.position.y, mouseWorldPosition.z));
    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            //currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            currentMovement.y = gravity;
            //currentRunMovement.y = gravity;
        }
    }

    private void handleHealth() 
    {
        if (health <= 0)
        {            
            handleDeath();
        }
    }

    private void handleDeath()
    {
        Debug.Log("Player died");

        if (!PlayerPrefs.HasKey("Score") || (PlayerPrefs.HasKey("Score") && score > PlayerPrefs.GetInt("Score")))
        {
            Debug.Log("Saving score: " + score);
            PlayerPrefs.SetInt("Score", score);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void AddScore()
    {
        score += 1;
    }

    private void PlayerShoot()
    {
        if (!canShoot) return;

        source.PlayOneShot(clip);
        Vector2 mouseScreenPositon = playerInput.CharacterControls.MousePosition.ReadValue<Vector2>();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPositon.x, mouseScreenPositon.y, 10f));
        GameObject g = Instantiate(bullet, bulletDirection.position, bulletDirection.rotation, bulletCollector);
        g.SetActive(true);

        enemySpawner.SpawnEnemy(1);
        StartCoroutine(CanShoot());
    }

    private void TextUpdate()
    {
        scoreText.text = score.ToString();
        healthText.text = health.ToString();
    }

    IEnumerator CanShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(shotDelay);
        canShoot = true;
    }        
}
