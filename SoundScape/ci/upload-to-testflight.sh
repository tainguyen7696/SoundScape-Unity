#!/usr/bin/env bash
set -euo pipefail

# locate this script’s dir
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# args: $1 = temp dir (unused), $2 = artifact dir, $3 = platform (ios)
ARTIFACT_DIR="$2"
PLATFORM="${3:-ios}"
echo "🔍 Artifact directory: $ARTIFACT_DIR"
echo "🔍 Target platform: $PLATFORM"

# 1) try to find an .ipa anywhere under ARTIFACT_DIR
IPA_PATH="$(find "$ARTIFACT_DIR" -type f -name '*.ipa' | head -n 1 || true)"
if [[ -n "$IPA_PATH" ]]; then
  echo "🔍 Found IPA: $IPA_PATH"
else
  echo "⚠️  No .ipa found; looking for .app bundle..."
  # 2) find a .app bundle
  APP_PATH="$(find "$ARTIFACT_DIR" -type d -name '*.app' | head -n 1 || true)"
  if [[ -n "$APP_PATH" ]]; then
    APP_NAME="$(basename "$APP_PATH" .app)"
    IPA_PATH="$ARTIFACT_DIR/$APP_NAME.ipa"
    echo "🔍 Found .app: $APP_PATH"
    echo "🔄 Zipping .app → $IPA_PATH"
    # zip from within the .app’s parent so the folder structure is correct
    (cd "$(dirname "$APP_PATH")" && zip -r "$IPA_PATH" "$(basename "$APP_PATH")" >/dev/null)
  else
    echo "❌ Neither .ipa nor .app found in $ARTIFACT_DIR"
    exit 1
  fi
fi

# 3) upload the IPA
echo "🚀 Uploading $IPA_PATH to TestFlight…"
xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -t "$PLATFORM" \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete!"
