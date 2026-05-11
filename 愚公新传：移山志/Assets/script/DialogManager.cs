using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using TMPro;

using UnityEngine.UI;



public class DiaLogmanager : MonoBehaviour

{
    private bool needToFindUIComponents = false;
    // 添加单例实例
    public static DiaLogmanager Instance { get; private set; }


    /// <summary>

    /// 对话内容文本，csv格式

    /// </summary> 

    public TextAsset dialogDataFile;



    /// <summary>

    /// 左侧角色图像

    /// </summary>

    public SpriteRenderer spriteLeft;

    /// <summary>

    /// 右侧角色图像

    /// </summary>

    public SpriteRenderer spriteRight;



    /// <summary>

    /// 角色名字文本

    /// </summary>

    public TMP_Text nameText;



    /// <summary>

    /// 对话内容文本

    /// </summary>

    public TMP_Text dialogText;



    /// <summary>

    /// 角色图片列表

    /// </summary>

    public List<Sprite> sprites = new List<Sprite>();



    /// <summary>

    /// 角色名字对应图片的字典

    /// </summary>

    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();

    /// <summary>

    /// 当前对话索引值

    /// </summary>

    public int dialogIndex;

    /// <summary>

    /// 对话文本按行分割

    /// </summary>

    public string[] dialogRows;

    /// <summary>

    /// 继续按钮

    /// </summary>

    public Button next;



    /// <summary>

    /// 选项按钮

    /// </summary>

    public GameObject optionButton;

    /// <summary>

    /// 选项按钮父节点

    /// </summary>

    public Transform buttonGroup;

    public GameObject[] panelsToHide;


    // Start is called before the first frame update

    public bool isDialogueActive = false;

    private void Awake()

    {
        //单例初始化
        if (Instance == null)
        {
            Instance = this;
            // 如果需要跨场景保持，可以取消下面一行的注释
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //图片初始化
        imageDic["愚公"] = sprites[0];

        imageDic["后生"] = sprites[1];

    }

    void Start()

    {
        //显示隐藏
        ShowAllPanels();

        ReadText(dialogDataFile);

        ShowDiaLogRow();

        //UpdateText("愚公", "后生");

        //UpdateImage("后生", false);//不在左侧

        // UpdateImage("愚公", true);//在左侧

    }

    // Update is called once per frame

    void Update()

    {



    }

    /// <summary>
    /// 显示所有指定的面板
    /// </summary>
    private void ShowAllPanels()
    {
        if (panelsToHide == null || panelsToHide.Length == 0)
        {
            Debug.LogWarning("panelsToHide数组为空，请在Unity编辑器中拖入需要隐藏的面板");
            return;
        }

        foreach (GameObject panel in panelsToHide)
        {
            if (panel != null)
            {
                panel.SetActive(true);
                Debug.Log($"已显示面板: {panel.name}");
            }
        }

        // 确保继续按钮也显示
        if (next != null && !next.gameObject.activeSelf)
        {
            next.gameObject.SetActive(true);
        }
    }



    //更新文本信息

    public void UpdateText(string _name, string _text)

    {

        nameText.text = _name;

        dialogText.text = _text;

    }

    //更新图片信息

    public void UpdateImage(string _name, string _position)

    {

        if (_position == "左")

        {

            spriteLeft.sprite = imageDic[_name];

        }

        else if (_position == "右")

        {

            spriteRight.sprite = imageDic[_name];

        }

    }



    public void ReadText(TextAsset _textAsset)

    {

        dialogRows = _textAsset.text.Split('\n');//以换行来分割

        // foreach(var row in rows)

        //{

        // string[] cell = row.Split(',');

        // }

        Debug.Log("读取成果");

    }



    public void ShowDiaLogRow()

    {

        for (int i = 0; i < dialogRows.Length; i++)

        {

            string[] cells = dialogRows[i].Split(',');

            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)

            {

                UpdateText(cells[2], cells[4]);

                UpdateImage(cells[2], cells[3]);



                dialogIndex = int.Parse(cells[5]);

                next.gameObject.SetActive(true);

                break;

            }

            else if (cells[0] == "@" && int.Parse(cells[1]) == dialogIndex)

            {

                next.gameObject.SetActive(false);//隐藏原来的按钮

                GenerateOption(i);

            }

            else if ((cells[0] == "END"|| cells[0] == "end") && int.Parse(cells[1]) == dialogIndex)

            {

                Debug.Log("对话结束");//这里结束
                EndDialogue(); //调用结束方法
                break;
            }

        }

    }
    public void EndDialogue()
    {
        //隐藏对话UI
        if (next != null)
            next.gameObject.SetActive(false);

        if (nameText != null)
            nameText.text = "";

        if (dialogText != null)
            dialogText.text = "";

        //隐藏角色图片
        if (spriteLeft != null)
            spriteLeft.sprite = null;

        if (spriteRight != null)
            spriteRight.sprite = null;

        HideAllPanels();
        isDialogueActive = false;
        Debug.Log("对话结束，所有UI元素已隐藏");

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject, 0.1f);
        }
        else
        {
            // 如果没有父对象，销毁当前对象
            Destroy(gameObject, 0.1f);
        }
    }
    /// <summary>
    /// 隐藏所有指定的面板
    /// </summary>
    private void HideAllPanels()
    {
        if (panelsToHide == null || panelsToHide.Length == 0)
        {
            Debug.LogWarning("panelsToHide数组为空，没有要隐藏的面板");
            return;
        }

        foreach (GameObject panel in panelsToHide)
        {
            if (panel != null)
            {
                panel.SetActive(false);
                Debug.Log($"已隐藏面板: {panel.name}");
            }
        }
    }
    public void OnClickNext()

    {

        ShowDiaLogRow();

    }

    public void GenerateOption(int _index)//生成按钮

    {

        string[] cells = dialogRows[_index].Split(',');

        if (cells[0] == "@")

        {

            GameObject button = Instantiate(optionButton, buttonGroup);

            //绑定按钮事件

            button.GetComponentInChildren<TMP_Text>().text = cells[4];

            button.GetComponent<Button>().onClick.AddListener(delegate

            {

                OnOptionClick(int.Parse(cells[5]));

            }

            );

            GenerateOption(_index + 1);

        }



    }



    public void OnOptionClick(int _id)

    {

        dialogIndex = _id;

        ShowDiaLogRow();

        for (int i = 0; i < buttonGroup.childCount; i++)

        {

            Destroy(buttonGroup.GetChild(i).gameObject);

        }

    }



    /// <summary>
    /// 开始新的对话
    /// </summary>
    /// <param name="newDialogDataFile">可选的新的对话CSV文件</param>
    public void StartNewDialogue(TextAsset newDialogDataFile = null)
    {
        // 重置对话索引
        dialogIndex = 0;
        isDialogueActive = true;

        // 清理现有的选项按钮
        if (buttonGroup != null)
        {
            foreach (Transform child in buttonGroup)
            {
                Destroy(child.gameObject);
            }
        }

        // 如果有新的对话文件，就使用它
        if (newDialogDataFile != null)
        {
            dialogDataFile = newDialogDataFile;
        }

        // 确保有对话文件
        if (dialogDataFile == null)
        {
            Debug.LogError("对话文件为空！");
            return;
        }

        // 显示UI面板
        ShowAllPanels();

        // 重新读取对话
        ReadText(dialogDataFile);

        // 显示第一行对话
        ShowDiaLogRow();

        Debug.Log("开始新的对话: " + dialogDataFile.name);
    }
}