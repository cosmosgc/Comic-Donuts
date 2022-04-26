using System;
using UnityEngine;
using System.Collections;

namespace PhotoViewer.Scripts
{
    public class Routines : MonoBehaviour
    {
        public void StopAllRoutines() => 
            StopAllCoroutines();

        public void LerpFloat(float from, float to, float time, Action<float> callbackOnLerp = null,
            Action callbackOnEnd = null) =>
            StartCoroutine(LerpRoutineFloat(@from, to, time, callbackOnLerp, callbackOnEnd));

        public void LerpVector2(Vector2 from, Vector2 to, float time, Action<Vector2> callbackOnLerp = null,
            Action callbackOnEnd = null) =>
            StartCoroutine(LerpRoutineVector2(@from, to, time, callbackOnLerp, callbackOnEnd));

        private IEnumerator LerpRoutineVector2(Vector2 from, Vector2 to, float time, Action<Vector2> callbackOnLerp,
            Action callbackOnEnd)
        {
            var value = from;
            float timer = 0;

            while (timer < time)
            {
                yield return null;

                timer += Time.deltaTime;
                value = Vector2.Lerp(from, to, timer / time);

                callbackOnLerp?.Invoke(value);
            }

            callbackOnLerp?.Invoke(to);
            callbackOnEnd?.Invoke();
        }

        private IEnumerator LerpRoutineFloat(float from, float to, float time, Action<float> callbackOnLerp,
            Action callbackOnEnd)
        {
            var value = from;
            float timer = 0;

            while (timer < time)
            {
                yield return null;

                timer += Time.deltaTime;
                value = Mathf.Lerp(from, to, timer / time);

                callbackOnLerp?.Invoke(value);
            }

            callbackOnLerp?.Invoke(to);
            callbackOnEnd?.Invoke();
        }
    }
}