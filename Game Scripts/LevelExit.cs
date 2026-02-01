using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public string nextLevelName;   // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.LogError("No next level assigned in LevelExit!");
            }
        }
    }
}
