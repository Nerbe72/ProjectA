using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] protected Image ShownFrame;
    [SerializeField] protected TMP_Text ShownText;
    [SerializeField] protected string ShownString;

    private KeyManager keyManager;

    private TMP_Text indicateText;

    private void Start()
    {
        keyManager = KeyManager.Instance;
        ShownText.text = ShownString;
    }

    private void OnTriggerEnter(Collider other)
    {
        //�ε������� ǥ��
        if (other == null) return;
        if (other.tag != "Player") return;

        ShownFrame.gameObject.SetActive(true);
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

        if (other == null) return;
        if (other.tag != "Player") return;

        ShownFrame.gameObject.SetActive(false);
    }

    public InteractType GetInteractType()
    {
        return type;
    }

    protected virtual void DoAction()
    {

    }
}
