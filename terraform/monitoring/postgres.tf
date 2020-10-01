
data "cloudfoundry_space" "postgres-1-monitor" {
  name     = var.postgres1["space"]
  org_name = var.paas_org_name
}

module "postgres-1" {
  // source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/postgres_prometheus_exporter"
     source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_prometheus_exporter?ref=master"

  install_space_id      = data.cloudfoundry_space.space.id
  monitor_space_id      = data.cloudfoundry_space.postgres-1-monitor.id
  name                  = "${var.environment}-${var.postgres1["name"]}"
  postgres-service-name = var.postgres1["service"]
  prometheus            = module.prometheus
}

data "cloudfoundry_space" "postgres-2-monitor" {
  name     = var.postgres2["space"]
  org_name = var.paas_org_name
}

module "postgres-2" {
  // source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/postgres_prometheus_exporter"
     source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_prometheus_exporter?ref=master"

  install_space_id      = data.cloudfoundry_space.space.id
  monitor_space_id      = data.cloudfoundry_space.postgres-2-monitor.id
  name                  = "${var.environment}-${var.postgres2["name"]}"
  postgres-service-name = var.postgres2["service"]
  prometheus            = module.prometheus
}



