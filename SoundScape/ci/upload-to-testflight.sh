#!/usr/bin/env bash
set -e

IPA_PATH="$2/build.ipa"
echo "🔍 IPA path: $IPA_PATH"

xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete!"
