using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Map
{
    Stage1,
    Stage1_BOSS,
}

public class FogWall : Interact
{
    [Header("Portal")]
    [SerializeField] private MapConnection connection;
    [SerializeField] private Map nextScene;
    [SerializeField] private Transform destination;

    private void Awake()
    {
        if (connection != MapConnection.SelectedConnection) return;

        var player = PlayerController.Instance;
        var playerCC = player.GetComponent<CharacterController>();

        CameraManager.Instance.main.transform.position = destination.position + new Vector3(0, 2, 2);
        playerCC.enabled = false;
        player.transform.position = destination.position;
        PlayerController.Instance.ResetRotation(destination.rotation);
        Debug.Log($"player {player.transform.rotation} / dest {destination.rotation}");
        playerCC.enabled = true;

        playerCC.velocity.Set(0, 0, 0);
    }

    protected override void DoAction()
    {
        
        MapConnection.SelectedConnection = connection;
        SceneManager.LoadScene(nextScene.ToString());
    }
}
