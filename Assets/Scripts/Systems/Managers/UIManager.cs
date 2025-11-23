using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI trashCountText;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject bagFullIndicator;

    private void OnEnable()
    {
        GameEvents.OnTrashCountUpdated += UpdateTrashCount;
        GameEvents.OnGameOver += ShowGameOverScreen;
        GameEvents.OnBagFilled += ShowBagFull;
        GameEvents.OnBagDisposed += HideBagFull;
    }

    private void OnDisable()
    {
        GameEvents.OnTrashCountUpdated -= UpdateTrashCount;
        GameEvents.OnGameOver -= ShowGameOverScreen;
        GameEvents.OnBagFilled -= ShowBagFull;
        GameEvents.OnBagDisposed -= HideBagFull;
    }

    private void Update()
    {
        if (GameManager.Instance != null && timerText != null)
        {
            float time = GameManager.Instance.GetTimeRemaining();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    private void UpdateTrashCount(int current, int total)
    {
        if (trashCountText != null)
            trashCountText.text = $"Trash: {current} / {total}";
    }

    private void ShowGameOverScreen(bool isWin)
    {
        if (isWin && winScreen != null) winScreen.SetActive(true);
        else if (!isWin && loseScreen != null) loseScreen.SetActive(true);
    }

    private void ShowBagFull()
    {
        if (bagFullIndicator != null) bagFullIndicator.SetActive(true);
    }

    private void HideBagFull()
    {
        if (bagFullIndicator != null) bagFullIndicator.SetActive(false);
    }
}
