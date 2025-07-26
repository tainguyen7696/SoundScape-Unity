// AudioBackground.mm
#import <AVFoundation/AVFoundation.h>
#import <Foundation/Foundation.h>

extern "C" {
    void _SetAudioSessionBackground()
    {
        NSLog(@"[AudioSession] → Setting background audio");
        AVAudioSession *session = [AVAudioSession sharedInstance];
        NSError *error = nil;

        // playback category allows background audio + mixes with others
        [session setCategory:AVAudioSessionCategoryPlayback
                 withOptions:AVAudioSessionCategoryOptionMixWithOthers
                       error:&error];
        if (error) {
            NSLog(@"[AudioSession] ⚠️ setCategory error: %@", error);
        }

        [session setActive:YES error:&error];
        if (error) {
            NSLog(@"[AudioSession] ⚠️ setActive error: %@", error);
        }
    }
}
