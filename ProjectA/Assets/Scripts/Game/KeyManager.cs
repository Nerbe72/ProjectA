using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum Keys
{
    Interact,
    Run,
    Dodge,
    Count,
}

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;

    [HideInInspector] public KeyCode[] KeySetting;
    [SerializeField][Tooltip("키 갯수와 순서에 주의")] private List<KeyCode> defaultKeys;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        KeySetting = new KeyCode[(int)Keys.Count];

        for (int i = 0; i < (int)Keys.Count; i++)
        {
            KeySetting[i] = (KeyCode)PlayerPrefs.GetInt(KeyString(i));
            if (KeySetting[i] == KeyCode.None)
            {
                PlayerPrefs.SetInt(KeyString(i), (int)defaultKeys[i]);
            }
        }
    }

    private string KeyString(int _n)
    {
        return ((Keys)_n).ToString();
    }

    public KeyCode InputKey(Keys _keyName)
    {
        return KeySetting[(int)_keyName];
    }

    public bool KeyDown(Keys _keyName)
    {
        return Input.GetKeyDown(InputKey(_keyName));
    }

    public bool KeyUp(Keys _keyName)
    {
        return Input.GetKeyUp(InputKey(_keyName));
    }

    public bool KeyOn(Keys _keyName)
    {
        return Input.GetKey(InputKey(_keyName));
    }

    public float HorizontalKey()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float VerticalKey()
    {
        return Input.GetAxisRaw("Vertical");
    }
}
