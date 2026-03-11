#!/bin/bash
# Deployment script for BookRepositoryApi

set -e

# Configuration
ENVIRONMENT=${1:-production}
AWS_REGION=${AWS_REGION:-us-east-1}
PROJECT_NAME="book-repository-api"

echo "=== BookRepositoryApi Deployment Script ==="
echo "Environment: $ENVIRONMENT"
echo "Region: $AWS_REGION"
echo ""

# Check if required tools are installed
command -v aws >/dev/null 2>&1 || { echo "AWS CLI is required but not installed. Aborting." >&2; exit 1; }
command -v docker >/dev/null 2>&1 || { echo "Docker is required but not installed. Aborting." >&2; exit 1; }
command -v terraform >/dev/null 2>&1 || { echo "Terraform is required but not installed. Aborting." >&2; exit 1; }

# Step 1: Initialize and apply Terraform
echo "Step 1: Deploying infrastructure with Terraform..."
cd terraform

if [ ! -d ".terraform" ]; then
    echo "Initializing Terraform..."
    terraform init
fi

echo "Planning Terraform changes..."
terraform plan -var-file=environments/${ENVIRONMENT}.tfvars -out=tfplan

echo "Applying Terraform changes..."
terraform apply tfplan

# Get outputs
ECR_REPO=$(terraform output -raw ecr_repository_url)
ECS_CLUSTER=$(terraform output -raw ecs_cluster_name)
ECS_SERVICE=$(terraform output -raw ecs_service_name)

cd ..

echo ""
echo "Step 2: Building Docker image..."
docker build -t ${PROJECT_NAME}:latest .

# Step 3: Push to ECR
echo ""
echo "Step 3: Pushing image to ECR..."
aws ecr get-login-password --region ${AWS_REGION} | docker login --username AWS --password-stdin ${ECR_REPO}

docker tag ${PROJECT_NAME}:latest ${ECR_REPO}:latest
docker tag ${PROJECT_NAME}:latest ${ECR_REPO}:$(git rev-parse --short HEAD 2>/dev/null || echo "manual")

docker push ${ECR_REPO}:latest
docker push ${ECR_REPO}:$(git rev-parse --short HEAD 2>/dev/null || echo "manual")

# Step 4: Update ECS service
echo ""
echo "Step 4: Updating ECS service..."
aws ecs update-service \
    --cluster ${ECS_CLUSTER} \
    --service ${ECS_SERVICE} \
    --force-new-deployment \
    --region ${AWS_REGION}

echo ""
echo "=== Deployment Complete ==="
echo "ECR Repository: ${ECR_REPO}"
echo "ECS Cluster: ${ECS_CLUSTER}"
echo "ECS Service: ${ECS_SERVICE}"
echo ""
echo "Monitor deployment status:"
echo "aws ecs describe-services --cluster ${ECS_CLUSTER} --services ${ECS_SERVICE} --region ${AWS_REGION}"
