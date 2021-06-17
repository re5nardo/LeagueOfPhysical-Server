using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace Entity
{
    public class PatrolObject : MapObjectBase
    {
        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 halfwayPoint;
        [SerializeField] float speed = 1;

        public override float MovementSpeed => speed;

        public Vector3 GetPositionByTick(int tick)
        {
            var halfMagnitude = (halfwayPoint - startPoint).magnitude;
            var distance = MovementSpeed * Game.Current.GameTime;

            if ((int)(distance / halfMagnitude) % 2 == 0)
            {
                return Vector3.Lerp(startPoint, halfwayPoint, (distance % halfMagnitude) / halfMagnitude);
            }
            else
            {
                return Vector3.Lerp(halfwayPoint, startPoint, (distance % halfMagnitude) / halfMagnitude);
            }
        }
       
        public override void OnTick(int tick)
        {
            base.OnTick(tick);

            Position = GetPositionByTick(tick);
        }
    }
}
