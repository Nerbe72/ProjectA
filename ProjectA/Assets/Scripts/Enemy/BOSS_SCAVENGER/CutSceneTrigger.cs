using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] private Enemy owner;
    [SerializeField] private PlayableDirector cutScene;

    PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.tag != "Player") return;

        //ÄÆ¾À Àç»ý
        //other.GetComponent<PlayerController>().enabled = false;
        playerController = other.GetComponent<PlayerController>();
        playerController.SetRefuseInput();
        CameraManager.Instance.OffAllCam();
        owner.SetCutscene();
        owner.SetFaced();
        cutScene.Play();
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.UnSetRefuseInput();
        else
            PlayerController.Instance.UnSetRefuseInput();
        CameraManager.Instance.ResetCam();
        owner.ResetCutscene();
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        if (other.tag != "Player") return;

        //ÄÆ¾À Àç»ý
        //other.GetComponent<PlayerController>().enabled = true;
        other.GetComponent<PlayerController>().UnSetRefuseInput();
    }
}
