using UnityEngine;

public class InputFieldHandler : MonoBehaviour
{
	public AndroidKeyBoard[] Inputs;
	private int count;
	private int currentField;

	// Use this for initialization
	private void Start ()
    {
		count = 0;
		Inputs = Component.FindObjectsOfType<AndroidKeyBoard> ();
		Ini_Inputs ();
	}

	private void Ini_Inputs()
    {
		if (Inputs != null)
        {
			for (int i = 0; i < Inputs.Length; i++)
            {
				Inputs [i].set_id (count);
				count++;
			}
		}
	}

	public void set_currentField(int id)
    {
		currentField = id;
	}

	public int get_currentField()
    {
		return currentField;
	}
}
