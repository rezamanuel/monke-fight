using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ConsoleLogBehavior : MonoBehaviour
{
    ConsoleLogBehavior Instance;
    [SerializeField] private Queue<string> UI_LogQueue = new Queue<string>();
    [SerializeField] private int LogQueueCapacity = 500; // no more than 500 log messages
    [SerializeField] private TextMeshProUGUI UI_ConsoleText;
    private ScrollRect UI_ScrollRect;

    bool isVisible;
     void Awake(){

        if(Instance != null){
            Destroy(this);
        }
        else{
            Instance = this;
            
        }

        UI_ScrollRect = UI_ConsoleText.gameObject.GetComponentInParent<ScrollRect>();
        isVisible =false;
        transform.GetChild(0).gameObject.SetActive(false);

     }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
        
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string console_contents = UI_ConsoleText.text;

        // check if queue is full
        if(UI_LogQueue.Count >= LogQueueCapacity) {
            // TODO add stacktrace option.
            UI_LogQueue.Dequeue();
            
            int first_log_index = console_contents.IndexOf('\n', 1);
           
            console_contents = console_contents.Substring(first_log_index,console_contents.Length- first_log_index);
        }

        string logItem = "["+System.DateTime.Now+"]"+"  " +logString;
        
        UI_LogQueue.Enqueue(logItem);
        UI_ConsoleText.text =  console_contents + '\n'+logItem ;
        // pass string to ConsoleLogUI
        StartCoroutine(EndOfFrame());
    }

    IEnumerator EndOfFrame(){
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        UI_ScrollRect.verticalScrollbar.value= 0;
    }

    void Update(){
        if(Input.GetButtonDown("Console Log")){
            transform.GetChild(0).gameObject.SetActive(!isVisible);
            isVisible = !isVisible;
        }
    }

}
