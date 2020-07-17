locals {
   dashboard_directory = "${path.module}/${var.grafana["dashboard_directory"]}"
   datasource_directory = "${path.module}/${var.grafana["datasource_directory"]}"
}

module "grafana" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/grafana"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=devops/get-into-teaching/add-postgres"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.grafana[ "name" ]}"
     admin_password = var.grafana[ "password" ]
     dashboard_directory = local.dashboard_directory
     datasource_directory = local.datasource_directory
     prometheus_endpoint = module.prometheus.endpoint
}

