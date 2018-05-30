using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AndroidKeyBoard : MonoBehaviour
{
    public RectTransform inputField;

    private AndroidJavaObject activity;
    private AndroidJavaObject context;
	private string temptext;
	[SerializeField]
    private Text text;
	[SerializeField]
    private Text placeholder;
	public int id;
	private InputFieldHandler handler;

    private bool isKeyboardOpen;

    private Vector3 normalLocalPosition;

    private RectTransform panelRectTrans;
    private Vector2 panelOffsetMinOriginal;
    private float panelHeightOriginal;

    private float keyboardHeight;
    private bool shouldListenForKeyboardHeight;

    public int get_id()
    {
		return id;

	}

	public void set_id(int c)
    {
		id = c;
	}

	// Pass execution context over to the Java UI thread.
	private void Start()
	{
        normalLocalPosition = inputField.anchoredPosition;

        handler = Component.FindObjectOfType<InputFieldHandler> ();

        panelRectTrans = inputField.GetComponent<RectTransform>();
        panelHeightOriginal = panelRectTrans.rect.height;
    }

    /*
	void runOnUiThread()
	{
		Debug.Log("I'm running on the Java UI thread!");
		var plugin = new AndroidJavaClass ("openkeyboard.windforceworld.com.keyboardplugin.PluginClass");
		Debug.Log( plugin.CallStatic<string>("OpenKeyBoard",context));
	}
*/

    // Use this for initialization
    public void OpenKeyBoard ()
    {
        handler.set_currentField (id);
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		context = activity.Call<AndroidJavaObject>("getApplicationContext");

		var plugin = new AndroidJavaClass ("openkeyboard.windforceworld.com.keyboardplugin.PluginClass");

        plugin.CallStatic<string>("OpenKeyBoard", context);


        if (!isKeyboardOpen && !shouldListenForKeyboardHeight)
        {
            shouldListenForKeyboardHeight = true;
            StartCoroutine(ListenForKeyboardHeight());
        }
        //plugin.CallStatic<string>("OpenKeyBoard", context);

        /*
        if (!isKeyboardOpen)
        {
            if (!shouldListenForKeyboardHeight)
            {
                shouldListenForKeyboardHeight = true;
                StartCoroutine(ListenForKeyboardHeight());
            }
            // Debug.Log(plugin.CallStatic<string>("OpenKeyBoard", context));
            // Debug.Log(GetKeyboardHeight());
            //panel.localPosition = new Vector2(panel.localPosition.x, GetKeyboardHeight());

            // StartCoroutine(MoveInputUp());
            //isKeyboardOpen = true;
        }
        else
        {
            // Debug.Log(plugin.CallStatic<string>("OpenKeyBoard", context));
            //  Debug.Log("normal: " + normalLocalPosition.y);

            //inputFIeld.anchoredPosition = normalLocalPosition;
            //isKeyboardOpen = false;
        }*/

        }
    /*
    private IEnumerator MoveInputUp()
    {
        yield return new WaitForSeconds(0.1f);

        Debug.Log("moved: " + GetKeyboardHeight());
        inputField.anchoredPosition = new Vector2(inputField.localPosition.x, GetKeyboardHeight());
    }
    */
    private void Update()
    {
        if (keyboardHeight > 0 && !isKeyboardOpen)
        {
           // StartCoroutine(MoveInputUp());
            inputField.anchoredPosition = new Vector2(inputField.localPosition.x, keyboardHeight);
            isKeyboardOpen = true;
        }
        else if(keyboardHeight == 0 && isKeyboardOpen)
        {
            inputField.anchoredPosition = normalLocalPosition;
            isKeyboardOpen = false;
        }

        if (id == handler.get_currentField())
        {
			if (text.text.Length <= 0 && !placeholder.IsActive ())
            {
				placeholder.gameObject.SetActive (true);
				text.gameObject.SetActive (false);
			}
            else if (text.text.Length > 0 && !text.IsActive ())
            {
				placeholder.gameObject.SetActive (false);

				text.gameObject.SetActive (true);
			}

			foreach (char c in Input.inputString)
            {
				if (c == '\b' && text.text.Length != 0)
                { 
                    // has backspace/delete been pressed?
                    text.text = text.text.Substring(0, text.text.Length - 1);
                }
                else if ((c == '\n') || (c == '\r'))
                {
                    // enter/return
					print ("User entered their name: " + text.text);
				}
                else
                {
					text.text += c;
				}

				//Debug.Log (text);
			}
		}
    }
    /*
    private float GetKeyboardHeight()
    {
        //StartCoroutine(GetKeyboardHeightShow());

        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", rect);
                return (float)(Screen.height - rect.Call<int>("height"));
            }
        }
    }*/
    
    private IEnumerator ListenForKeyboardHeight()
    {
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                while (shouldListenForKeyboardHeight)
                {
                    View.Call("getWindowVisibleDisplayFrame", rect);
                    keyboardHeight = (float)(Screen.height - rect.Call<int>("height"));

                    Debug.Log(keyboardHeight);

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }
}
