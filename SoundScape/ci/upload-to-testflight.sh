#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

ARTIFACT_DIR="$2"
echo "🔍 Looking for IPA in $ARTIFACT_DIR"

IPA_CANDIDATES=( "$ARTIFACT_DIR"/*.ipa )
if [ ${#IPA_CANDIDATES[@]} -eq 0 ]; then
  echo "❌ No .ipa found in $ARTIFACT_DIR"
  exit 1
fi

IPA_PATH="${IPA_CANDIDATES[0]}"
echo "🔍 Found IPA: $IPA_PATH"

xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  -t ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete!"
