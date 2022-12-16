using System;
using UnityEngine;

namespace ToolShed.Tools
{
    public class RunTimeHook : MonoBehaviour
    {
        public static Action pass;

        public void Update()
        {
            pass?.Invoke();
        }
    }
}