using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorial1;
    [SerializeField] private GameObject tutorial2;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject tutorialPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void StartNatureWalk()
    {
        SceneManager.LoadScene("NatureScene");
    }

    public void StartDogSim()
    {
        SceneManager.LoadScene("HouseScene");
    }

    public void StartMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void SwapTutorial()
    {
        if (tutorial1.activeSelf)
        {
            tutorial1.SetActive(false);
            tutorial2.SetActive(true);
        } else if (tutorial2.activeSelf)
        {
            tutorialPanel.SetActive(false);
        }
    }
}
