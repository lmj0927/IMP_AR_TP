using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] FilterCamera filterCamera;
    [SerializeField] private int stageLevel = 0;
    [SerializeField] private GameOverUI gameOverUI;

    public void GameOver()
    {
        gameOverUI.Show();
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }

    public void IncreaseStageLevel()
    {
        stageLevel++;
    }
    
    public void IncreaseLeftFilterTime(float increaseAmount)
    {
        filterCamera.IncreaseLeftFilterTime(increaseAmount);
    }
}
