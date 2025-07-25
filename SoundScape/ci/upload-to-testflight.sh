#!/usr/bin/env bash

echo "=== upload-to-testflight.sh version: $(date '+%Y-%m-%d %H:%M:%S') ==="
head -n1 "$0"

set -euo pipefail
IFS=$'\n\t'

WORKSPACE="$1"
ARTIFACT_DIR="${2:-}"

# 0️⃣ Only dump the gym log if it exists
LOG="$WORKSPACE/Library/Logs/gym/SoundScape-Unity-iPhone.log"
if [[ -f "$LOG" ]]; then
  echo "=== Xcode Archive Log ($LOG) ==="
  cat "$LOG"
fi

# 1️⃣ Where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "🔍 Script directory: $SCRIPT_DIR"

# 2️⃣ Artifact directory passed in by Cloud Build
if [[ -z "$ARTIFACT_DIR" ]]; then
  echo "❌ No artifact directory provided"
  exit 1
fi
echo "🔍 Artifact directory: $ARTIFACT_DIR"

# helper to zip .app → .ipa
zip_app_to_ipa() {
  local app="$1"
  local out_ipa="$2"
  echo "🔄 Zipping $app → $out_ipa"
  (cd "$(dirname "$app")" && zip -r "$out_ipa" "$(basename "$app")" >/dev/null)
}

IPA_PATH=""

# 3️⃣ Search for .ipa in ARTIFACT_DIR and its parent
for D in "$ARTIFACT_DIR" "$(dirname "$ARTIFACT_DIR")"; do
  echo "🔍 Scanning $D for .ipa"
  if [[ -d "$D" ]]; then
    IPA_PATH="$(find "$D" -type f -iname '*.ipa' -print -quit 2>/dev/null || true)"
    if [[ -n "$IPA_PATH" ]]; then
      echo "✅ Found IPA: $IPA_PATH"
      break
    fi
  fi
done

# 4️⃣ If no .ipa, look for .xcarchive → .app inside → zip it
if [[ -z "$IPA_PATH" ]]; then
  echo "⚠️  No .ipa found; searching for .xcarchive..."
  XCARCHIVE="$(find "$ARTIFACT_DIR" -type d -iname '*.xcarchive' -print -quit || true)"
  if [[ -n "$XCARCHIVE" ]]; then
    APP_IN_ARCHIVE="$(find "$XCARCHIVE/Products/Applications" -type d -iname '*.app' -print -quit || true)"
    if [[ -n "$APP_IN_ARCHIVE" ]]; then
      IPA_PATH="$ARTIFACT_DIR/$(basename "$APP_IN_ARCHIVE" .app).ipa"
      zip_app_to_ipa "$APP_IN_ARCHIVE" "$IPA_PATH"
    fi
  fi
fi

# 5️⃣ If still no .ipa, look for any .app → zip it
if [[ -z "$IPA_PATH" ]]; then
  echo "⚠️  No .app inside .xcarchive; looking for any .app..."
  APP_PATH="$(find "$ARTIFACT_DIR" -type d -iname '*.app' -print -quit || true)"
  if [[ -n "$APP_PATH" ]]; then
    IPA_PATH="$ARTIFACT_DIR/$(basename "$APP_PATH" .app).ipa"
    zip_app_to_ipa "$APP_PATH" "$IPA_PATH"
  fi
fi

# 6️⃣ Fail if we still don’t have an IPA
if [[ -z "$IPA_PATH" || ! -f "$IPA_PATH" ]]; then
  echo "❌ Could not find or produce an IPA under $ARTIFACT_DIR"
  exit 1
fi

# 7️⃣ Upload to TestFlight using Apple ID + app‑specific password
echo "🚀 Uploading $IPA_PATH to TestFlight…"
if [[ -z "${APPLE_ID:-}" || -z "${APP_SPECIFIC_PASSWORD:-}" ]]; then
  echo "❌ Missing APPLE_ID or APP_SPECIFIC_PASSWORD environment variable"
  exit 1
fi

xcrun altool \
  --upload-app \
  -f "$IPA_PATH" \
  --type ios \
  -u "$APPLE_ID" \
  -p "$APP_SPECIFIC_PASSWORD" \
  --verbose \
  --output-format xml

echo "✅ Upload complete – your build should now appear in TestFlight!"
