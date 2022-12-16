using System;
using UnityEngine;

namespace ToolShed.Tools
{
    public class Stopwatch
    {
        public float endTime;
        public Action finished;
        public Action pass;
        public float time;

        public void setTime(float time)
        {
            this.time = time;
        }

        public void start(float endTime = 0)
        {
            this.endTime = endTime;
            RunTimeHook.pass += updateTime;
            RunTimeHook.pass += pass;
        }

        public void stop()
        {
            RunTimeHook.pass -= updateTime;
            RunTimeHook.pass -= pass;
        }

        public void reset(float resetTime = 0)
        {
            RunTimeHook.pass -= updateTime;
            RunTimeHook.pass -= pass;
            time = resetTime;
            endTime = 0;
        }

        public void updateTime()
        {
            time += Time.deltaTime;

            if (endTime != 0 && time >= endTime)
            {
                finished?.Invoke();
                reset();
            }
        }
    }
}