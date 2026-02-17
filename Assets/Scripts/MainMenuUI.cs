using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private AudioClip buttonPressSFX;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("FirstTime"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        CursorManager.EnableCursor(true);
    }

    public void StartGame()
    {
        ButtonPressSFX();
        CursorManager.EnableCursor(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSettings()
    {
        ButtonPressSFX();
    }

    public void QuitGame()
    {
        ButtonPressSFX();
        Application.Quit();
    }

    public void ButtonPressSFX()
    {
        SoundManager.instance.PlaySFX(ref buttonPressSFX);
    }
}
