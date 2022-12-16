using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ToolShed.Tools
{
    public class Clock
    {
        private readonly MonoBehaviour mono;
        public int count;
        public Action pass;
        public float rate;
        private bool run;

        public Clock(float rate)
        {
            this.rate = rate;
            mono = Object.FindObjectOfType<MonoBehaviour>();
        }

        public void start()
        {
            run = true;
            mono.StartCoroutine(tick());
        }

        public void stop()
        {
            run = false;
            count = 0;
        }

        private IEnumerator tick()
        {
            while (run)
            {
                pass?.Invoke();
                count++;
                yield return new WaitForSeconds(rate);
            }
        }
    }
}