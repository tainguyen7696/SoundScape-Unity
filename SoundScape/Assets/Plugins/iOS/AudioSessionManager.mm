#import <AVFoundation/AVFoundation.h>

extern "C" {
    void _SetAudioSessionPlayback()
    {
        AVAudioSession *session = [AVAudioSession sharedInstance];
        // CategoryPlayback ignores the Ring/Silent switch
        [session setCategory:AVAudioSessionCategoryPlayback error:nil];
        [session setActive:YES error:nil];
    }
}
