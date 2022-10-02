using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MRK
{
    public class Main : MonoBehaviour
    {
        private float _deltaTime;
        private GUIStyle _fpsStyle;
        [SerializeField]
        private Transform _moon;
        [SerializeField]
        private Camera _camera;
        private CameraBehaviour[] _cameraBehaviours;
        private int _cameraBehaviourIndex;
        [SerializeField]
        private Toggle _dmTog, _smTog;
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Pin _pinPrefab;
        private List<Pin> _smPins;
        private List<Pin> _dmPins;
        [SerializeField]
        private CanvasGroup mInfo;
        [SerializeField]
        private TextMeshProUGUI mText;

        CameraBehaviour CurrentCameraBehaviour
        {
            get { return _cameraBehaviourIndex == -1 ? null : _cameraBehaviours[_cameraBehaviourIndex]; }
        }

        public static Main Instance
        {
            get; private set;
        }

        public bool SMOn { get => _smTog.isOn; }
        public bool DMOn { get => _dmTog.isOn; }

        public Main()
        {
            _cameraBehaviourIndex = -1;
        }

        private void Awake()
        {
            Instance = this;

            Application.targetFrameRate = -1;

            _cameraBehaviours = new CameraBehaviour[]
            {
                new FreeMoveCameraBehaviour(_moon, _camera),
                new OrbitCameraBehaviour(_moon, _camera),
                new FocusedCameraBehaviour(_moon, _camera)
            };

            Pin.Canvas = _canvas;
        }

        private void Start()
        {
            UpdateCameraBehaviour(0);

            StartCoroutine(GeneratePins());

            mInfo.gameObject.SetActive(false);
        }

        float maxMag, maxDepth;
        IEnumerator GeneratePins()
        {
            var data = Data.Instance;
            while (!data.Ready)
                yield return new WaitForSeconds(0.2f);

            _smPins = new List<Pin>();
            _dmPins = new List<Pin>();

            var shallows = data.GetShallowMoonQuakesData(out float maxMag);
            this.maxMag = maxMag;
            foreach (var sm in shallows)
            {
                var go = Instantiate(_pinPrefab.gameObject, _canvas.transform);
                var pin = go.GetComponent<Pin>();
                pin.Moon = _moon;
                pin.IsOn = () => SMOn;
                pin.Color = Color.Lerp(Color.blue, Color.green, sm.Magnitude / maxMag);
                pin.Scale = Mathf.Clamp(sm.Magnitude / maxMag, 0.5f, 1f);
                pin.Camera = _camera;
                pin.Latitude = sm.Lat;
                pin.Longitude = sm.Long;
                pin.Data = sm;
                go.SetActive(true);

                _smPins.Add(pin);
            }

            var deeps = data.GetDeepMoonQuakesData(out float maxDepth);
            this.maxDepth = maxDepth;
            foreach (var dm in deeps)
            {
                var go = Instantiate(_pinPrefab.gameObject, _canvas.transform);
                var pin = go.GetComponent<Pin>();
                pin.Moon = _moon;
                pin.IsOn = () => DMOn;
                pin.Color = Color.Lerp(Color.cyan, Color.red, dm.Depth / maxDepth);
                pin.Scale = dm.Depth / maxDepth;
                pin.Camera = _camera;
                pin.Latitude = dm.Lat;
                pin.Longitude = dm.Long;
                pin.Data = dm;
                go.SetActive(true);

                _dmPins.Add(pin);
            }
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            if (CurrentCameraBehaviour != null)
            {
                CurrentCameraBehaviour.Update();
            }
        }

        private void OnGUI()
        {
            if (_fpsStyle == null)
            {
                _fpsStyle = new GUIStyle
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 27,
                    normal = {
                        textColor = Color.yellow
                    },
                    richText = true
                };

                GUI.skin.label.richText = true;
            }

            GUI.Label(new Rect(40f, Screen.height - 50f, 100f, 50f), string.Format("<b>{0:0.0}</b> ms (<b>{1:0.}</b> fps) {2}",
                        _deltaTime * 1000f, 1f / _deltaTime, DOTween.TotalPlayingTweens()), _fpsStyle);

            if (CurrentCameraBehaviour != null)
            {
                CurrentCameraBehaviour.OnGUI();
            }
        }

        public void UpdateCameraBehaviour(int behaviourIndex)
        {
            if (_cameraBehaviourIndex == behaviourIndex) return;

            Debug.Log($"Updating cam behaviour idx={behaviourIndex}");

            //call ondisabled
            if (_cameraBehaviourIndex != -1)
            {
                CurrentCameraBehaviour.OnDisabled();
            }

            _cameraBehaviourIndex = behaviourIndex;
            CurrentCameraBehaviour.OnEnabled();
        }

        public void PinClick(Pin p)
        {
            if (!mInfo.gameObject.activeSelf)
            {
                mInfo.gameObject.SetActive(true);

                DOTween.To(
                () => mInfo.alpha,
                x => mInfo.alpha = x,
                1f,
                0.5f).ChangeStartValue(0f);
            }


            if (p.Data is SMData sm)
            {
                mText.text = $"Type: Shallow MoonQuake\n" +
                    $"Year: {sm.Year}\n" +
                    $"Day: {sm.Day}\n" +
                    $"Time: {sm.H}:{sm.M}:{sm.S}\n" +
                    $"Magnitude: {sm.Magnitude}\n\n\n" +
                    $"Latitude: {sm.Lat}\n" +
                    $"Longitude: {sm.Long}\n";

            }
            else if (p.Data is DMData dm)
            {
                mText.text = $"Type: Deep MoonQuake\n" +
                    $"ID: {dm.A}\n" +
                    $"Side: {dm.Side}\n\n" +
                    $"Latitude: {dm.Lat} +- ({dm.Lat_err})\n" +
                    $"Longitude: {dm.Long} +- ({dm.Long_err})\n\n" +
                    $"Depth: {dm.Depth} +- ({dm.Depth_err})\n" +
                    $"Assumed: {dm.Assumed}\n";

            }
        }
    }
}