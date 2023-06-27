using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup optionPanel;
    public CanvasGroup transitionPanel;
    [SerializeField] Animator transitionAnim;
    public static MainMenuController instance;

    public void PlayGame()
    {
        StartCoroutine(nextLevel());
    }

    public IEnumerator nextLevel()
    {
        transitionAnim.SetTrigger("End");
        transitionPanel.blocksRaycasts = false;
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //transitionAnim.SetTrigger("Start");
    }

    public void Option()
    {
        optionPanel.alpha = 1;
        optionPanel.blocksRaycasts = true;
    }

    public void Back()
    {
        optionPanel.alpha = 0;
        optionPanel.blocksRaycasts = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
