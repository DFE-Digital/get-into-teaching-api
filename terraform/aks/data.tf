data "azurerm_resource_group" "resource_group" {
  name = var.rg_name
}

data "azurerm_subscription" "current" {}
