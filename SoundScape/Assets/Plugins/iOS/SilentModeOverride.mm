//
//  SilentModeOverride.m
//  SoundScape
//
//  Forces audio to play even when the device is in silent/mute mode.
//  Place this file under Assets/Plugins/iOS/ (or Libraries/Plugins/iOS/) in your Unity project.
//

#import <AVFoundation/AVFoundation.h>

__attribute__((constructor))
static void InitAudioSession() {
    AVAudioSession *session = [AVAudioSession sharedInstance];
    NSError *error = nil;

    // 1️⃣ Set the category to Playback so it ignores the silent switch
    [session setCategory:AVAudioSessionCategoryPlayback
                   mode:AVAudioSessionModeDefault
                options:AVAudioSessionCategoryOptionMixWithOthers
                  error:&error];
    if (error) {
        NSLog(@"[SilentModeOverride] Error setting AVAudioSession category: %@", error);
    }

    // 2️⃣ Activate the session
    [session setActive:YES error:&error];
    if (error) {
        NSLog(@"[SilentModeOverride] Error activating AVAudioSession: %@", error);
    }

    NSLog(@"[SilentModeOverride] Audio session overridden to Playback");
}
