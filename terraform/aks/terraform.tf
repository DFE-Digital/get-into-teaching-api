terraform {
  required_version = "=1.4.6"

  backend "azurerm" {
    container_name = "gitapi-tfstate"
  }

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.116.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "2.32.0"
    }
    statuscake = {
      source  = "StatusCakeDev/statuscake"
      version = "2.2.2"
    }
  }
}
