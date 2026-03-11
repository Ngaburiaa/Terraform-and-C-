#!/bin/bash
# Terraform initialization script

set -e

ENVIRONMENT=${1:-production}

echo "=== Initializing Terraform for $ENVIRONMENT ==="

cd terraform

# Initialize Terraform
terraform init

# Validate configuration
echo ""
echo "Validating Terraform configuration..."
terraform validate

# Format check
echo ""
echo "Checking Terraform formatting..."
terraform fmt -check -recursive || {
    echo "Formatting issues found. Running terraform fmt..."
    terraform fmt -recursive
}

echo ""
echo "=== Terraform initialized successfully ==="
echo ""
echo "Next steps:"
echo "1. Set required environment variables:"
echo "   export TF_VAR_db_username='your_username'"
echo "   export TF_VAR_db_password='your_password'"
echo "   export TF_VAR_jwt_secret_key='your_jwt_secret'"
echo ""
echo "2. Plan the deployment:"
echo "   terraform plan -var-file=environments/${ENVIRONMENT}.tfvars"
echo ""
echo "3. Apply the configuration:"
echo "   terraform apply -var-file=environments/${ENVIRONMENT}.tfvars"
