﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    public static List<string[]> CurrentChoices;  // 直前に来た選択肢の一覧を持っている

    public GameObject ChoiceField;
    Transform choiceTransform;
    public GameObject ChoicePrefab;
    public GameObject ChoicePrefabSystemButton;
    public GameObject ChoicePrefabSystemWindow;

    public GameObject manager;
    DialogManager dialogManager;

    private static int quitCounter = 0;  // 連打しないとescしないように
    private static bool buttonflag = false;  // ボタン連打ができないように

    void Start()
    {
        dialogManager = manager.GetComponent<DialogManager>();
    }
    void Update()
    {
        if (ChoiceClass.WaitOperation == true)
        {
            if (this.OnReceive(ChoiceClass.InputHolder) < 1)
            {  // うけとっためっせーじから選択肢を生成
                dialogManager.myclient.Send("<SendAgain>");
            }
            ChoiceClass.WaitOperation = false;
            
        }

    }

    public void OnQuitRequest()
    {
        quitCounter++;
        if (quitCounter > 5)
        {
            Quit();
        }
    }

    void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
                UnityEngine.Application.Quit();
        #endif
    }


    public bool IsChoice(string str)
    {
        string[] temp = str.Split(':');
        Debug.Log(temp[0]);
        if (temp[0] == "<Choice>")
        {
            return true;
        }
        else if (temp[0] == "<Clear>")
        {
            choiceTransform = ChoiceField.transform;
            foreach (Transform n in choiceTransform)
            {
                GameObject.Destroy(n.gameObject);
            }
            choiceTransform.DetachChildren();  // 過去の子供を全員抹消
            return false;
        }
        else
        {
            return false;
        }
    }

    public static List<string[]> ParserC(string str)
    {
        List<string[]> temp = new List<string[]>();
        string[] calcTemp = str.Split(';');

        foreach (string calcDetail in calcTemp)
        {
            temp.Add(calcDetail.Split(','));
        }

        return temp;
    }

    private int OnReceive(string message)
    {
        quitCounter = 0;

        if (IsChoice(message))
        {
            CurrentChoices = ParserC(message.Split(':')[1]);  // List<string[]>で全選択肢の情報を持っている→これを各ボタンに割り当てる
            Debug.Log(message.Split(':')[1]);

 
        }
        else
        {
            Debug.Log("不正な入力（選択肢ではない）");
            return 0;
        }

        choiceTransform = ChoiceField.transform;
        foreach (Transform n in choiceTransform)
        {
            GameObject.Destroy(n.gameObject);
        }
        choiceTransform.DetachChildren();  // 過去の子供を全員抹消

        int i = 0;
        foreach (string[] choice in CurrentChoices)
        {
            Debug.Log(choice[2][0]);
            GameObject listChoice = Instantiate(ChoicePrefab) as GameObject; ;
            if (choice[2][0] == '@')
            {
                //プレハブからボタンを生成
                listChoice = Instantiate(ChoicePrefabSystemButton) as GameObject;
            }
            if (choice[2][0] == '&')
            {
                //プレハブからボタンを生成
                listChoice = Instantiate(ChoicePrefabSystemWindow) as GameObject;
            }
            if (choice[2][0] == '$')
            {
                string log = "<Command>:" + ImportedConst.YourID + "," + CurrentChoices[i][0] + "," + CurrentChoices[i][3] + "," + CurrentChoices[i][4];  // ロボID、発話ID、読まれる内容そのまま、ジェスチャのための態度
                                                                                                                                                          // dialogManager.myclient.Send(log);

                if (ChoiceClass.LastSpeaker == "A")
                    ChoiceField.transform.Rotate(new Vector3(0, 0, 0));
                if (ChoiceClass.LastSpeaker == "B")
                    ChoiceField.transform.Rotate(new Vector3(0, 0, -90));
                if (ChoiceClass.LastSpeaker == "C")
                    ChoiceField.transform.Rotate(new Vector3(0, 0, -180));
                if (ChoiceClass.LastSpeaker == "D")
                    ChoiceField.transform.Rotate(new Vector3(0, 0, -270));

                Debug.Log("rotation");
                Debug.Log(CurrentChoices[0][1]);
                if (CurrentChoices[0][1] == "A")
                {
                    ChoiceField.transform.Rotate(new Vector3(0, 0, 0));
                    Debug.Log("plus 0");
                }

                if (CurrentChoices[0][1] == "B")
                {
                    ChoiceField.transform.Rotate(new Vector3(0, 0, 90));
                    Debug.Log("plus 90");
                }
                if (CurrentChoices[0][1] == "C")
                {
                    ChoiceField.transform.Rotate(new Vector3(0, 0, 180));
                    Debug.Log("plus 180");
                }

                if (CurrentChoices[0][1] == "D")
                {
                    ChoiceField.transform.Rotate(new Vector3(0, 0, 270));
                    Debug.Log("plus 270");
                }

                ChoiceClass.LastSpeaker = CurrentChoices[0][1];  // 最後に話したロボットのID

                i++;
                continue;
            }


            //プレハブからボタンを生成
            //Vertical Layout Group の子にする
            listChoice.transform.SetParent(choiceTransform, false);

            listChoice.transform.Find("Text").GetComponent<Text>().text = choice[2].Replace("@", "").Replace("#", "\n").Replace("&", "");

            int n = i;
            //引数に何番目のボタンかを渡す
            listChoice.GetComponent<Button>().onClick.AddListener(() => SearchOnClick(n));

            i++;

        }
        buttonflag = false;


        return i;

    }

    void SearchOnClick(int index)
    {

        if (buttonflag == true)
        {
            return;
        }

        string log = "<Command>:" + ImportedConst.YourID + "," + CurrentChoices[index][0] + "," + CurrentChoices[index][3] + "," + CurrentChoices[index][4];  // ロボID、発話ID、読まれる内容そのまま、ジェスチャのための態度
        Debug.Log("pressed by " + log);
        int i = 0;

        foreach (Transform n in choiceTransform)
        {
            if (i != index)
            {
                // GameObject.Destroy(n.gameObject);
                // n.gameObject.SetActive(false);
                n.gameObject.GetComponent<Button>().interactable = false;

            }
            else
            {
                //n.gameObject.GetComponent<Button>().isActiveAndEnabled = false;
            }
            i++;
        }

        buttonflag = true;

        Invoke("WaitBeforeDestroy", 1.8f);

        dialogManager.myclient.Send(log);

    }

    void WaitBeforeDestroy()
    {
        foreach (Transform n in choiceTransform)
        {
            if (buttonflag == true)
            {
                GameObject.Destroy(n.gameObject);
                Debug.Log("invoked");
                // choiceTransform.DetachChildren();  // 過去の子供を全員抹消
            }
        }
        buttonflag = false;
    }



}
