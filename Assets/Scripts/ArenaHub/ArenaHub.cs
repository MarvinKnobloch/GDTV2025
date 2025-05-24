using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ArenaHub : MonoBehaviour
{
    [Header("ArenaGate")]
    [SerializeField] private VoidEventChannel openArena;
    [SerializeField] private OpenGate arenaGate;

    [Header("ScreenShake")]
    [SerializeField] private float baseImpulseVelocityX;
    [SerializeField] private float baseImpulseVelocityY;
    [SerializeField] private float impulseBaseForce;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void OnEnable()
    {
        openArena.OnEventRaised += OpenArena;
    }
    private void OnDisable()
    {
        openArena.OnEventRaised -= OpenArena;
    }
    private void OpenArena()
    {
        if (PlayerPrefs.GetInt("ArenaOpen") == 1) return;

        PlayerPrefs.SetInt("ArenaOpen", 1);
        arenaGate.MovementStart();
        StartCoroutine(ScreenShake());
    }
    IEnumerator ScreenShake()
    {
        impulseSource.DefaultVelocity = new Vector3(baseImpulseVelocityX, baseImpulseVelocityY, 0);
        impulseSource.GenerateImpulseWithForce(impulseBaseForce);
        yield return new WaitForSeconds(1);
        impulseSource.DefaultVelocity = new Vector3(baseImpulseVelocityX, baseImpulseVelocityY, 0);
        impulseSource.GenerateImpulseWithForce(impulseBaseForce);
    }
}
