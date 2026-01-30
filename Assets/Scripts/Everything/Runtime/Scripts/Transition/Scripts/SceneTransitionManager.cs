using EasyButtons;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SceneTransition
{

    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        protected override bool DestroyOnLoad => false;

        public int forceTransition = -1;

        public Material material;
        public Transition[] transitions;
        public RawImage image;


        private bool inTransition;


        public delegate void OnTransitionEnd();

        public OnTransitionEnd onTransitionEndOnce;
        public OnTransitionEnd onTransitionEndAlways;

        private void Start()
        {
            if (DestroyIfInitialised(this)) return;

            EnsureInitialised();
            ResetMaterial();
        }
        private void OnDestroy()
        {
            if (ImTheOne(this))
                ResetMaterial();
        }

        [Button]
        void ResetMaterial()
        {
            material.SetFloat("_time", 0);
        }


        public bool IsAlreadyTransitioning()
        {
            return inTransition;
        }

        
        public void ChangeScene(string sceneName)
        {
            if (inTransition)
            {
                throw new UnityException("There is already a transition in progress");
            }

            StartCoroutine(SceneSwap(sceneName));
        }

        public void CancelCurrentTransition()
        {
            if (!inTransition)
            {
                throw new UnityException("There is no active transition");
            }

            StopAllCoroutines();
            inTransition = false;
        }

        private IEnumerator SceneSwap(string sceneName)
        {
            var sceneLoadingData = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadingData.allowSceneActivation = false;

            Transition transition = forceTransition >= 0 ? transitions[forceTransition] : transitions[Random.Range(0, transitions.Length)];

            inTransition = true;

            image.texture = transition.background;
            material.SetColor("_backgroundColor", transition.backgroundColor);
            material.SetTexture("_transitionGradient", transition.In.gradientMask);

            //Fade in
            material.SetFloat("_time", 0);

            //TODO: temp change
            material.SetFloat("_invert", Random.value > 0.5f ? 1 : 0);


            if (transition.In.enabled)
                for (float i = 0; i < transition.In.duration; i += Time.deltaTime)
                {

                    material.SetFloat("_time", transition.In.interpolation.Interpolate(i / transition.In.duration));
                    yield return null;
                }

            material.SetFloat("_time", 1);

            //TODO: temp change
            material.SetFloat("_inverted", Random.value > 0.5f ? 1 : 0);

            material.SetTexture("_transitionGradient", transition.Out.gradientMask);
            //Load Scene
            //SceneManager.LoadScene(sceneName);


            if (transition.middleScreenDuration > 0)
                yield return new WaitForSeconds(transition.middleScreenDuration);

            while (sceneLoadingData.progress < 0.9f) yield return null;

            sceneLoadingData.allowSceneActivation = true;


            //Wait screen

            //Fade out

            material.SetFloat("_invert", transition.Out.invert ? 1 : 0);


            if (transition.Out.enabled)
                for (float i = transition.Out.duration; i > 0; i -= Time.deltaTime)
                {
                    material.SetFloat("_time", transition.Out.interpolation.Interpolate(i / transition.Out.duration));
                    yield return null;
                }

            material.SetFloat("_time", 0);


            onTransitionEndAlways?.Invoke();
            onTransitionEndOnce?.Invoke();

            onTransitionEndOnce = () => { };

            inTransition = false;
        }


        public bool IsTransitionComplete()
        {
            return !inTransition;
        }

    }

}