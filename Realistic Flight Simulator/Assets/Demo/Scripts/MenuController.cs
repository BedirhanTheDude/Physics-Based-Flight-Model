using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [Header("Canvases")]

    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject pauseMenu;

    [Header("Camera Animation")]

    [SerializeField] private Transform menuCamTransform;
    [SerializeField] private Transform mainCamTransform;

    [SerializeField] private float animationSpeed;
    [SerializeField] private float animationRotationSpeed;

    [Header("Plane")]

    [SerializeField] private Rigidbody rb;

    private bool animationRun = false;

    public void StartButton()
    {
        startMenu.SetActive(false);
        HUD.SetActive(true);
        animationRun = true;
    }

    public void ResumeButton()
    {
        Time.timeScale = 1f;
        HUD.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void FixedUpdate()
    {
        if (animationRun)
        {
            menuCamTransform.position = Vector3.MoveTowards(menuCamTransform.position,
                                       mainCamTransform.position, animationSpeed * Time.fixedDeltaTime);
            Vector3 menuCamRotationVector = menuCamTransform.rotation.eulerAngles;
            Vector3 mainCamRotationVector = mainCamTransform.rotation.eulerAngles;

            menuCamRotationVector = Vector3.MoveTowards(menuCamRotationVector, mainCamRotationVector, 
                                                        animationRotationSpeed * Time.fixedDeltaTime);
            menuCamTransform.rotation = Quaternion.Euler(menuCamRotationVector);

            if (menuCamTransform.rotation == mainCamTransform.rotation && menuCamTransform.position == menuCamTransform.position)
            {
                animationRun = false;
                menuCamTransform.gameObject.SetActive(false);
                mainCamTransform.gameObject.SetActive(true);
                rb.useGravity = true;
            }
        }        
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            if (startMenu.activeInHierarchy) return;
            Time.timeScale = 0f;
            HUD.SetActive(false);
            pauseMenu.SetActive(true);
        }
    }
}
