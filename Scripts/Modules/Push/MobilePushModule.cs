using System;

namespace TinyMVC.Modules.Push {
    public abstract class MobilePushModule : IApplicationModule {
        protected readonly PushParameters _parameters;
        
        protected MobilePushModule() => _parameters = PushParameters.LoadFromResources();
        
        public bool TryGetNotification(string key, out PushParameters.NotificationData notification) => _parameters.TryGetNotification(key, out notification);
        
        public abstract void Send(string key, string text, DateTime date, string smallIconName, string largeIconName, string channelId);
        
        public abstract void CancelAll();
        
        public abstract void DeleteChannel(string id);
        
        public abstract void CreateChannel(string id, string name, string description);
    }
}