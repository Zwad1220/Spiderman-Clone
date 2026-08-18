using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject tutorial;

    PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Controls.performed += ctx => ToggleObject();
    }

    private void Start()
    {
        tutorial.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void ToggleObject()
    {
        tutorial.SetActive(!tutorial.activeSelf);

        if (tutorial.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

}
