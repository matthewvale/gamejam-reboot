/* Author:		Matthew Vale
 * Role:		Lead Game Developer
 * Company:		Red Phoenix Studios
*/
using UnityEngine;
using UnityEngine.UI;

namespace RPSCore {

    public class ImageStyleBinding : UIStyleBindingBase {

        #region Private Properties
        private Image _image;
        private UIImageStyleSheet _styleSheet;
        public bool AlphaOverride;
        [Range(0f, 255f)] public float Alpha = 255f;
        #endregion


        #region Public Methods
        public override void ApplyStyling() {
            if (_image == null) {
                if (TryGetComponent(out Image btn)) {
                    _image = btn;
                } else {
                    RPSLib.Debug.Log("ImageStyleBinding :: No Image component found on GameObject: " + gameObject.name, RPSLib.Debug.Style.Warning);
                    return;
                }
            }

            _styleSheet = styleSheet as UIImageStyleSheet;

            if (_styleSheet == null) {
                RPSLib.Debug.Log("[" + name + "] Failed to get Style Sheet of type: UIImageStyleSheet on: " + gameObject.name, RPSLib.Debug.Style.Warning);
                return;
            }

            _image.sprite = _styleSheet.PrimaryImage;
            _image.color = _styleSheet.PrimaryColor;

            if (AlphaOverride)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Alpha / 255f);
            }
        }
        #endregion

    }

}