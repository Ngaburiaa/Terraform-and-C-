# Main Terraform configuration for BookRepositoryApi AWS Production Environment

terraform {
  required_version = ">= 1.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  backend "s3" {
    bucket         = "itrack-terraform-state-prod-471744311346"
    key            = "book-repository-api/production/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "terraform-state-lock-production"
    encrypt        = true
  }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Environment = var.environment
      Project     = "BookRepositoryApi"
      ManagedBy   = "Terraform"
    }
  }
}

# Shared security group used to allow app tasks to reach RDS without
# creating an ECS<->RDS module dependency cycle.
resource "aws_security_group" "app_shared" {
  name        = "${var.project_name}-${var.environment}-app-shared-sg"
  description = "Shared app security group for ECS tasks and RDS ingress"
  vpc_id      = module.vpc.vpc_id

  egress {
    description = "Allow all outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name        = "${var.project_name}-${var.environment}-app-shared-sg"
    Environment = var.environment
  }
}

# VPC Module
module "vpc" {
  source = "./modules/vpc"

  environment        = var.environment
  vpc_cidr           = var.vpc_cidr
  availability_zones = var.availability_zones
  project_name       = var.project_name
}

# RDS Module for Database
module "rds" {
  source = "./modules/rds"

  environment           = var.environment
  vpc_id                = module.vpc.vpc_id
  database_subnets      = module.vpc.database_subnets
  app_security_group_id = aws_security_group.app_shared.id
  db_name               = var.db_name
  db_username           = var.db_username
  db_password           = var.db_password
  instance_class        = var.db_instance_class
  allocated_storage     = var.db_allocated_storage
  backup_retention      = var.backup_retention_days
}

# ECR Repository for Docker Images
resource "aws_ecr_repository" "app" {
  name                 = "${var.project_name}-${var.environment}"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  encryption_configuration {
    encryption_type = "AES256"
  }
}

# ECS Module for Container Orchestration
module "ecs" {
  source = "./modules/ecs"

  environment         = var.environment
  project_name        = var.project_name
  vpc_id              = module.vpc.vpc_id
  public_subnets      = module.vpc.public_subnets
  private_subnets     = module.vpc.private_subnets
  ecr_repository_url  = aws_ecr_repository.app.repository_url
  container_port      = var.container_port
  desired_count       = var.ecs_desired_count
  cpu                 = var.ecs_cpu
  memory              = var.ecs_memory
  health_check_path   = var.health_check_path
  acm_certificate_arn = var.acm_certificate_arn
  additional_security_group_ids = [
    aws_security_group.app_shared.id
  ]

  # Environment variables for the application
  environment_variables = {
    ASPNETCORE_ENVIRONMENT               = "Production"
    ConnectionStrings__DefaultConnection = "Server=${module.rds.endpoint};Database=${var.db_name};User Id=${var.db_username};Password=${var.db_password};"
    Jwt__Key                             = var.jwt_secret_key
    Jwt__Issuer                          = var.jwt_issuer
    Jwt__Audience                        = var.jwt_audience
  }
}

# S3 Module for Static Assets and Backups
module "s3" {
  source = "./modules/s3"

  environment  = var.environment
  project_name = var.project_name
}

# CloudFront Module for CDN
module "cloudfront" {
  source = "./modules/cloudfront"

  environment      = var.environment
  project_name     = var.project_name
  alb_dns_name     = module.ecs.alb_dns_name
  alb_use_https    = var.acm_certificate_arn != ""
  s3_bucket_domain = module.s3.assets_bucket_domain
  s3_bucket_id     = module.s3.assets_bucket_id
}

# AWS Backup Module
module "backup" {
  source = "./modules/backup"

  environment      = var.environment
  project_name     = var.project_name
  rds_arn          = module.rds.db_instance_arn
  backup_retention = var.backup_retention_days
}
