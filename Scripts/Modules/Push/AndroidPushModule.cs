#if UNITY_ANDROID && UNITY_MOBILE_NOTIFICATIONS

using System;
using System.Collections.Generic;
using Unity.Notifications.Android;

#if I2_LOCALIZE
using I2.Loc;
#endif

namespace TinyMVC.Modules.Push {
    public sealed class AndroidPushModule : MobilePushModule {
        public AndroidNotification Create(string key, string text, DateTime date, string smallIconName, string largeIconName) {
            AndroidNotification notification = new AndroidNotification();
            
            #if I2_LOCALIZE
            if (API<AndroidPushModule>.module.TryGetNotification(key, out PushParameters.NotificationData data)) {
                text = LocalizationManager.GetTranslation(data.term);
            }
            #endif
            
            notification.Title = _parameters.appTitle;
            notification.Text = text;
            notification.FireTime = date;
            notification.SmallIcon = smallIconName;
            notification.LargeIcon = largeIconName;
            
            return notification;
        }
        
        public override void Send(string key, string text, DateTime date, string smallIconName, string largeIconName, string channelId) {
            Send(Create(key, text, date, smallIconName, largeIconName), channelId);
        }
        
        public void Send(AndroidNotification[] notifications, string channelId) {
            for (int i = 0; i < notifications.Length; i++) {
                Send(notifications[i], channelId);
            }
        }
        
        public void Send(List<AndroidNotification> notifications, string channelId) {
            for (int i = 0; i < notifications.Count; i++) {
                Send(notifications[i], channelId);
            }
        }
        
        public void Send<T>(Dictionary<T, AndroidNotification> notifications, string channelId) {
            foreach (AndroidNotification notification in notifications.Values) {
                Send(notification, channelId);
            }
        }
        
        public void Send(AndroidNotification notification, string channelId) {
            if (notification.FireTime < DateTime.Now) {
                return;
            }
            
            AndroidNotificationCenter.SendNotification(notification, channelId);
        }
        
        public override void CancelAll() => AndroidNotificationCenter.CancelAllNotifications();
        
        public override void DeleteChannel(string id) => AndroidNotificationCenter.DeleteNotificationChannel(id);
        
        public override void CreateChannel(string id, string name, string description) {
            AndroidNotificationChannel channel = new AndroidNotificationChannel();
            
            channel.Id = id;
            channel.Name = name;
            channel.Importance = Importance.High;
            channel.Description = description;
            
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
    }
}

#endif