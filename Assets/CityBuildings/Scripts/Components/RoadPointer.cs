using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuildings.Components
{
    using Creators;
    using Structs;
    using Utilities;

    using Random = UnityEngine.Random;
    using PointerDic = Dictionary<int, KeyValuePair<Vector2, List<int>>>;

    public static class RoadPointer
    {
        private static PointerDic Points = new PointerDic();
        private static int Id = 0;


        public static int AddPoint(Vector2 point, int roadID)
        {
            foreach(var p in Points)
            {
                if(p.Value.Key == point)
                {
                    p.Value.Value.Add(roadID);
                    return p.Key;
                }
            }

            Points.Add(Id++, new KeyValuePair<Vector2, List<int>>(point, new List<int>() { roadID }));
            return Id - 1;
        }
        
        public static void AddRoadID(int id, int roadID)
        {
            if(Points.ContainsKey(id) == false)
            {
                return;
            }

            Points[id].Value.Add(roadID);
        }

        public static List<int> GetRoadsID(int id)
        {
            if(Points.ContainsKey(id) == false)
            {
                return new List<int>();
            }

            return Points[id].Value;
        }

        public static Vector2 GetPoint(int id)
        {
            return Points.ContainsKey(id) == true ? Points[id].Key : Vector2.zero;
        }

        public static int GetNextRoadID(Road road, int pointID, CityArea city, float straightRate)
        {
            var ids = new List<int>(GetRoadsID(pointID));

            for(var i = 0; i < ids.Count; i++)
            {
                var id = ids[i];
                if(id == road.Id)
                {
                    ids.Remove(road.Id);
                    i--;
                    continue;
                }

                var d = Vector2.Dot(road.Direction, city.Roads[id].Direction);
                if(Mathf.Abs(d - 1f) <= Vector2.kEpsilon && Random.value <= straightRate)
                {
                    return id;
                }
            }

            return ids[Random.Range(0, ids.Count)];
        }
    }
}
