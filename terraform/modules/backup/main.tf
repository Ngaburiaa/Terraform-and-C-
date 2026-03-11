# AWS Backup Module for BookRepositoryApi

# Backup Vault
resource "aws_backup_vault" "main" {
  name = "${var.project_name}-${var.environment}-backup-vault"

  tags = {
    Name        = "${var.project_name}-${var.environment}-backup-vault"
    Environment = var.environment
  }
}

# IAM Role for AWS Backup
resource "aws_iam_role" "backup" {
  name = "${var.project_name}-${var.environment}-backup-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Principal = {
          Service = "backup.amazonaws.com"
        }
      }
    ]
  })

  tags = {
    Name        = "${var.project_name}-${var.environment}-backup-role"
    Environment = var.environment
  }
}

# Attach AWS managed backup policy
resource "aws_iam_role_policy_attachment" "backup" {
  role       = aws_iam_role.backup.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSBackupServiceRolePolicyForBackup"
}

resource "aws_iam_role_policy_attachment" "backup_restore" {
  role       = aws_iam_role.backup.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSBackupServiceRolePolicyForRestores"
}

# Backup Plan
resource "aws_backup_plan" "main" {
  name = "${var.project_name}-${var.environment}-backup-plan"

  rule {
    rule_name         = "daily_backup"
    target_vault_name = aws_backup_vault.main.name
    schedule          = "cron(0 2 * * ? *)" # Daily at 2 AM UTC

    lifecycle {
      delete_after = var.backup_retention
    }

    recovery_point_tags = {
      Environment = var.environment
      BackupType  = "Daily"
    }
  }

  rule {
    rule_name         = "weekly_backup"
    target_vault_name = aws_backup_vault.main.name
    schedule          = "cron(0 3 ? * SUN *)" # Weekly on Sunday at 3 AM UTC

    lifecycle {
      delete_after = var.backup_retention * 4
    }

    recovery_point_tags = {
      Environment = var.environment
      BackupType  = "Weekly"
    }
  }

  tags = {
    Name        = "${var.project_name}-${var.environment}-backup-plan"
    Environment = var.environment
  }
}

# Backup Selection for RDS
resource "aws_backup_selection" "rds" {
  name         = "${var.project_name}-${var.environment}-rds-backup-selection"
  plan_id      = aws_backup_plan.main.id
  iam_role_arn = aws_iam_role.backup.arn

  resources = [
    var.rds_arn
  ]

  selection_tag {
    type  = "STRINGEQUALS"
    key   = "Environment"
    value = var.environment
  }
}
