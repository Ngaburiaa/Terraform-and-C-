# ECS Module Variables

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "project_name" {
  description = "Project name"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID"
  type        = string
}

variable "public_subnets" {
  description = "List of public subnet IDs"
  type        = list(string)
}

variable "private_subnets" {
  description = "List of private subnet IDs"
  type        = list(string)
}

variable "ecr_repository_url" {
  description = "ECR repository URL"
  type        = string
}

variable "container_port" {
  description = "Port exposed by the container"
  type        = number
  default     = 8080
}

variable "desired_count" {
  description = "Desired number of ECS tasks"
  type        = number
  default     = 2
}

variable "cpu" {
  description = "CPU units for ECS task"
  type        = string
  default     = "256"
}

variable "memory" {
  description = "Memory for ECS task in MB"
  type        = string
  default     = "512"
}

variable "health_check_path" {
  description = "Health check path for ALB"
  type        = string
  default     = "/health"
}

variable "acm_certificate_arn" {
  description = "ACM certificate ARN for ALB HTTPS listener (optional)"
  type        = string
  default     = ""
}

variable "additional_security_group_ids" {
  description = "Additional security groups to attach to ECS tasks"
  type        = list(string)
  default     = []
}

variable "environment_variables" {
  description = "Environment variables for the container"
  type        = map(string)
  default     = {}
}
