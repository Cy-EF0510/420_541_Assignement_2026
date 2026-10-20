using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapDamage : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ScoreManager.Instance == null)
            {
                Debug.LogError("ScoreManager.Instance is null!");
                return;
            }

            ScoreManager.Instance.ReduceHealth();
            if (ScoreManager.Instance.Health <= 0)
            {
                ScoreManager.Instance.SaveTotalHealth();
                SceneManager.LoadScene("EndScreen");
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}