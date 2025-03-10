#!/bin/bash

# Strict mode for better error handling
set -euo pipefail

# Variables
RESOURCE_GROUP="book-management-rg"
LOCATION="eastus"
APP_NAME="book-management-api"
SUBSCRIPTION_ID="bb3c97ff-11b5-4b37-92cf-1897b2d8766b"

# Generate secure values if not provided
generate_jwt_secret() {
    openssl rand -base64 32
}

# Use environment variables or defaults
DB_CONNECTION_STRING="${DB_CONNECTION_STRING:-sqlite_connection_string}"
JWT_SECRET_KEY="${JWT_SECRET_KEY:-$(generate_jwt_secret)}"

# Logging function
log() {
    echo "[$(date +'%Y-%m-%d %H:%M:%S')] $*"
}

# Check Azure CLI is installed
if ! command -v az &> /dev/null; then
    log "Azure CLI is not installed. Please install it first."
    exit 1
fi

# Ensure logged into correct subscription
log "Setting active subscription"
az account set --subscription "$SUBSCRIPTION_ID"

# Create Resource Group
log "Creating Resource Group: $RESOURCE_GROUP"
az group create \
    --name "$RESOURCE_GROUP" \
    --location "$LOCATION"

# Create App Service Plan
log "Creating App Service Plan"
az appservice plan create \
    --name "${APP_NAME}-plan" \
    --resource-group "$RESOURCE_GROUP" \
    --sku B1 \
    --is-linux

# Create Web App
log "Creating Web App"
az webapp create \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --plan "${APP_NAME}-plan" \
    --runtime "DOTNET|8.0"

# Configure App Settings
log "Configuring App Settings"
az webapp config appsettings set \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --settings \
        ConnectionStrings__DefaultConnection="$DB_CONNECTION_STRING" \
        JWT__SecretKey="$JWT_SECRET_KEY" \
        ASPNETCORE_ENVIRONMENT="Production"

# Enable logging
log "Enabling Web App Logging"
az webapp log config \
    --name "$APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --web-server-logging filesystem

log "Deployment completed successfully!"