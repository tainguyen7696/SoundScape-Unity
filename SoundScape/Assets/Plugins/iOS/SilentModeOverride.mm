#import <AVFoundation/AVFoundation.h>

// Forces audio to play even when the device is in silent/mute mode.
// Place this file under Assets/Plugins/iOS/ (or Libraries/Plugins/iOS/) in your Unity project.

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

#ifdef __cplusplus
extern "C" {
#endif

// Export the IL2CPP‑generated symbol so the native call resolves correctly
__attribute__((visibility("default")))
void SilentModeOverride_Awake_m6C0A14C279D27D3596B13FF949A67057450594E2() {
    // Ensure audio session override runs when Unity calls Awake
    InitAudioSession();
}

#ifdef __cplusplus
}
#endif
