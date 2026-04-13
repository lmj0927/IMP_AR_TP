using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] FilterCamera filterCamera;
    [SerializeField] private int stageLevel = 0;
    [SerializeField] private GameOverUI gameOverUI;
    protected override void Initialize()
    {
        base.Initialize();
    }

    public void GameOver()
    {
        //TODO : 게임오버 로직
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
