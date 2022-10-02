using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MRK
{
    public class Pin : MonoBehaviour
    {
        Vector3 wpos;
        Vector3 spos;
        Button b;
        Image image;

        public Transform Moon { get; set; }
        public Camera Camera { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Color Color { get; set; } = Color.white;
        public float Scale { get; set; } = 1f;
        public Func<bool> IsOn { get; set; }
        public object Data { get; set; }

        public static Canvas Canvas { get; set; }

        private void Start()
        {
            Application.onBeforeRender += Application_onBeforeRender;

            var img = transform.Find("img");
            b = img.GetComponent<Button>();
            image = img.Find("real").GetComponent<Image>();

            image.color = Color;
            img.transform.localScale = new Vector3(Scale, Scale, Scale);

            b.onClick.AddListener(() =>
            {
                Main.Instance.PinClick(this);
            });

            //highlight fix
            b.interactable = false;
            b.interactable = true;
        }

        private void Application_onBeforeRender()
        {
            if (Moon == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (b.isActiveAndEnabled)
            {
                wpos = MapUtils.GeoToWorldGlobePosition(Latitude, Longitude, Moon.localScale.x) + Moon.position; //scale * geosphere
                spos = Camera.WorldToScreenPoint(wpos);
                transform.position = ScreenToMarkerSpace(spos);
            }
        }

        private void Update()
        {
            if (Moon == null)
            {
                gameObject.SetActive(false);
                return;
            }

            b.gameObject.SetActive(IsOn());

            if (b.isActiveAndEnabled)
            {
                var dir = (wpos - Moon.position).normalized;
                image.enabled = !Physics.Linecast(wpos + dir * 2f, Camera.transform.position);


                image.transform.rotation = Quaternion.LookRotation(dir);

                var scale = (1f - Mathf.Clamp01(Vector3.Distance(Camera.transform.position, wpos) / 5000f)) * Scale;

                b.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private static Vector3 ScreenToMarkerSpace(Vector2 spos)
        {
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)Canvas.transform, spos, Canvas.worldCamera, out point);
            return Canvas.transform.TransformPoint(point);
        }
    }
}
