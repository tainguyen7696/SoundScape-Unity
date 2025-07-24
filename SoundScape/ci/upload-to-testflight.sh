#!/usr/bin/env bash
set -e

echo "🔍 Working directory: $(pwd)"

# 1️⃣ Figure out where this script lives, regardless of CWD
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ Look for the .p8 right next to the script
KEY_PATH="$SCRIPT_DIR/AuthKey_999VD2N739.p8"
echo "🔍 Looking for API key at $KEY_PATH"
if [ ! -f "$KEY_PATH" ]; then
  echo "❌  Cannot find API key at $KEY_PATH"
  exit 1
fi

# 3️⃣ Locate the IPA that Unity Cloud Build produced
#    UNITY_CLOUD_BUILD_TARGET should already be set in your env
IPA_PATH="$WORKSPACE/.build/last/$UNITY_CLOUD_BUILD_TARGET/build.ipa"
echo "🔍 IPA path: $IPA_PATH"
if [ ! -f "$IPA_PATH" ]; then
  echo "❌  Cannot find .ipa at $IPA_PATH"
  exit 1
fi

# 4️⃣ Upload via Apple's altool
xcrun altool --upload-app \
  --type ios \
  --file "$IPA_PATH" \
  --apiKey "$ASC_API_KEY_ID" \
  --apiIssuer "$ASC_API_KEY_ISSUER_ID"

echo "✅  .ipa successfully uploaded to TestFlight!"
