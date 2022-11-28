using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HeathenEngineering.SteamworksIntegration;

namespace Systems{
        
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] float loadProgress;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Validate());
        }

        public float getLoadProgress(){
            return loadProgress;
        }

        private IEnumerator Validate(){

            yield return null;

            // check for dependent systems or integrations; load required data (like system settings, etc.)

            Debug.Log( "Waiting... 3");
            yield return new WaitForSeconds(1f);
            Debug.Log( "Waiting... 2");
            yield return new WaitForSeconds(1f);
            Debug.Log( "Waiting... 1");
            yield return new WaitForSeconds(1f);

            yield return new WaitUntil(() => SteamSettings.Initialized);

            Debug.Log( "Steam API is initialized. Starting Scene Load");
            // Must load the scene additively, meaning no scene unloaded
            // For additive structure, manually specify what scene gets unloaded and when.
            // never unload bootstrap; will house camera, other system-level things.
            
            var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            
            operation.allowSceneActivation = true; // load in as soon as it's ready.

            while (!operation.isDone){
                loadProgress = operation.progress;
                yield return new WaitForEndOfFrame();
            }
            loadProgress = 1f;
        }
    }
}
