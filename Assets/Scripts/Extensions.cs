using UnityEditor;
using UnityEngine;

namespace MRK
{
    public static class Extensions
    {
        public static Rect WorldRect(this RectTransform rectTransform)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }

        public static Rect RectTransformToScreenSpace(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }

        public static Vector3 ToVector3xz(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }

        public static Vector3 ToVector3xz(this Vector2d v)
        {
            return new Vector3((float)v.x, 0, (float)v.y);
        }

        public static Vector2 ToVector2xz(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2d ToVector2d(this Vector3 v)
        {
            return new Vector2d(v.x, v.z);
        }

        public static Vector3 Perpendicular(this Vector3 v)
        {
            return new Vector3(-v.z, v.y, v.x);
        }

        public static void MoveToGeocoordinate(this Transform t, double lat, double lng, Vector2d refPoint, float scale = 1)
        {
            t.position = MapUtils.GeoToWorldPosition(lat, lng, refPoint, scale).ToVector3xz();
        }

        public static void MoveToGeocoordinate(this Transform t, Vector2d latLon, Vector2d refPoint, float scale = 1)
        {
            t.MoveToGeocoordinate(latLon.x, latLon.y, refPoint, scale);
        }

        public static Vector3 AsUnityPosition(this Vector2 latLon, Vector2d refPoint, float scale = 1)
        {
            return MapUtils.GeoToWorldPosition(latLon.x, latLon.y, refPoint, scale).ToVector3xz();
        }

        public static Vector2d GetGeoPosition(this Transform t, Vector2d refPoint, float scale = 1)
        {
            var pos = refPoint + (t.position / scale).ToVector2d();
            return MapUtils.MetersToLatLon(pos);
        }

        public static Vector2d GetGeoPosition(this Vector3 position, Vector2d refPoint, float scale = 1)
        {
            var pos = refPoint + (position / scale).ToVector2d();
            return MapUtils.MetersToLatLon(pos);
        }

        public static Vector2d GetGeoPosition(this Vector2 position, Vector2d refPoint, float scale = 1)
        {
            return position.ToVector3xz().GetGeoPosition(refPoint, scale);
        }
    }
}
