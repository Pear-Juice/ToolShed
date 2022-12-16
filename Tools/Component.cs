using System;
using System.Collections.Generic;
using System.Linq;
using CloneExtensions;
using Helpers;
using UnityEngine;

namespace ToolShed.Tools
{
    public class Component
    {
        public ConnectionDict inputs = new();
        public ConnectionDict outputs = new();

        public Component clone()
        {
            var inputClone = this.GetClone();

            //clone inputs components and reattach links
            var keyValuePairs = inputs.dict.ToList();
            foreach (var inputPair in keyValuePairs)
            {
                var comp = (Component)inputPair.Value[1];
                var outputComp = comp.clone();

                foreach (var outputPair in outputComp.outputs.dict)
                    outputComp.outputs.Reconnect(outputPair.Key, outputComp, inputClone, inputPair.Key);
            }

            return inputClone;
        }

        public virtual Component cloneSub()
        {
            return null;
        }

        public class ConnectionDict
        {
            public Dictionary<string, List<object>> dict = new();

            public void Add<T>(string name, Action<T> action)
            {
                dict.Add(name, new List<object>(2) { new Connection<T>(action), null });
            }

            public Connection<T> Get<T>(string name)
            {
                return (Connection<T>)dict[name][0];
            }

            public void Set<T>(string name, Connection<T> connection)
            {
                dict[name][0] = connection;
            }

            public void Remove(string name)
            {
                dict.Remove(name);
            }

            public Component getComp(string name)
            {
                return (Component)dict[name][1];
            }

            public void setComp(string name, Component comp)
            {
                dict[name][1] = comp;
            }

            public void Connect<T>(string outputName, Component outputComp, Component inputComp, string inputName)
            {
                Get<T>(outputName).setAction = inputComp.inputs.Get<T>(inputName).setAction;
                //set this this output to have connections comp
                setComp(outputName, inputComp);
                //set connections input to have this comp
                inputComp.inputs.setComp(inputName, outputComp);
            }

            public void Reconnect(string outputName, Component outputComp, Component inputComp, string inputName)
            {
                dict[outputName][0].TrySetProperty("setAction", inputComp.inputs.dict[inputName][0]);
                //set this this output to have connections comp
                setComp(outputName, inputComp);
                //set connections input to have this comp
                inputComp.inputs.setComp(inputName, outputComp);
            }
        }

        public class Connection<T>
        {
            public Action<T> setAction;
            private T value;

            public Connection(Action<T> setAction)
            {
                this.setAction = setAction;
                if (this.setAction != null)
                    this.setAction += v => value = v;
            }

            public T Value
            {
                get => value;
                set
                {
                    this.value = value;
                    Debug.Log("Values are being set");
                    setAction?.Invoke(value);
                }
            }
        }
    }
}