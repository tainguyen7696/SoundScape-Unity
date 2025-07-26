// AudioBackground.mm
#import <AVFoundation/AVFoundation.h>

extern "C" {
    void _SetAudioSessionBackground()
    {
        AVAudioSession *session = [AVAudioSession sharedInstance];
        NSError *error = nil;

        // .playback allows background audio
        [session setCategory:AVAudioSessionCategoryPlayback
                 withOptions:0
                       error:&error];
        if (error) {
            NSLog(@"⚠️ Error setting AVAudioSession category: %@", error);
        }

        [session setActive:YES error:&error];
        if (error) {
            NSLog(@"⚠️ Error activating AVAudioSession: %@", error);
        }
    }
}
