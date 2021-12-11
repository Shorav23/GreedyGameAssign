using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class GameController : MonoBehaviour
{
    public string jsonTextOnlyURL;
    public string jsonTextColorURL;
    public string jsonFrameOnlyURL;
    public string jsonFrameColorURL;

    private string selectedTextURL;
    private string selectedFrameURL;

    public GameObject adFrame;
    public GameObject startMenu;
    public GameObject inputField;
    public GameObject text;
    public TMP_Text messageText;

    public InputField inputText;
    private int frameFlag = 0;
    private int textFlag = 0;
    private string imageURL;

    public Image textureImage;

    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello! Starting the execution.");
        //gameManager();
    }

    public void HandleTextDropDownInp(int val)
    {
        if(val == 0)
        {
            inputField.SetActive(false);
        }

        if (val == 1)
        {
            textFlag = val;
            inputField.SetActive(true);
            selectedTextURL = jsonTextOnlyURL;
        }

        if (val == 2)
        {
            textFlag = val;
            inputField.SetActive(true);
            selectedTextURL = jsonTextColorURL;
        }
    }

    public void HandleFrameDropDownInp(int val)
    {
        if(val == 0)
        {
            frameFlag = 0;
        }
        
        if(val == 1)
        {
            frameFlag = 2 + val;
            selectedFrameURL = jsonFrameOnlyURL;
        }

        if(val == 2)
        {
            frameFlag = 2 + val;
            selectedFrameURL = jsonFrameColorURL;
        }
    }

    public void GameManager()
    {
        startMenu.SetActive(false);

        if (frameFlag != 0)
        {
            StartCoroutine(GetJASONData(selectedFrameURL, frameFlag));
        }

        if(textFlag != 0)
        {
            StartCoroutine(GetJASONData(selectedTextURL, textFlag));
        }
    }

    private void RenderFrame(string jsonFile)
    {
        jsonDataClass jsonData = JsonUtility.FromJson<jsonDataClass>(jsonFile);

        adFrame.GetComponent<RectTransform>().localPosition = new Vector3(jsonData.layers[0].placement[0].position.x, jsonData.layers[0].placement[0].position.y, 0);
        adFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(jsonData.layers[0].placement[0].position.width, jsonData.layers[0].placement[0].position.height);

        if (frameFlag == 3 && string.Equals(jsonData.layers[0].type, "frame"))
        {
            imageURL = jsonData.layers[0].path;
            StartCoroutine(LoadTextureFromWeb());
        }

        else if (frameFlag == 4 && string.Equals(jsonData.layers[0].type, "frame"))
        {
            imageURL = jsonData.layers[0].path;
            StartCoroutine(LoadTextureFromWeb());
            adFrame.SetActive(true);
            if (ColorUtility.TryParseHtmlString(jsonData.layers[0].operations[0].argument, out color))
            {
                color.a = 0.3f;
                adFrame.GetComponent<Image>().color = color; 
            }
        }
    }

    private void RenderText(string jsonFile)
    {
        jsonDataClass jsonData = JsonUtility.FromJson<jsonDataClass>(jsonFile);

        text.GetComponent<RectTransform>().localPosition = new Vector3(jsonData.layers[0].placement[0].position.x,jsonData.layers[0].placement[0].position.y,0);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(jsonData.layers[0].placement[0].position.width, jsonData.layers[0].placement[0].position.height);
        //Debug.Log(jsonData.layers[0].placement[0].position.x);

        if (textFlag == 1 && string.Equals(jsonData.layers[0].type, "text"))
        {
            text.SetActive(true);
            messageText.SetText(inputText.text);
        }

        else if (textFlag == 2 && string.Equals(jsonData.layers[0].type, "text"))
        {
            text.SetActive(true);
           // Debug.Log(jsonData.layers[0].operations[0].argument);
            messageText.SetText("<color=" + jsonData.layers[0].operations[0].argument + ">" + inputText.text + "</color>");
        }
    }

    IEnumerator LoadTextureFromWeb()
    {
        //Debug.Log(imageURL);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: Invalid Template" + www.error);
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            textureImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            textureImage.SetNativeSize();
        }
    }

    IEnumerator GetJASONData(string downloadURL, int flag)
    {
        UnityWebRequest _www = UnityWebRequest.Get(downloadURL);
        yield return _www.SendWebRequest();
        if (_www.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log(_www.downloadHandler.text);
            if (flag == 3 || flag == 4)
            {
                RenderFrame(_www.downloadHandler.text);
            }

            if (flag == 1 || flag == 2)
            {
                RenderText(_www.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log("Error: Invalid Template" + _www.error);
        }
    }
}