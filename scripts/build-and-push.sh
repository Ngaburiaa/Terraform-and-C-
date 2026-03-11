#!/bin/bash
# Build and push Docker image to ECR

set -e

AWS_REGION=${AWS_REGION:-us-east-1}
PROJECT_NAME="book-repository-api"
IMAGE_TAG=${1:-latest}

echo "=== Building and Pushing Docker Image ==="
echo "Tag: $IMAGE_TAG"
echo ""

# Get ECR repository URL from Terraform output
cd terraform
ECR_REPO=$(terraform output -raw ecr_repository_url 2>/dev/null)

if [ -z "$ECR_REPO" ]; then
    echo "Error: Could not get ECR repository URL from Terraform outputs."
    echo "Make sure Terraform has been applied successfully."
    exit 1
fi

cd ..

echo "ECR Repository: $ECR_REPO"
echo ""

# Build Docker image
echo "Building Docker image..."
docker build -t ${PROJECT_NAME}:${IMAGE_TAG} .

# Login to ECR
echo ""
echo "Logging in to ECR..."
aws ecr get-login-password --region ${AWS_REGION} | docker login --username AWS --password-stdin ${ECR_REPO}

# Tag and push
echo ""
echo "Tagging and pushing image..."
docker tag ${PROJECT_NAME}:${IMAGE_TAG} ${ECR_REPO}:${IMAGE_TAG}
docker push ${ECR_REPO}:${IMAGE_TAG}

# Also tag as latest if not already
if [ "$IMAGE_TAG" != "latest" ]; then
    docker tag ${PROJECT_NAME}:${IMAGE_TAG} ${ECR_REPO}:latest
    docker push ${ECR_REPO}:latest
fi

echo ""
echo "=== Image pushed successfully ==="
echo "Image: ${ECR_REPO}:${IMAGE_TAG}"
