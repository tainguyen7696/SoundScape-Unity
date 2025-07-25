#!/usr/bin/env bash
set -euo pipefail

# 1️⃣ Where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ The third argument is your artifact directory
ARTIFACT_DIR="${2:-}"  # fallback in case it's ever missing
# Actually Cloud Build passes:
#   $1 = temp dir, $2 = artifact dir, $3 = platform (ios)
# but some docs say $2 is temp. To be safe, detect the build.ipa location:
if [ -f "$2/build.ipa" ]; then
  ARTIFACT_DIR="$2"
elif [ -f "$3/build.ipa" ]; then
  ARTIFACT_DIR="$3"
else
  echo "❌ Could not find build.ipa in either $2 or $3"
  exit 1
fi
echo "🔍 Artifact directory: $ARTIFACT_DIR"

# 3️⃣ Point at the IPA
IPA_PATH="$ARTIFACT_DIR/build.ipa"
echo "🔍 IPA path: $IPA_PATH"
if [ ! -f "$IPA_PATH" ]; then
  echo "❌ IPA not found at $IPA_PATH"
  exit 1
fi

# 4️⃣ Upload to App Store Connect via altool
xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -t ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete – your build should now appear in TestFlight!"
