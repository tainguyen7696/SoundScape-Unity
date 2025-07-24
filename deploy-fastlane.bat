@echo off
REM ────────────────────────────────────────────────────────
REM deploy-fastlane.bat
REM ────────────────────────────────────────────────────────
REM 1) Change to the script’s directory
cd /d "%~dp0"

REM 2) Set environment variable to skip Fastlane update check
set FASTLANE_SKIP_UPDATE_CHECK=1

REM 3) Path to your IPA (adjust if needed)
set IPA_PATH=MyApp.ipa

REM 4) Run Fastlane
if exist "bin\fastlane.bat" (
    echo ▶ Using binstub…
    call bin\fastlane.bat ios beta ipa:"%IPA_PATH%"
) else (
    echo ▶ Using Bundler…
    call bundle exec fastlane ios beta ipa:"%IPA_PATH%"
)

REM 5) Pause so you can see any output/errors
pause
