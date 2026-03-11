# Outputs for BookRepositoryApi Terraform Configuration

output "vpc_id" {
  description = "VPC ID"
  value       = module.vpc.vpc_id
}

output "alb_dns_name" {
  description = "DNS name of the Application Load Balancer"
  value       = module.ecs.alb_dns_name
}

output "alb_url" {
  description = "URL of the Application Load Balancer"
  value       = "http://${module.ecs.alb_dns_name}"
}

output "cloudfront_domain_name" {
  description = "CloudFront distribution domain name"
  value       = module.cloudfront.distribution_domain_name
}

output "cloudfront_url" {
  description = "CloudFront distribution URL"
  value       = "https://${module.cloudfront.distribution_domain_name}"
}

output "ecr_repository_url" {
  description = "ECR repository URL"
  value       = aws_ecr_repository.app.repository_url
}

output "rds_endpoint" {
  description = "RDS instance endpoint"
  value       = module.rds.endpoint
  sensitive   = true
}

output "s3_assets_bucket" {
  description = "S3 bucket for static assets"
  value       = module.s3.assets_bucket_id
}

output "s3_backups_bucket" {
  description = "S3 bucket for backups"
  value       = module.s3.backups_bucket_id
}

output "ecs_cluster_name" {
  description = "ECS cluster name"
  value       = module.ecs.cluster_name
}

output "ecs_service_name" {
  description = "ECS service name"
  value       = module.ecs.service_name
}
