using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NightCity.Components
{
    using Creators;
    using Structs;

    [DisallowMultipleComponent]
    [AddComponentMenu("Night City/Components/Car Render")]
    public class CarRender : MonoBehaviour
    {
        public void Init(Skyscraper skyscraper)
        {
            var roads = skyscraper.CityArea.Roads;


        }
    }
}
