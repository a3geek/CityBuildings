using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity
{
    using Components;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(WindowTexture))]
    [AddComponentMenu("Night City/Main Controller")]
    public class MainController : MonoBehaviour
    {
        private WindowTexture windowTexture = null;


        private void Awake()
        {
            this.windowTexture = this.windowTexture ?? GetComponent<WindowTexture>();
            this.windowTexture.Init();
        }
    }
}
