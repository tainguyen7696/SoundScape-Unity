#!/usr/bin/env bash
set -euo pipefail

# 1️⃣ Where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ Your artifact directory is the second argument
ARTIFACT_DIR="$2"
echo "🔍 Artifact directory: $ARTIFACT_DIR"

# 3️⃣ First, look for an existing .ipa
IPA_CANDIDATES=( "$ARTIFACT_DIR"/*.ipa )
if [ ${#IPA_CANDIDATES[@]} -gt 0 ] && [ -f "${IPA_CANDIDATES[0]}" ]; then
  IPA_PATH="${IPA_CANDIDATES[0]}"
  echo "🔍 Found IPA: $IPA_PATH"
else
  echo "⚠️  No .ipa found; looking for .app bundle..."

  # 4️⃣ If no .ipa, find a .app and zip it into an .ipa
  APP_CANDIDATES=( "$ARTIFACT_DIR"/*.app )
  if [ ${#APP_CANDIDATES[@]} -gt 0 ] && [ -d "${APP_CANDIDATES[0]}" ]; then
    APP_NAME="$(basename "${APP_CANDIDATES[0]}" .app)"
    IPA_PATH="$ARTIFACT_DIR/$APP_NAME.ipa"
    echo "🔍 Found .app: ${APP_CANDIDATES[0]}"
    echo "🔄 Zipping .app → $IPA_PATH"
    (cd "$ARTIFACT_DIR" && zip -r "$APP_NAME.ipa" "$APP_NAME.app" >/dev/null)
  else
    echo "❌ Neither .ipa nor .app found in $ARTIFACT_DIR"
    exit 1
  fi
fi

# 5️⃣ Finally, upload the IPA to TestFlight
echo "🚀 Uploading $IPA_PATH to TestFlight…"
xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -t ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete!"
