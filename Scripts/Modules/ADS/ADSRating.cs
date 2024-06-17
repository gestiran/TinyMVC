#if GOOGLE_ADS_MOBILE
using GoogleMobileAds.Api;

namespace TinyMVC.Modules.ADS {
    public sealed class ADSRating {
        public bool tagForChildDirectedTreatment { get; }
        public TagForUnderAgeOfConsent tagForUnderAgeOfConsent { get; }
        
        //public bool hasUserConsent => _hasUserConsent;
        //public bool gdprConsentMetaData => _GDPRConsentMetaData;
        //public bool isConsent => _isConsent;
        public AgeRating rating { get; }
        
        //private readonly bool _hasUserConsent;
        //private readonly bool _GDPRConsentMetaData;
        //private readonly bool _isConsent;
        
        public enum AgeRating : byte { G = 3, PG = 7, T = 12, MA = 16 }
        
        public ADSRating(byte age) {
            if (age <= (byte)AgeRating.G) {
                tagForChildDirectedTreatment = true;
                tagForUnderAgeOfConsent = TagForUnderAgeOfConsent.True;
                
                //_hasUserConsent = false;
                //_GDPRConsentMetaData = false;
                //_isConsent = false;
                rating = AgeRating.G;
            } else if (age <= (byte)AgeRating.PG) {
                tagForChildDirectedTreatment = true;
                tagForUnderAgeOfConsent = TagForUnderAgeOfConsent.True;
                
                //_hasUserConsent = false;
                //_GDPRConsentMetaData = false;
                //_isConsent = false;
                rating = AgeRating.MA;
            } else if (age <= (byte)AgeRating.T) {
                tagForChildDirectedTreatment = true;
                tagForUnderAgeOfConsent = TagForUnderAgeOfConsent.True;
                
                //_hasUserConsent = false;
                //_GDPRConsentMetaData = false;
                //_isConsent = false;
                rating = AgeRating.MA;
            } else {
                tagForChildDirectedTreatment = false;
                tagForUnderAgeOfConsent = TagForUnderAgeOfConsent.False;
                
                //_hasUserConsent = true;
                //_GDPRConsentMetaData = true;
                //_isConsent = true;
                rating = AgeRating.MA;
            }
        }
        
        public TagForChildDirectedTreatment GetTagForChildDirectedTreatment() {
            return tagForChildDirectedTreatment ? TagForChildDirectedTreatment.True : TagForChildDirectedTreatment.False;
        }
        
        public MaxAdContentRating GetRating() {
            switch (rating) {
                case AgeRating.G: return MaxAdContentRating.G;
                case AgeRating.PG: return MaxAdContentRating.PG;
                case AgeRating.T: return MaxAdContentRating.T;
                case AgeRating.MA: return MaxAdContentRating.MA;
            }
            
            return MaxAdContentRating.G;
        }
    }
}
#endif