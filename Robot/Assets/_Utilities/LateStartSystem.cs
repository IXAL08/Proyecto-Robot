using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Robot
{
    public static class LateStartSystem
    {
        private static bool initialized = false;
        private static bool executed = false;
        private static readonly List<Action> actions = new();

        public static void ExecuteOnLateStart(Action action)
        {
            if (executed)
            {
                // Si ya pasó el LateStart, ejecuta inmediatamente
                action?.Invoke();
                return;
            }

            actions.Add(action);

            if (!initialized)
            {
                initialized = true;
                RunLateStart().Forget();
            }
        }

        private static async UniTaskVoid RunLateStart()
        {
            // Espera a que Unity haya ejecutado todos los Start()
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            executed = true;

            foreach (var a in actions)
                a?.Invoke();

            actions.Clear();
        }
    }
}