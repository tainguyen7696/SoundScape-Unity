#!/usr/bin/env bash
set -e

echo "🔍 Working directory: $(pwd)"

# 1️⃣ Find where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ Locate the API key right next to the script
KEY_PATH="$SCRIPT_DIR/AuthKey_999VD2N739.p8"
echo "🔍 Looking for API key at $KEY_PATH"
if [ ! -f "$KEY_PATH" ]; then
  echo "❌ Cannot find API key at $KEY_PATH"
  exit 1
fi

# 3️⃣ Grab the artifact directory passed in ($2) and point at build.ipa
#    Cloud Build invokes: upload-to-testflight.sh <checkout> <artifact_dir> <platform>
ARTIFACT_DIR="$2"
echo "🔍 Artifact directory: $ARTIFACT_DIR"
IPA_PATH="$ARTIFACT_DIR/build.ipa"
echo "🔍 IPA path: $IPA_PATH"
if [ ! -f "$IPA_PATH" ]; then
  echo "❌ Cannot find .ipa at $IPA_PATH"
  exit 1
fi

# 4️⃣ Upload with Apple's CLI
xcrun altool --upload-app \
  --type ios \
  --file "$IPA_PATH" \
  --apiKey "$ASC_API_KEY_ID" \
  --apiIssuer "$ASC_API_KEY_ISSUER_ID"

echo "✅ .ipa successfully uploaded to TestFlight!"
