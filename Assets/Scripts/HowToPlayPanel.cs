using UnityEngine;

public class HowToPlayPanel : MonoBehaviour
{
    [SerializeField] GameObject howToPlayPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        howToPlayPanel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }
}
