#!/usr/bin/env bash
set -e

echo "🏗️  Build complete. Uploading .ipa to TestFlight…"

# 1️⃣ Locate the .ipa (Cloud Build places it here)
IPA_PATH="$WORKSPACE/.build/last/$UNITY_CLOUD_BUILD_TARGET/build.ipa"
echo "IPA path: $IPA_PATH"

# 2️⃣ Ensure your API key file is in place
if [ ! -f "$ASC_API_KEY_PATH" ]; then
  echo "❌  Cannot find API key at $ASC_API_KEY_PATH"
  exit 1
fi

# 3️⃣ Upload via altool (Apple’s CLI uploader) :contentReference[oaicite:1]{index=1}
xcrun altool --upload-app \
  --type ios \
  --file "$IPA_PATH" \
  --apiKey "$ASC_API_KEY_ID" \
  --apiIssuer "$ASC_API_KEY_ISSUER_ID"

echo "✅  .ipa successfully uploaded to TestFlight!"
