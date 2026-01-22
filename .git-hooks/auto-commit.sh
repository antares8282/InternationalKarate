#!/bin/bash

# Automatic Git Backup Script for Unity Project
# This script commits and pushes changes every hour to prevent data loss

PROJECT_DIR="/Users/gokhan/Downloads/UnityProjects/InternationalKarate"
LOG_FILE="$PROJECT_DIR/.git-hooks/auto-commit.log"

# Function to log messages
log_message() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" >> "$LOG_FILE"
}

# Navigate to project directory
cd "$PROJECT_DIR" || exit 1

# Check if there are any changes
if [[ -n $(git status -s) ]]; then
    log_message "Changes detected, creating automatic backup..."

    # Stage all changes (excluding .claude settings and crash dumps)
    git add Assets/ ProjectSettings/ Packages/

    # Create commit with timestamp
    TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')
    git commit -m "Auto-backup: $TIMESTAMP" -m "Automatic hourly backup to prevent data loss" >> "$LOG_FILE" 2>&1

    # Push to remote
    if git push origin main >> "$LOG_FILE" 2>&1; then
        log_message "✓ Backup successful"
    else
        log_message "✗ Push failed - check internet connection"
    fi
else
    log_message "No changes to backup"
fi
