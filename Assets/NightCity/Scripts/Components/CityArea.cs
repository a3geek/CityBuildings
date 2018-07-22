using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightCity.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/City Area")]
    public class CityArea : MonoBehaviour
    {
        [SerializeField]
        private float maxWidth = 1000f;
        [SerializeField]
        private float maxHeight = 1000f;
        
    }
}
