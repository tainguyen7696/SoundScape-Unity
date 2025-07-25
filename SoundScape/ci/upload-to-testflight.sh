#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

# 1️⃣ Locate where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ The second argument is your artifact directory
ARTIFACT_DIR="${2:-}"
if [[ -z "$ARTIFACT_DIR" ]]; then
  echo "❌ No artifact directory provided"
  exit 1
fi
echo "🔍 Artifact directory: $ARTIFACT_DIR"

# helper to zip .app → .ipa
zip_app_to_ipa() {
  local app="$1"; local out_ipa="$2"
  echo "🔄 Zipping $app → $out_ipa"
  (cd "$(dirname "$app")" && zip -r "$out_ipa" "$(basename "$app")" >/dev/null)
}

IPA_PATH=""

# 3️⃣ Try to find an .ipa anywhere
IPA_PATH="$(find "$ARTIFACT_DIR" -type f -iname '*.ipa' | head -n1 || true)"
if [[ -n "$IPA_PATH" ]]; then
  echo "🔍 Found existing IPA: $IPA_PATH"
else
  echo "⚠️  No .ipa found; looking for .xcarchive..."
  # 4️⃣ Look for an .xcarchive and its .app
  XCARCHIVE="$(find "$ARTIFACT_DIR" -type d -iname '*.xcarchive' | head -n1 || true)"
  if [[ -n "$XCARCHIVE" ]]; then
    APP_IN_ARCHIVE="$(find "$XCARCHIVE/Products/Applications" -type d -iname '*.app' | head -n1 || true)"
    if [[ -n "$APP_IN_ARCHIVE" ]]; then
      IPA_PATH="$ARTIFACT_DIR/$(basename "$APP_IN_ARCHIVE" .app).ipa"
      zip_app_to_ipa "$APP_IN_ARCHIVE" "$IPA_PATH"
    fi
  fi

  # 5️⃣ If still no .ipa, look for a standalone .app
  if [[ -z "$IPA_PATH" || ! -f "$IPA_PATH" ]]; then
    echo "⚠️  No .app inside archive; looking for any .app..."
    APP_PATH="$(find "$ARTIFACT_DIR" -type d -iname '*.app' | head -n1 || true)"
    if [[ -n "$APP_PATH" ]]; then
      IPA_PATH="$ARTIFACT_DIR/$(basename "$APP_PATH" .app).ipa"
      zip_app_to_ipa "$APP_PATH" "$IPA_PATH"
    fi
  fi
fi

# 6️⃣ Fail if we still don’t have an IPA
if [[ -z "$IPA_PATH" || ! -f "$IPA_PATH" ]]; then
  echo "❌ Could not find or produce an IPA under $ARTIFACT_DIR"
  exit 1
fi

# 7️⃣ Upload to App Store Connect (TestFlight)
#    APPLE_ID and APP_SPECIFIC_PASSWORD must be set in your Cloud Build env!
echo "🚀 Uploading $IPA_PATH to TestFlight…"
xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  --type ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete – your build should now appear in TestFlight!"
