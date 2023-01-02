using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingTextBehaviour : MonoBehaviour
{
    TextMeshProUGUI m_TMPComponent;
    private List<string> LoadingTextList = new List<string>();
    private int m_ListIndex = 0;
    void Awake()
    {
        m_TMPComponent = this.GetComponent<TextMeshProUGUI>();
        LoadingTextList.Add("Loading.");
        LoadingTextList.Add("Loading..");
        LoadingTextList.Add("Loading...");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateLoadingText());
    }

    IEnumerator UpdateLoadingText()
    {

        while (this.enabled)
        {
            yield return new WaitForSeconds(.5f);
            m_TMPComponent.SetText(LoadingTextList[m_ListIndex]);
            if (m_ListIndex < LoadingTextList.Count -1 )
            {
                m_ListIndex += 1;
            }
            else
            {
                m_ListIndex = 0;
            }

            
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
