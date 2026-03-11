# BookRepositoryApi - Terraform Infrastructure

This directory contains Terraform configuration for deploying the BookRepositoryApi to AWS production environment.

## Architecture Overview

The infrastructure includes:

- **VPC**: Multi-AZ VPC with public, private, and database subnets
- **ECS Fargate**: Container orchestration for running the API
- **RDS PostgreSQL**: Managed database service
- **Application Load Balancer**: Traffic distribution and SSL termination
- **CloudFront**: CDN for API and static assets
- **S3**: Storage for static assets and backups
- **AWS Backup**: Automated backup solution for RDS
- **Auto Scaling**: CPU and memory-based scaling for ECS tasks

## Prerequisites

1. **AWS CLI** configured with appropriate credentials
2. **Terraform** >= 1.0 installed
3. **Docker** for building container images
4. **AWS Account** with appropriate permissions

## Directory Structure

```
terraform/
├── main.tf                 # Main Terraform configuration
├── variables.tf            # Variable definitions
├── outputs.tf              # Output definitions
├── environments/
│   └── production.tfvars   # Production environment variables
└── modules/
    ├── vpc/                # VPC networking module
    ├── ecs/                # ECS container orchestration module
    ├── rds/                # RDS database module
    ├── s3/                 # S3 storage module
    ├── cloudfront/         # CloudFront CDN module
    └── backup/             # AWS Backup module
```

## Backend Configuration

The Terraform state is stored in S3 with DynamoDB for state locking:

- **S3 Bucket**: `itrack-terraform-state-prod-471744311346`
- **State Key**: `book-repository-api/production/terraform.tfstate`
- **DynamoDB Table**: `terraform-state-lock-production`
- **Region**: `us-east-1`

## Setup Instructions

### 1. Initialize Terraform

```bash
cd terraform
terraform init
```

### 2. Set Required Secrets

Export sensitive variables as environment variables:

```bash
export TF_VAR_db_username="your_db_username"
export TF_VAR_db_password="your_secure_db_password"
export TF_VAR_jwt_secret_key="your_jwt_secret_key_min_32_chars"
```

### 3. Review the Deployment Plan

```bash
terraform plan -var-file=environments/production.tfvars
```

### 4. Apply the Configuration

```bash
terraform apply -var-file=environments/production.tfvars
```

### 5. Build and Push Docker Image

After infrastructure is created, build and push the Docker image:

```bash
# Get ECR login credentials
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <ECR_REPOSITORY_URL>

# Build the Docker image
docker build -t book-repository-api:latest .

# Tag the image
docker tag book-repository-api:latest <ECR_REPOSITORY_URL>:latest

# Push to ECR
docker push <ECR_REPOSITORY_URL>:latest
```

### 6. Deploy to ECS

Update the ECS service to use the new image:

```bash
aws ecs update-service \
  --cluster book-repository-api-production-cluster \
  --service book-repository-api-production-service \
  --force-new-deployment \
  --region us-east-1
```

## Configuration Variables

### Required Variables (Must be set via environment or tfvars)

- `db_username`: Database master username
- `db_password`: Database master password
- `jwt_secret_key`: JWT secret key for authentication (minimum 32 characters)

### Optional Variables (With defaults in production.tfvars)

- `aws_region`: AWS region (default: us-east-1)
- `environment`: Environment name (default: production)
- `vpc_cidr`: VPC CIDR block (default: 10.0.0.0/16)
- `db_instance_class`: RDS instance type (default: db.t3.small)
- `ecs_desired_count`: Number of ECS tasks (default: 2)
- `ecs_cpu`: ECS task CPU units (default: 512)
- `ecs_memory`: ECS task memory in MB (default: 1024)

## Outputs

After successful deployment, Terraform will output:

- `alb_url`: Application Load Balancer URL
- `cloudfront_url`: CloudFront distribution URL
- `ecr_repository_url`: ECR repository URL for pushing images
- `ecs_cluster_name`: ECS cluster name
- `s3_assets_bucket`: S3 bucket for static assets

## Disaster Recovery

### Backup Strategy

- **Daily backups**: Retained for 30 days
- **Weekly backups**: Retained for 120 days (30 days × 4)
- **Backup window**: 2:00 AM UTC (daily), 3:00 AM UTC (weekly)

### Restore from Backup

To restore the database from a backup:

```bash
aws backup start-restore-job \
  --recovery-point-arn <RECOVERY_POINT_ARN> \
  --metadata '{"DBInstanceIdentifier":"restored-db-instance"}' \
  --iam-role-arn <BACKUP_ROLE_ARN> \
  --region us-east-1
```

## Scaling

### Manual Scaling

To manually scale ECS tasks:

```bash
aws ecs update-service \
  --cluster book-repository-api-production-cluster \
  --service book-repository-api-production-service \
  --desired-count 4 \
  --region us-east-1
```

### Auto Scaling

Auto scaling is configured based on:
- **CPU**: Scales when average CPU utilization exceeds 70%
- **Memory**: Scales when average memory utilization exceeds 80%
- **Min tasks**: 2
- **Max tasks**: 6

## Monitoring

### CloudWatch Logs

Application logs are available in CloudWatch Logs:
- Log Group: `/ecs/book-repository-api-production`
- Retention: 30 days

### Metrics

Key metrics to monitor:
- ECS CPU and Memory utilization
- ALB request count and latency
- RDS CPU, storage, and connections
- CloudFront request count and error rates

## Security

### Network Security

- Public subnets: ALB only
- Private subnets: ECS tasks
- Database subnets: RDS instances (isolated)
- Security groups: Principle of least privilege

### Data Security

- S3 encryption: AES-256
- RDS encryption: At rest
- Secrets: Use AWS Secrets Manager or environment variables
- IAM roles: Least privilege access

## Cost Optimization

- **NAT Gateway**: Using one per AZ (consider single NAT for non-production)
- **RDS**: Multi-AZ enabled for production
- **S3**: Lifecycle policies for backups
- **CloudFront**: PriceClass_All for production

## Troubleshooting

### Common Issues

1. **ECS tasks not starting**
   - Check CloudWatch logs for container errors
   - Verify security groups allow traffic
   - Ensure ECR image exists

2. **Database connection issues**
   - Verify RDS security group allows ECS task traffic
   - Check connection string format
   - Ensure database credentials are correct

3. **ALB health checks failing**
   - Verify health check path returns 200
   - Check application logs
   - Ensure container port matches ALB target group

## Cleanup

To destroy all resources:

```bash
terraform destroy -var-file=environments/production.tfvars
```

**Warning**: This will delete all resources including databases and backups. Ensure you have exported any critical data first.

## Additional Resources

- [AWS ECS Best Practices](https://docs.aws.amazon.com/AmazonECS/latest/bestpracticesguide/)
- [Terraform AWS Provider Documentation](https://registry.terraform.io/providers/hashicorp/aws/latest/docs)
- [AWS Well-Architected Framework](https://aws.amazon.com/architecture/well-architected/)
