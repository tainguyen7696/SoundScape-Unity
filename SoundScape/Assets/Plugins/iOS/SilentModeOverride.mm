#import <AVFoundation/AVFoundation.h>

// Make sure this is extern "C" so the symbol name matches exactly
extern "C" void SilentModeOverride_Awake()
{
    NSError *error = nil;

    // Override silent switch — use Playback category
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback
                                     withOptions:0
                                           error:&error];
    if (error) {
        NSLog(@"⚠️ Audio category error: %@", error);
    }

    // Activate session
    [[AVAudioSession sharedInstance] setActive:YES error:&error];
    if (error) {
        NSLog(@"⚠️ Audio activation error: %@", error);
    }
}
