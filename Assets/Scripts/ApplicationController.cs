using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HeathenEngineering.SteamworksIntegration;
using Unity.Netcode;
using VContainer.Unity;
using VContainer;
using System;
using Monke.Utilities;

namespace Systems{

     /// <summary>
    /// An entry point to the application, where we bind all the common dependencies to the root DI scope. Performs initialization.
    /// </summary>
        
    public class ApplicationController : LifetimeScope

    {
        [SerializeField] float initProgress;
        [SerializeField] NetworkManager m_NetworkManager;
        // Start is called before the first frame update
        
        protected override void Configure (IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(m_NetworkManager);

        }

        void Start()
        {
            StartCoroutine(ValidateSteamConnection());
            DontDestroyOnLoad(gameObject);
            Application.wantsToQuit += onWantstoQuit;
        }
        
        /// <summary>
        ///     In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
        ///     So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
        /// </summary>
        private bool onWantstoQuit()
        {
            throw new NotImplementedException();
        }

        #region SteamAPIConnection


        public float getLoadProgress(){
            return initProgress;
        }

        private IEnumerator ValidateSteamConnection(){

            yield return null;

            // check for dependent systems or integrations; load required data (like system settings, etc.)

            Debug.Log( "Waiting... 3");
            yield return new WaitForSeconds(0f);
            Debug.Log( "Waiting... 2");
            yield return new WaitForSeconds(0f);
            Debug.Log( "Waiting... 1");
            yield return new WaitForSeconds(0f);

            
            // UNCOMMENT BELOW FOR STEAM-COMPATIBLE BUILD
            
            // yield return new WaitUntil(() => SteamSettings.Initialized);

            // Debug.Log( "Steam API is initialized. Starting Scene Load");
            // Must load the scene additively, meaning no scene unloaded
            // For additive structure, manually specify what scene gets unloaded and when.
            // never unload bootstrap; will house camera, other system-level things.

           LoadMenu(); // 1 is the main menu, TODO: make an enum list with the proper scene names?

        }

        private void LoadMenu(){
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);

        }
        #endregion
    }
}
