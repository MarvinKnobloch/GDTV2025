using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MenuController menuController;
    public PlayerUI playerUI;
    [NonSerialized] public CinemachineCamera cinemachineCamera;
    [NonSerialized] public CinemachineFollow cinemachineFollow;
    [NonSerialized] public Vector3 baseDamping;

    [NonSerialized] public int playerCurrency;

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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Menu, true, 0.1f, 1);
        }
        if (Player.Instance == null) return;

        //playerUI.PlayerCurrencyUpdate(PlayerPrefs.GetInt("PlayerCurrency"));

        //PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);

        //if (SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    //AudioManager.Instance.SetSong((int)AudioManager.MusicSongs.Tutorial);
        //    AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.Tutorial, true, 0.1f, 1);
        //}
        //else if (SceneManager.GetActiveScene().buildIndex == 2)
        //{
        //    AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.FireArea, true, 0.1f, 1);
        //}
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
}
