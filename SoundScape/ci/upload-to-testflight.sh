#!/usr/bin/env bash
set -euo pipefail

# 1️⃣ Locate where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ The second argument is your artifact directory
ARTIFACT_DIR="$2"
echo "🔍 Artifact directory: $ARTIFACT_DIR"

# 3️⃣ Point at the IPA inside your artifact dir
IPA_PATH="$ARTIFACT_DIR/build.ipa"
echo "🔍 IPA path: $IPA_PATH"
if [ ! -f "$IPA_PATH" ]; then
  echo "❌ IPA not found at $IPA_PATH"
  exit 1
fi

# 4️⃣ Upload to App Store Connect (TestFlight)
#    Make sure APPLE_ID and APP_SPECIFIC_PASSWORD are set in your Cloud Build env!
xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -t ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete – your build should now appear in TestFlight!"
