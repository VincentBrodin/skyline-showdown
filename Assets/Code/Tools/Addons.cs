using UnityEngine;

namespace Code.Tools{
    public static class Addons{
        public static Vector3 GetNormal(this Transform transform, Normal normal){
            Vector3 up = transform.up;
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            return normal switch
            {
                Normal.Up => up,
                Normal.Down => -up,
                Normal.Left => -right,
                Normal.Right => right,
                Normal.Forward => forward,
                Normal.Back => -forward,
                _ => Vector3.zero
            };
        }

    }
}