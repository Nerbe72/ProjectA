using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum InteractType
{
    REST,
    WALL,
}

public class Interact : MonoBehaviour
{
    //상호작용 타입에 따라 구분
    //타입은 총 2가지로 모닥불(휴식), 안개벽(워프)
    [SerializeField] private InteractType type;
    [SerializeField] protected string ShownString;

    private KeyManager keyManager;

    private TMP_Text indicateText;

    private void Start()
    {
        keyManager = KeyManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        //상호작용 인디케이터 표시
    }

    private void OnTriggerStay(Collider other)
    {
        //상호작용 가능
        if (other == null) return;
        if (other.tag != "Player") return;

        //indicateText.enabled = true;
        if (keyManager.KeyDown(Keys.Interact))
        {
            DoAction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //indicateText.enabled = false;
        //상호작용 인디케이터 제거
    }

    public InteractType GetInteractType()
    {
        return type;
    }

    protected virtual void DoAction()
    {

    }
}
