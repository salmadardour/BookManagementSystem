#!/bin/bash

# Variables
RESOURCE_GROUP="book-management-rg"
LOCATION="eastus"
APP_NAME="book-management-api"
DB_CONNECTION_STRING="Data Source=books.db" # Replace with your actual connection string
JWT_SECRET_KEY="your-secure-jwt-secret-key-here" # Replace with your actual secret key

# Create Resource Group
echo "Creating Resource Group: $RESOURCE_GROUP"
az group create --name $RESOURCE_GROUP --location $LOCATION

# Deploy ARM Template
echo "Deploying Azure Resources..."
az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file azure-template.json \
  --parameters webAppName=$APP_NAME \
  --parameters databaseConnectionString=$DB_CONNECTION_STRING \
  --parameters jwtSecretKey=$JWT_SECRET_KEY

echo "Deployment completed!"