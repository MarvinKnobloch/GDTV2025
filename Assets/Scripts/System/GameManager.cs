using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MenuController menuController;
    public PlayerUI playerUI;
    public VictoryManager victoryManager;
    [NonSerialized] public CinemachineCamera cinemachineCamera;
    [NonSerialized] public CinemachineFollow cinemachineFollow;
    [NonSerialized] public Vector3 baseDamping;

    [NonSerialized] public int playerCurrency;

    [SerializeField] private DifficultySettings[] difficultySettings;
    [SerializeField] private TextMeshProUGUI[] difficultyTexts;

    public enum SaveFilePlayerPrefs
    {
        BossDefeated,
        ArenaEntranceDialog,
        IntroBoss1,
        IntroBoss2,
        IntroBoss3,
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    private void Start()
    {
        if (Player.Instance == null) return;

        if (SceneManager.GetActiveScene().buildIndex == (int)GameScenes.TitelScreen)
        {
            AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.TitleScreen, false, 0.1f, 1);
        }
        else if (SceneManager.GetActiveScene().buildIndex == (int)GameScenes.AreaHub)
        {
            //AudioManager.Instance.SetSong((int)AudioManager.MusicSongs.Tutorial);
            AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.ArenaHub, false, 0.1f, 1);
        }
        else
        {
            AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Boss, false, 0.1f, 5);
        }

        StartCoroutine(SetDifficultyButtons());
    }
    IEnumerator SetDifficultyButtons()
    {
        yield return null;

        if (PlayerPrefs.GetFloat("BossHealthMultiplier") == 0) PlayerPrefs.SetFloat("BossHealthMultiplier", 100);

        for (int i = 0; i < difficultyTexts.Length; i++)
        {
            if (i == 0) difficultyTexts[i].text = "<b><u>Explorer</u></b>";
            else if (i == 1) difficultyTexts[i].text = "<b><u>Average</u></b>";
            else difficultyTexts[i].text = "<b><u>Gamer</u></b>";

            int health = difficultySettings[i].playerBonusHealth + Player.Instance.health.MaxValue;
            difficultyTexts[i].text += "\n\n(<color=green>" + health + "</color> Player health)\n";
            difficultyTexts[i].text += "(<color=green>" + difficultySettings[i].bossHealthMultiplier + "% </color> Boss health)\n";
        }
    }

	public void PlayBossMusic()
	{
		AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Boss, false, 0.1f, 5);
	}

    public void ShowVictoryScreen()
    {
        victoryManager.gameObject.SetActive(true);
        playerUI.ToggleBossHealth(false);
        PoolingSystem.PurgePool();
    }

    public void ActivateCursor()
    {
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }
    public void DeactivateCursor()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    //public bool LoadProgress(GameManager.OverworldSaveNames saveName)
    //{
    //    if (PlayerPrefs.GetInt(saveName.ToString()) == 1)
    //    {
    //        return true;
    //    }
    //    else return false;    
    //}
    //public void SaveProgress(GameManager.OverworldSaveNames saveName)
    //{
    //    if (saveName != GameManager.OverworldSaveNames.Empty)
    //    {
    //        PlayerPrefs.SetInt(saveName.ToString(), 1);
    //    }
    //}
    public void ChangeCamera(Transform newTarget)
    {
        cinemachineFollow.TrackerSettings.PositionDamping = new Vector3(1, 1, 1);
        cinemachineCamera.Target.TrackingTarget = newTarget;
        StartCoroutine(ResetCameraDamping());
    }
    IEnumerator ResetCameraDamping()
    {
        yield return new WaitForSeconds(2);
        cinemachineFollow.TrackerSettings.PositionDamping = baseDamping;
    }
    public void SetDifficultyValues(int setting)
    {
        PlayerPrefs.SetInt("PlayerDifficultyHealth", difficultySettings[setting].playerBonusHealth);
        PlayerPrefs.SetFloat("BossHealthMultiplier", difficultySettings[setting].bossHealthMultiplier);

        PlayerPrefs.SetInt("CurrentDifficulty", setting);
        AudioManager.Instance.PlayUtilityOneshot((int)AudioManager.UtilitySounds.MenuSelect);
    }
    [Serializable]
    public class DifficultySettings
    {
        public int playerBonusHealth;
        public float bossHealthMultiplier;
    }
}
