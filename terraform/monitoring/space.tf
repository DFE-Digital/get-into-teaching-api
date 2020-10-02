
data "cloudfoundry_space" "space" {
  name     = var.delivery_space
  org_name = var.paas_org_name
}

data "cloudfoundry_space" "application_space" {
  name     = var.application_space
  org_name = var.paas_org_name
}
