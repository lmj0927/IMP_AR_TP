using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }
    public void Show()
    {
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
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
