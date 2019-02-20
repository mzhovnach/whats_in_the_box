#import "iOSNativeReview.h"
#import <StoreKit/StoreKit.h>

@implementation iOSNativeReview {
}

# pragma mark - C API

bool requestReview() {
    if (@available(iOS 10.3, *)) {
        [SKStoreReviewController requestReview];
        return true;
    } else {
    	return false;
    }
}
@end
