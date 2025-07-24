using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.IAP {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public abstract class ConfiscateFakeButton<T> : MonoBehaviour where T : CodelessIAPModule, new() {
    #if ODIN_INSPECTOR
        [Required]
    #endif
        [SerializeField]
        private Button _thisButton;
        
        private void Awake() {
        #if UNITY_PURCHASING_FAKE
            _thisButton.onClick.AddListener(API<T>.module.ConfiscateAllPurchased);
            gameObject.SetActive(true);
        #else
            gameObject.SetActive(false);
        #endif
        }
        
    #if UNITY_EDITOR
        
        [ContextMenu("Soft Reset")]
        private void Reset() {
            _thisButton = GetComponent<Button>();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
    #endif
    }
}