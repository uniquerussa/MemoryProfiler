using UnityEngine;
using System.Collections;

public class ReporterMessageReceiver : MonoBehaviour 
{
	Reporter reporter ;
	void Start()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnPreStart(){
	
		if( reporter == null )
			reporter = gameObject.GetComponent<Reporter>();

		if( Screen.width < 1000 )
			reporter.size = new Vector2( 32 , 32 );
		else 
			reporter.size = new Vector2( 48 , 48);

	}

    public void SetAllCollidersStatus(bool active)
    {
        


    }

    void OnHideReporter()
	{
        GameObject UIobj = GameObject.Find("UI");

        if (UIobj != null)
        {
            Raven_ButtonMessage[] temp = UIobj.GetComponentsInChildren<Raven_ButtonMessage>();
            //Raven_ButtonMessage temp = GetComponent<Raven_ButtonMessage>();
            if (temp != null)
            {
                foreach (Raven_ButtonMessage uiSprite in temp)
                {
                    uiSprite.enabled = true;
                }
            }
        }
    }

	void OnShowReporter()
	{
        GameObject UIobj = GameObject.Find("UI");
        if (UIobj != null)
        {
            Raven_ButtonMessage[] temp = UIobj.GetComponentsInChildren<Raven_ButtonMessage>();
            //Raven_ButtonMessage temp = GetComponent<Raven_ButtonMessage>();
            if (temp != null)
            {
                foreach (Raven_ButtonMessage uiSprite in temp)
                {
                    uiSprite.enabled = false;
                }
            }
        }

    }
}
