using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NightCity.Creators
{
    using Components;
    using LineStructures;
    using Structs;
    using Utilities;

    [Serializable]
    public class CityArea
    {
        public List<Section> Sections { get; } = new List<Section>();
        public List<Road> MainRoads => this.mainRoad.Roads;
        public List<Rect> Rects => this.mainRoad.Rects;

        [SerializeField]
        private Vector2 field = new Vector2(1000f, 1000f);
        [SerializeField]
        private Vector2 sectionX = new Vector2(30f, 60f);
        [SerializeField]
        private Vector2 sectionY = new Vector2(30f, 60f);

        [Header("About Road"), Space]
        [SerializeField]
        private MainRoadroller mainRoad = new MainRoadroller();
        [SerializeField]
        private float subRoadWidth = 4f;
        [SerializeField]
        private float interval = 1f;


        public void CreateAreas()
        {
            var section = new Vector4(this.sectionX.x, this.sectionX.y, this.sectionY.x, this.sectionY.y);
            this.mainRoad.Create(this.field, section, this.interval);
        }
    }
}
