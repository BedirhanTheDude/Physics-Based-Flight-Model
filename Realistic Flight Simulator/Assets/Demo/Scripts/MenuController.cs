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
    [SerializeField] private AircraftController controller;

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
            menuCamTransform.position = Maths.DampV(menuCamTransform.position, mainCamTransform.position,
                                                        3f, Time.deltaTime);

            menuCamTransform.rotation = Maths.DampQ(menuCamTransform.rotation, mainCamTransform.rotation,
                                                        3f, Time.deltaTime);

            if (menuCamTransform.rotation == mainCamTransform.rotation && menuCamTransform.position == menuCamTransform.position)
            {
                animationRun = false;
                menuCamTransform.gameObject.SetActive(false);
                mainCamTransform.gameObject.SetActive(true);
                controller.enabled = true;
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
