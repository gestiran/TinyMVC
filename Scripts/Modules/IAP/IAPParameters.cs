using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyMVC.Modules.IAP {
    [CreateAssetMenu(fileName = nameof(IAPParameters), menuName = "API/" + nameof(IAPParameters))]
    public sealed class IAPParameters : ScriptableObject {
    #if UNITY_EDITOR
        
        [field: SerializeField, Header("Debug:")]
        public string pathToProductCatalog { get; private set; } = "IAPProductCatalog";
        
        [field: SerializeField]
        public string productSeparatorId { get; private set; }
        
        [field: SerializeField]
        public bool isUsingLastPurchases { get; private set; }
        
        [field: SerializeField
            #if ODIN_INSPECTOR
              , ValueDropdown("@BuyHandler.LoadPurchasesValues()")
    #endif
        ]
        public string[] debugPurchases { get; private set; }
        
    #endif
        
        private const string _PATH = "Application/" + nameof(IAPParameters);
        
        public static IAPParameters LoadFromResources() => Resources.Load<IAPParameters>(_PATH);
    }
}