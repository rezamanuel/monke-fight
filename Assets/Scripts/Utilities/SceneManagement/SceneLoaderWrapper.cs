using UnityEngine;
using UnityEngine.SceneManagement;
using Monke.Utilities;
using Unity.Netcode;
using System;
namespace Monke.Utilities
{
    public class SceneLoaderWrapper : NetworkBehaviour
    {
        /// <summary>
        /// Manages a loading screen by wrapping around scene management APIs. It loads scene using the SceneManager,
        /// or, on listening servers for which scene management is enabled, using the NetworkSceneManager and handles
        /// the starting and stopping of the loading screen.
        /// </summary>

        [SerializeField]
        ClientLoadingScreen m_ClientLoadingScreen;

        [SerializeField]
        LoadingProgressManager m_LoadingProgressManager;

        bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

        public static SceneLoaderWrapper Instance { get; protected set; }

#if UNITY_EDITOR
        public UnityEditor.SceneAsset SceneAsset;
        private void OnValidate()
        {
            if (SceneAsset != null)
            {
                m_SceneName = SceneAsset.name;
            }
        }
#endif
        [SerializeField]
        private string m_SceneName;
        private Scene m_LoadedScene;

        private string m_SceneToLoadNext; // scene name that will load after the current scene is unloaded on all clients.
        public event Action OnClientSynchronized;
        public bool SceneIsLoaded
        {
            get
            {
                if (m_LoadedScene.IsValid() && m_LoadedScene.isLoaded)
                {
                    return true;
                }
                return false;
            }
        }

        public virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(this);
        }


        public override void OnNetworkSpawn()
        {
            if (IsServer && !string.IsNullOrEmpty(m_SceneName))
            {
                var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
                CheckStatus(status);
            }

            base.OnNetworkSpawn();
        }

        public void UnloadScene()
        {
            // Assure only the server calls this when the NetworkObject is
            // spawned and the scene is loaded.
            if (!IsServer || !IsSpawned || !m_LoadedScene.IsValid() || !m_LoadedScene.isLoaded)
            {
                return;
            }

            // Unload the scene
            var status = NetworkManager.SceneManager.UnloadScene(m_LoadedScene);
            CheckStatus(status, false);
        }
        private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
        {
            var sceneEventAction = isLoading ? "load" : "unload";
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to {sceneEventAction} {m_SceneName} with" +
                    $" a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
        public virtual void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            base.OnDestroy();
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager != null && NetworkManager.SceneManager != null)
            {
                NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }

        /// <summary>
        /// Initializes the callback on scene events. This needs to be called right after initializing NetworkManager
        /// (after StartHost, StartClient or StartServer)
        /// </summary>
        public virtual void AddOnSceneEventCallback()
        {
            if (IsNetworkSceneManagementEnabled)
            {
                NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }

        /// <summary>
        /// Loads a scene asynchronously using the specified loadSceneMode, with NetworkSceneManager if on a listening
        /// server with SceneManagement enabled, or SceneManager otherwise. If a scene is loaded via SceneManager, this
        /// method also triggers the start of the loading screen.
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        /// <param name="useNetworkSceneManager">If true, uses NetworkSceneManager, else uses SceneManager</param>
        /// <param name="loadSceneMode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
        public virtual void LoadScene(string sceneName, bool useNetworkSceneManager, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            if (useNetworkSceneManager)
            {
                if (IsSpawned && IsNetworkSceneManagementEnabled && !NetworkManager.ShutdownInProgress)
                {
                    if (NetworkManager.IsServer)
                    {
                        // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                        NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                    }
                }
            }
            else
            {
                // Load using SceneManager
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                m_ClientLoadingScreen.StartLoadingScreen(sceneName);
                m_LoadingProgressManager.LocalLoadOperation = loadOperation;
            }
        }
        public void QueueNextScene(string sceneName){
            m_SceneToLoadNext = sceneName;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                m_ClientLoadingScreen.StopLoadingScreen();
            }
        }

        void OnSceneEvent(SceneEvent sceneEvent)
        {
            var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                                          // Only executes on client
                    if (NetworkManager.IsClient)
                    {
                        m_ClientLoadingScreen.StartLoadingScreen(sceneEvent.SceneName);
                        m_LoadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;

                    }
                    break;
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                                                        // Only executes on client
                    if (NetworkManager.IsClient)
                    {
                        Debug.Log($"All clients finished loading the {sceneEvent.SceneName} scene.");
                        m_ClientLoadingScreen.StopLoadingScreen();
                        m_LoadingProgressManager.ResetLocalProgress();
                        OnClientSynchronized?.Invoke();
                    }
                    break;
                case SceneEventType.LoadComplete:
                    {
                        // We want to handle this for only the server-side
                        if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                        {
                            // *** IMPORTANT ***
                            // Keep track of the loaded scene, you need this to unload it
                            m_LoadedScene = sceneEvent.Scene;
                        }
                        Debug.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                            $"{clientOrServer}-({sceneEvent.ClientId}).");
                        break;
                    }
                case SceneEventType.UnloadComplete:
                    {
                        Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on " +
                            $"{clientOrServer}-({sceneEvent.ClientId}).");
                        break;
                    }
                case SceneEventType.UnloadEventCompleted:
                    {
                        var loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                        Debug.Log($"{loadUnload} event completed for the following client " +
                            $"identifiers:({sceneEvent.ClientsThatCompleted})");
                        
                        if(m_SceneToLoadNext != null){
                            LoadScene(m_SceneToLoadNext, true);
                            m_SceneToLoadNext = null;
                        }

                        if (sceneEvent.ClientsThatTimedOut.Count > 0)
                        {
                            Debug.LogWarning($"{loadUnload} event timed out for the following client " +
                                $"identifiers:({sceneEvent.ClientsThatTimedOut})");
                        }
                        break;
                    }
                case SceneEventType.Synchronize: // Server told client to start synchronizing scenes
                    {
                        // Only executes on client that is not the host
                        if (NetworkManager.IsClient && !NetworkManager.IsHost)
                        {
                            // unload all currently loaded additive scenes so that if we connect to a server with the same
                            // main scene we properly load and synchronize all appropriate scenes without loading a scene
                            // that is already loaded.
                            UnloadAdditiveScenes();
                        }
                        break;
                    }
                case SceneEventType.SynchronizeComplete: // Client told server that they finished synchronizing
                                                         // Only executes on server
                    if (NetworkManager.IsServer)
                    {
                        // Send client RPC to make sure the client stops the loading screen after the server handles what it needs to after the client finished synchronizing, for example character spawning done server side should still be hidden by loading screen.
                        StopLoadingScreenClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { sceneEvent.ClientId } } });

                    }
                    break;
            }
        }

        void UnloadAdditiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        [ClientRpc]
        void StopLoadingScreenClientRpc(ClientRpcParams clientRpcParams = default)
        {
            Debug.Log("Stopping loading screen on client.");
            m_ClientLoadingScreen.StopLoadingScreen();  
            OnClientSynchronized?.Invoke();
        }
    }
}