using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailed : MonoBehaviour
{
    //public string nextSceneName;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Failed!");
            GameManager.instance.SetState(GameManager.GameState.GameOver);
            GameManager.instance.GameOver();
        }
    }
}
