# CloudFront Module Variables

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "project_name" {
  description = "Project name"
  type        = string
}

variable "alb_dns_name" {
  description = "DNS name of the Application Load Balancer"
  type        = string
}

variable "alb_use_https" {
  description = "Whether CloudFront should connect to ALB via HTTPS"
  type        = bool
  default     = false
}

variable "s3_bucket_domain" {
  description = "Domain name of the S3 bucket"
  type        = string
}

variable "s3_bucket_id" {
  description = "ID of the S3 bucket"
  type        = string
}
