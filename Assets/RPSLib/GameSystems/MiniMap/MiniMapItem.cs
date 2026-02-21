/// ------------------------------
/// Original Author: Matthew Vale
/// ------------------------------

using UnityEngine;

namespace RPSCore
{

    public class MiniMapItem : MonoBehaviour
    {
        private Transform _iconTransform;
        private Vector2 _targetRotation;

        public float TargetDirection = 90f;
        public Sprite MiniMapIcon;
        public float IconScale = 1f;


        private void Awake()
        {
            _iconTransform = transform;
            GetComponent<SpriteRenderer>().sprite = MiniMapIcon;
            _iconTransform.localScale = _iconTransform.parent.localScale * IconScale;
        }

        private void LateUpdate()
        {
            _targetRotation = _iconTransform.eulerAngles;
            _targetRotation.x = TargetDirection;
            _iconTransform.eulerAngles = _targetRotation;
        }

    }
}