using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour, IStatObserver
{
    public static HealthIndicator Instance;

    [SerializeField] private Slider health;
    private Coroutine c_healthHolder;
    private RectTransform healthRect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.dontDestroyObjects.Add(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        healthRect = health.GetComponent<RectTransform>();
    }

    private void Start()
    {
        PlayerStat.Instance.AddObserver(this);
    }

    private void OnDestroy()
    {
        PlayerStat.Instance.RemoveObserver(this);
    }

    public void OnHealthChanged(int _maxHealth, int _changedHealth)
    {
        //maxHealth�� ���� ü�¹� ���� ����
        healthRect.sizeDelta = new Vector2(CalcBarWidth(_maxHealth), healthRect.sizeDelta.y);
        //�ִ� ����
        health.maxValue = _maxHealth;

        //changedHealth�� ���� value ����
        //�ε巯�� ü�� ����
        if (c_healthHolder!= null)
            StopCoroutine(c_healthHolder);
        c_healthHolder = StartCoroutine(StatLerp(health, _maxHealth, _changedHealth));
    }

    public void OnSoulChanged(int _currentSoul)
    {
        //lerp soul
    }


    private int CalcBarWidth(int _maxValue)
    {
        int defaultWidth = 200;

        //���� 10�� 5�� ����
        defaultWidth += (_maxValue / 2);
        return defaultWidth;
    }

    private IEnumerator StatLerp(Slider target, int _max, int _to)
    {
        float time = 0;
        float from = target.value;
        
        while (true)
        {
            time += Time.deltaTime * 10f;

            target.value = Mathf.Lerp(from, _to, time);

            if (time >= 1f) break;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
