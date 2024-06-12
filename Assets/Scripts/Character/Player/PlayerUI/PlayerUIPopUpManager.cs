using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AN
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("Death PopUp")] 
        [SerializeField] private GameObject deathPopUpGameObject;
        [SerializeField] private TextMeshProUGUI deathPopUpBackGroundText;
        [SerializeField] private TextMeshProUGUI deathPopUpText;
        [SerializeField] private CanvasGroup deathPopUpCanvasGroup;

        public void OpenDeathPopUp()
        {
            //Todo: activate postprocessing effect (Bloom,... etc)
            
            deathPopUpGameObject.SetActive(true);
            deathPopUpBackGroundText.characterSpacing = 0;
            
            //Stretch out popup
            StartCoroutine(StretchPopUpOvertime(deathPopUpBackGroundText, 8, 20));
            //Fade in popup
            StartCoroutine(FadeInPopUpOvertime(deathPopUpCanvasGroup,5));
            //Wait, then fade out popup
            StartCoroutine(WaitThenFadeOutPopUpOvertime(deathPopUpCanvasGroup,2,5));
        }

        public IEnumerator StretchPopUpOvertime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0)
            {
                text.characterSpacing = 0;
                float timer = 0;
                
                // yield return null will pause the execution until the next frame.
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;

                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * Time.deltaTime);
                    
                    
                    // yield return null will pause the execution until the next frame.
                    yield return null;
                }
            }

        }
        
        public IEnumerator FadeInPopUpOvertime(CanvasGroup canvas, float duration)
        {
            if (duration > 0)
            {
                canvas.alpha = 0;
                float timer = 0;
                
                // yield return null will pause the execution until the next frame.
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;

                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    
                    // yield return null will pause the execution until the next frame.
                    yield return null;
                }
                
            }
            
            canvas.alpha = 1;
            
            // yield return null will pause the execution until the next frame.
            yield return null;

        }
        
        public IEnumerator WaitThenFadeOutPopUpOvertime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0)
            {
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    // yield return null will pause the execution until the next frame.
                    yield return null;
                }
                
                canvas.alpha = 1;
                float timer = 0;
                
                // yield return null will pause the execution until the next frame.
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;

                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    
                    // yield return null will pause the execution until the next frame.
                    yield return null;
                }
                
            }
            
            canvas.alpha = 0;
            
            // yield return null will pause the execution until the next frame.
            yield return null;

        }
    }
}
