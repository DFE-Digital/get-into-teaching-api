terraform {
  backend "azurerm" {
    resource_group_name  = "s146t01-rg-tfstate"
    storage_account_name = "s146t01sgtfstate"
    container_name       = "pass-tfstate"
    key                  = "api.test.terraform.tfstate"
    access_key           = var.tf_state_key 
  }
}
