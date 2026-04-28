using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Shows game-over feedback with a black fade, then enables restart to reload the active scene.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text endText;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }
    public void Show(bool isGameOver)
    {
        StartCoroutine(GameOver(isGameOver));
    }

    IEnumerator GameOver(bool isGameOver)
    {
        endText.text = isGameOver ? "Game Over" : "Game Clear";
        fadeImage.gameObject.SetActive(true);
        var timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            var alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        gameOverText.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
