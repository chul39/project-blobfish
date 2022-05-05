using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour {

    [SerializeField] private GameObject playButton, pauseButton, restartButton, soundOnButton, soundOffButton, healthText, growthText, pauseText, gameOverText;

    public void PauseGame() {
        gameObject.GetComponent<SceneManager>().SetGameState(false);
        playButton.SetActive(true);
        pauseButton.SetActive(false);
        pauseText.SetActive(true);
    }

    public void UnpauseGame() {
        gameObject.GetComponent<SceneManager>().SetGameState(true);
        playButton.SetActive(false);
        pauseButton.SetActive(true);
        pauseText.SetActive(false);
    }

    public void RestartGame() {
        gameObject.GetComponent<SceneManager>().RestartGame();
        gameOverText.SetActive(false);
        restartButton.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void HandleGameOver() {
        gameOverText.SetActive(true);
        restartButton.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void TurnSoundOn() {
        gameObject.GetComponent<AudioSource>().mute = false;
        soundOnButton.SetActive(false);
        soundOffButton.SetActive(true);
    }

    public void TurnSoundOff() {
        gameObject.GetComponent<AudioSource>().mute = true;
        soundOnButton.SetActive(true);
        soundOffButton.SetActive(false);
    }

    public void UpdateHealthText(int currentHealth, int maxHealth) {
        int toBeDisplayNumber = currentHealth > 0 ? currentHealth : 0;
        string toBeDisplayText = "HP " + toBeDisplayNumber.ToString() + "/" + maxHealth.ToString();
        healthText.GetComponent<TMPro.TMP_Text>().text = toBeDisplayText;
    }

    public void UpdateGrowthText(int currentLevel, int currentGrowth, int toNextGrowth) {
        string toBeDisplayText = "Lv." + currentLevel + " ";
        toBeDisplayText += currentLevel == 3 ? "-/-": currentGrowth.ToString() + "/" + toNextGrowth.ToString();
        growthText.GetComponent<TMPro.TMP_Text>().text = toBeDisplayText;
    }
    
}
