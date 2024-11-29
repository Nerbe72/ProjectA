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
    //��ȣ�ۿ� Ÿ�Կ� ���� ����
    //Ÿ���� �� 2������ ��ں�(�޽�), �Ȱ���(����)
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
        //��ȣ�ۿ� �ε������� ǥ��
    }

    private void OnTriggerStay(Collider other)
    {
        //��ȣ�ۿ� ����
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
        //��ȣ�ۿ� �ε������� ����
    }

    public InteractType GetInteractType()
    {
        return type;
    }

    protected virtual void DoAction()
    {

    }
}
