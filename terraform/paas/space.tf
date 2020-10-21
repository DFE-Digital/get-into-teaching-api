data "cloudfoundry_space" "space" {
  name     = var.paas_space
  org_name = var.paas_org_name
}
data "cloudfoundry_space" "monitor" {
  name     = var.monitor_space
  org_name = var.paas_org_name
}
