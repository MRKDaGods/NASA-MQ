using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI.Extensions;
using System.Collections;

namespace MRK.UI
{
    public class UICameraSelector : MonoBehaviour
    {
        private bool _shown;
        private CanvasGroup _canvasGroup;
        private float _lastInteractionTime;
        [SerializeField]
        private CanvasGroup _selectorCanvasGroup;
        [SerializeField]
        private ScrollSnap _scrollSnap;

        IEnumerator Start()
        {
            _shown = false;
            _canvasGroup = GetComponent<CanvasGroup>();

            UpdateCanvasGroupState();

            _scrollSnap.onPageChange += OnScrollPageChanged;

            _scrollSnap.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            _scrollSnap.gameObject.SetActive(true);
        }

        private void OnScrollPageChanged(int page)
        {
            Main.Instance.UpdateCameraBehaviour(page);
        }

        public void OnPointerDown(BaseEventData data)
        {
            UpdateInteraction();
        }

        public void OnPointerEnter(BaseEventData data)
        {
            UpdateInteraction();
        }

        private void UpdateInteraction()
        {
            if (!_shown)
            {
                _shown = true;
                _lastInteractionTime = Time.time;

                UpdateCanvasGroupState();
            }
        }

        private void UpdateCanvasGroupState()
        {
            DOTween.To(
                () => _canvasGroup.alpha,
                x => _canvasGroup.alpha = x,
                _shown ? 1f : 0.1f,
                0.5f);

            DOTween.To(
                () => _selectorCanvasGroup.alpha,
                x => _selectorCanvasGroup.alpha = x,
                _shown ? 1f : 0f,
                0.3f);

            var ncv = _scrollSnap.NextButton.GetComponent<CanvasGroup>();
            var pcv = _scrollSnap.PrevButton.GetComponent<CanvasGroup>();
            DOTween.To(
                () => ncv.alpha,
                x => ncv.alpha = x,
                _shown ? 1f : 0f,
                0.3f);

            DOTween.To(
                () => pcv.alpha,
                x => pcv.alpha = x,
                _shown ? 1f : 0f,
                0.3f);
        }

        private void Update()
        {
            if (_shown && Time.time - _lastInteractionTime >= 3f)
            {
                _shown = false;
                UpdateCanvasGroupState();
            }
        }
    }
}
