# Backup Module Variables

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "project_name" {
  description = "Project name"
  type        = string
}

variable "rds_arn" {
  description = "ARN of the RDS instance to backup"
  type        = string
}

variable "backup_retention" {
  description = "Number of days to retain backups"
  type        = number
  default     = 7
}
