using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Monke.Utilities;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    [SerializeField] Object m_MatchScene;
    // Start is called before the first frame update
    void Awake()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            SceneManager.UnloadSceneAsync(1); // unload main menu
            
        });

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            SceneLoaderWrapper.Instance.AddOnSceneEventCallback();
             SceneManager.UnloadSceneAsync(1); // unload main menu
            SceneLoaderWrapper.Instance.LoadScene(m_MatchScene.name, true, LoadSceneMode.Additive);
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            SceneLoaderWrapper.Instance.AddOnSceneEventCallback();
            SceneManager.UnloadSceneAsync(1); // unload main menu\
             SceneLoaderWrapper.Instance.LoadScene(m_MatchScene.name, true, LoadSceneMode.Additive);

        });
    }
}
