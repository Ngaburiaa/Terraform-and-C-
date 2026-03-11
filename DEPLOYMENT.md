# BookRepositoryApi - AWS Deployment Guide

This document provides comprehensive instructions for deploying the BookRepositoryApi to AWS using Terraform.

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Detailed Deployment Steps](#detailed-deployment-steps)
- [Post-Deployment](#post-deployment)
- [Monitoring and Maintenance](#monitoring-and-maintenance)
- [Troubleshooting](#troubleshooting)

## Overview

The BookRepositoryApi is deployed to AWS using:
- **Infrastructure**: Terraform (Infrastructure as Code)
- **Compute**: Amazon ECS Fargate
- **Database**: Amazon RDS PostgreSQL
- **Storage**: Amazon S3
- **CDN**: Amazon CloudFront
- **Load Balancing**: Application Load Balancer
- **Backup**: AWS Backup

## Prerequisites

### Required Software

1. **AWS CLI** (v2.x or later)
   ```bash
   aws --version
   ```

2. **Terraform** (v1.0 or later)
   ```bash
   terraform version
   ```

3. **Docker** (for building container images)
   ```bash
   docker --version
   ```

4. **Git** (for version control)
   ```bash
   git --version
   ```

### AWS Setup

1. **AWS Account** with appropriate permissions
2. **IAM User** with the following permissions:
   - EC2, ECS, RDS, S3, CloudFront, IAM, CloudWatch
   - Or use `AdministratorAccess` policy (not recommended for production)

3. **AWS CLI Configuration**
   ```bash
   aws configure
   ```
   Provide:
   - AWS Access Key ID
   - AWS Secret Access Key
   - Default region (e.g., us-east-1)
   - Default output format (json)

4. **Terraform Backend** (Pre-existing)
   - S3 Bucket: `itrack-terraform-state-prod-471744311346`
   - DynamoDB Table: `terraform-state-lock-production`

## Quick Start

For experienced users, here's the quick deployment:

```bash
# 1. Set environment variables
export TF_VAR_db_username="admin"
export TF_VAR_db_password="YourSecurePassword123!"
export TF_VAR_jwt_secret_key="YourVeryLongAndSecureJWTSecretKey123!"

# 2. Initialize Terraform
cd terraform
terraform init

# 3. Deploy infrastructure
terraform apply -var-file=environments/production.tfvars

# 4. Build and push Docker image
cd ..
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin $(terraform -chdir=terraform output -raw ecr_repository_url)
docker build -t book-repository-api:latest .
docker tag book-repository-api:latest $(terraform -chdir=terraform output -raw ecr_repository_url):latest
docker push $(terraform -chdir=terraform output -raw ecr_repository_url):latest

# 5. Deploy to ECS
aws ecs update-service \
  --cluster $(terraform -chdir=terraform output -raw ecs_cluster_name) \
  --service $(terraform -chdir=terraform output -raw ecs_service_name) \
  --force-new-deployment \
  --region us-east-1
```

## Detailed Deployment Steps

### Step 1: Prepare Environment Variables

Create a secure file to store sensitive variables (do not commit to git):

```bash
# Create .env file
cat > .env << 'EOF'
export TF_VAR_db_username="bookadmin"
export TF_VAR_db_password="ChangeThisToASecurePassword123!"
export TF_VAR_jwt_secret_key="ThisShouldBeAVeryLongAndRandomSecretKey32CharsMin!"
EOF

# Load variables
source .env
```

### Step 2: Initialize Terraform

```bash
# Navigate to terraform directory
cd terraform

# Initialize Terraform (downloads providers and modules)
terraform init

# Validate configuration
terraform validate

# Format Terraform files
terraform fmt -recursive
```

### Step 3: Review Infrastructure Plan

```bash
# Generate and review the execution plan
terraform plan -var-file=environments/production.tfvars -out=tfplan

# Review the plan carefully:
# - Resources to be created
# - Estimated costs
# - Security configurations
```

### Step 4: Deploy Infrastructure

```bash
# Apply the Terraform configuration
terraform apply tfplan

# This will create:
# - VPC with public, private, and database subnets
# - Security groups and network ACLs
# - RDS PostgreSQL instance
# - ECS cluster and task definitions
# - Application Load Balancer
# - S3 buckets for assets and backups
# - CloudFront distribution
# - AWS Backup vault and plans
# - IAM roles and policies

# Expected duration: 15-20 minutes
```

### Step 5: Build Docker Image

```bash
# Navigate back to project root
cd ..

# Build the Docker image
docker build -t book-repository-api:latest .

# Verify the image was created
docker images | grep book-repository-api
```

### Step 6: Push Image to ECR

```bash
# Get ECR repository URL
ECR_REPO=$(terraform -chdir=terraform output -raw ecr_repository_url)
echo "ECR Repository: $ECR_REPO"

# Authenticate Docker to ECR
aws ecr get-login-password --region us-east-1 | \
  docker login --username AWS --password-stdin $ECR_REPO

# Tag the image
docker tag book-repository-api:latest $ECR_REPO:latest
docker tag book-repository-api:latest $ECR_REPO:v1.0.0

# Push to ECR
docker push $ECR_REPO:latest
docker push $ECR_REPO:v1.0.0
```

### Step 7: Deploy to ECS

```bash
# Get ECS cluster and service names
ECS_CLUSTER=$(terraform -chdir=terraform output -raw ecs_cluster_name)
ECS_SERVICE=$(terraform -chdir=terraform output -raw ecs_service_name)

# Update the ECS service to deploy the new image
aws ecs update-service \
  --cluster $ECS_CLUSTER \
  --service $ECS_SERVICE \
  --force-new-deployment \
  --region us-east-1

# Monitor deployment status
aws ecs describe-services \
  --cluster $ECS_CLUSTER \
  --services $ECS_SERVICE \
  --region us-east-1 \
  --query 'services[0].deployments'
```

### Step 8: Verify Deployment

```bash
# Get the ALB URL
ALB_URL=$(terraform -chdir=terraform output -raw alb_url)
echo "Application URL: $ALB_URL"

# Test health endpoint
curl $ALB_URL/health

# Expected response:
# {"status":"Healthy","timestamp":"2026-03-09T22:40:15Z","service":"BookRepositoryApi"}

# Get CloudFront URL (for production traffic)
CF_URL=$(terraform -chdir=terraform output -raw cloudfront_url)
echo "CloudFront URL: $CF_URL"
```

## Post-Deployment

### Access the API

1. **Via Application Load Balancer** (Direct access)
   ```bash
   curl http://<alb-dns-name>/health
   ```

2. **Via CloudFront** (Recommended for production)
   ```bash
   curl https://<cloudfront-domain>/health
   ```

### API Endpoints

- `POST /api/auth/login` - User authentication
- `GET /api/books` - List all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create new book (Admin only)
- `PUT /api/books/{id}` - Update book (Admin only)
- `DELETE /api/books/{id}` - Delete book (Admin only)
- `GET /health` - Health check
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

### Test the API

```bash
# Login as admin
curl -X POST http://<alb-dns-name>/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}'

# Save the token
TOKEN="<token-from-response>"

# Get books
curl http://<alb-dns-name>/api/books \
  -H "Authorization: Bearer $TOKEN"
```

## Monitoring and Maintenance

### CloudWatch Logs

View application logs:
```bash
aws logs tail /ecs/book-repository-api-production --follow
```

### ECS Service Monitoring

```bash
# View running tasks
aws ecs list-tasks \
  --cluster book-repository-api-production-cluster \
  --service-name book-repository-api-production-service

# View service details
aws ecs describe-services \
  --cluster book-repository-api-production-cluster \
  --services book-repository-api-production-service
```

### Database Monitoring

```bash
# View RDS instance status
aws rds describe-db-instances \
  --db-instance-identifier production-bookrepository
```

### Scaling

The application auto-scales based on:
- CPU utilization > 70%
- Memory utilization > 80%
- Min: 2 tasks
- Max: 6 tasks

Manual scaling:
```bash
aws ecs update-service \
  --cluster book-repository-api-production-cluster \
  --service book-repository-api-production-service \
  --desired-count 4
```

## Troubleshooting

### ECS Tasks Not Starting

1. **Check CloudWatch Logs**
   ```bash
   aws logs tail /ecs/book-repository-api-production --follow
   ```

2. **Check Task Definition**
   ```bash
   aws ecs describe-task-definition \
     --task-definition book-repository-api-production
   ```

3. **Verify ECR Image Exists**
   ```bash
   aws ecr describe-images \
     --repository-name book-repository-api-production
   ```

### Database Connection Issues

1. **Verify Security Groups**
   - RDS security group allows traffic from ECS security group
   - Port 5432 is open

2. **Check Connection String**
   - Format: `Server=<endpoint>;Database=<name>;User Id=<user>;Password=<pass>;`

3. **Test from ECS Task**
   ```bash
   aws ecs execute-command \
     --cluster book-repository-api-production-cluster \
     --task <task-id> \
     --container book-repository-api \
     --interactive \
     --command "/bin/bash"
   ```

### ALB Health Checks Failing

1. **Verify Health Endpoint**
   - Endpoint: `/health`
   - Expected: HTTP 200
   - Response: JSON with status

2. **Check Security Groups**
   - ALB can reach ECS tasks on port 8080

3. **Review Target Group Health**
   ```bash
   aws elbv2 describe-target-health \
     --target-group-arn <target-group-arn>
   ```

## Updating the Application

### Rolling Update

```bash
# 1. Build new image with version tag
docker build -t book-repository-api:v1.1.0 .

# 2. Push to ECR
docker tag book-repository-api:v1.1.0 $ECR_REPO:v1.1.0
docker tag book-repository-api:v1.1.0 $ECR_REPO:latest
docker push $ECR_REPO:v1.1.0
docker push $ECR_REPO:latest

# 3. Deploy (ECS will automatically pull latest tag)
aws ecs update-service \
  --cluster book-repository-api-production-cluster \
  --service book-repository-api-production-service \
  --force-new-deployment
```

## Cleanup

To destroy all resources:

```bash
cd terraform
terraform destroy -var-file=environments/production.tfvars
```

**⚠️ Warning**: This will delete all resources including:
- Database and all data
- Backups
- Container images
- Logs

Ensure you have exported any critical data before destroying!

## Cost Estimation

Approximate monthly costs for production environment:

- **ECS Fargate**: ~$30-40 (2 tasks)
- **RDS db.t3.small**: ~$50-60
- **Application Load Balancer**: ~$25
- **NAT Gateway**: ~$65 (2 AZs)
- **Data Transfer**: Variable
- **CloudFront**: Variable
- **S3 Storage**: ~$5-10

**Total**: ~$175-200/month (excluding data transfer)

## Support

For issues or questions:
1. Check CloudWatch Logs
2. Review Terraform outputs
3. Consult AWS documentation
4. Review this guide

---

**Last Updated**: 2026-03-09
**Version**: 1.0.0
