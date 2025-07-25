#import <AVFoundation/AVFoundation.h>

using UnityEditor.PackageManager;
/// Called from Unity to flip the silent‑switch override.
void _OverrideAudioSessionToPlayback()
{
    AVAudioSession* session = [AVAudioSession sharedInstance];

    // Set category to Playback so audio plays even in silent mode
    NSError* categoryError = nil;
    [session setCategory:AVAudioSessionCategoryPlayback
             withOptions:0
                   error: &categoryError];

if (categoryError)
{
    NSLog(@"Error setting AVAudioSession category: %@", categoryError);
}

// Activate the session
NSError* activationError = nil;
[session setActive:YES error:&activationError] ;
if (activationError)
{
    NSLog(@"Error activating AVAudioSession: %@", activationError);
}
}
