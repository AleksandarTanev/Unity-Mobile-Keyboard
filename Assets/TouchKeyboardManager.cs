using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TouchKeyboardManager : MonoBehaviour 
{
    /* public RectTransform targetToReposition;

     private TouchScreenKeyboard keyboard;

     private bool hasOpened;

     private Vector3 normalLocalPosition;

     private void Start()
     {
         normalLocalPosition = targetToReposition.localPosition;
     }

     public void OpenKeyboard()
     {
         keyboard = TouchScreenKeyboard.Open("Hiiiiiii", TouchScreenKeyboardType.ASCIICapable, false, true, false);
     }

     private void Update () 
     {
         Debug.Log("yMax " + TouchScreenKeyboard.area.yMax);
         Debug.Log("yMin " + TouchScreenKeyboard.area.yMin);
         Debug.Log("xMax " + TouchScreenKeyboard.area.xMax);
         Debug.Log("xMin " + TouchScreenKeyboard.area.xMin);

         Debug.Log("height " + TouchScreenKeyboard.area.height);
         Debug.Log("width " + TouchScreenKeyboard.area.width);

         Debug.Log("X " + TouchScreenKeyboard.area.x);
         Debug.Log("Y " + TouchScreenKeyboard.area.y);

         Debug.Log("Is visible: " + TouchScreenKeyboard.visible);

         if (TouchScreenKeyboard.visible && !hasOpened)
         {
             targetToReposition.localPosition = new Vector2(targetToReposition.localPosition.x, targetToReposition.localPosition.y - GetKeyboardSize());

             Debug.Log("Test = " + GetKeyboardSize());

             hasOpened = true;
         }
         else
         {
             if (hasOpened)
             {
                 targetToReposition.localPosition = normalLocalPosition;

                 hasOpened = false;
             }
         }
     }

     public int GetKeyboardSize()
     {
         using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
         {
             AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");

             using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
             {
                 View.Call("getWindowVisibleDisplayFrame", Rct);

                 return Screen.height - Rct.Call<int>("height");
             }
         }
     }
     */


    public InputField field;

    public void OpenKeyboard()
    {
        TouchScreenKeyboard.Open("Hiiiiiii", TouchScreenKeyboardType.ASCIICapable, false, true, false);
    }



    public void DisplayAddedText(string text)
    {
        Debug.Log(text);

        field.text = "";
    }


    // Assign panel here in order to adjust its height when TouchScreenKeyboard is shown
    public GameObject panel;

    private InputField inputField;
    private RectTransform panelRectTrans;
    private Vector2 panelOffsetMinOriginal;
    private float panelHeightOriginal;
    private float currentKeyboardHeightRatio;

    public void Start()
    {
        TouchScreenKeyboard.hideInput = true;

        inputField = transform.GetComponent<InputField>();
        inputField.shouldHideMobileInput = true;

        panelRectTrans = panel.GetComponent<RectTransform>();
        panelOffsetMinOriginal = panelRectTrans.offsetMin;
        panelHeightOriginal = panelRectTrans.rect.height;
    }

    public void LateUpdate()
    {
        if (inputField.isFocused)
        {
            float newKeyboardHeightRatio = GetKeyboardHeightRatio();
            if (currentKeyboardHeightRatio != newKeyboardHeightRatio)
            {
                Debug.Log("InputFieldForScreenKeyboardPanelAdjuster: Adjust to keyboard height ratio: " + newKeyboardHeightRatio);
                currentKeyboardHeightRatio = newKeyboardHeightRatio;
                panelRectTrans.offsetMin = new Vector2(panelOffsetMinOriginal.x, panelHeightOriginal * currentKeyboardHeightRatio);
            }
        }
        else if (currentKeyboardHeightRatio != 0f)
        {
            if (panelRectTrans.offsetMin != panelOffsetMinOriginal)
            {
                StartCoroutine(WaitForOffset());
            }
            currentKeyboardHeightRatio = 0f;
        }
    }

    private IEnumerator WaitForOffset()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("InputFieldForScreenKeyboardPanelAdjuster: Revert to original");
        panelRectTrans.offsetMin = panelOffsetMinOriginal;
    }

    private float GetKeyboardHeightRatio()
    {
        if (Application.isEditor)
        {
            return 0.4f; // fake TouchScreenKeyboard height ratio for debug in editor        
        }

#if UNITY_ANDROID        
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", rect);
                return (float)(Screen.height - rect.Call<int>("height")) / Screen.height;
            }
        }
#else
        return (float)TouchScreenKeyboard.area.height / Screen.height;
#endif
    }
}
