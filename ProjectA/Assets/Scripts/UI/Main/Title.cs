using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Title : MonoBehaviour
{
    //타이틀 만들고 이벤트 등록
    //새로하기 이어하기 종료

    private UIDocument uiDocument;
    [SerializeField] private Button newgameBtn;
    [SerializeField] private VisualElement newgameAnswer;
    [SerializeField] private Button newgameYesBtn;
    [SerializeField] private Button newgameNoBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button quitBtn;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        InitUI();
    }

    private void InitUI()
    {
        var root = uiDocument.rootVisualElement;

        newgameBtn = root.Q<Button>("newgame");
        newgameAnswer = root.Q<VisualElement>("newgame_answer");
        newgameYesBtn = root.Q<Button>("newgame_yes");
        newgameNoBtn = root.Q<Button>("newgame_no");

        continueBtn = root.Q<Button>("continue");
        quitBtn = root.Q<Button>("quit");

        if (!File.Exists(Path.Combine(Application.persistentDataPath, "Datas", "PLAYER_STAT.json")))
        {
            continueBtn.focusable = false;
            continueBtn.pickingMode = PickingMode.Ignore;
            continueBtn.style.color = Color.gray;
        }

        newgameBtn.clicked += () => { newgameAnswer.style.display = DisplayStyle.Flex; };
        newgameYesBtn.clicked += NewgameYes;
        newgameNoBtn.clicked += () => { newgameAnswer.style.display = DisplayStyle.None; };
    }

    private void NewgameYes()
    {
        //씬 넘기기
        GameManager.IsNewGame = true;
        SceneManager.LoadScene("LoadDataPlayer");
    }

    private void Continue()
    {
        GameManager.IsNewGame = false;
        SceneManager.LoadScene("LoadDataPlayer");
    }

    private void Quit()
    {
        Application.Quit();
    }
}
