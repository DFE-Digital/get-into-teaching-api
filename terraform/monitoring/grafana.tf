module "grafana" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/grafana"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=devops/get-into-teaching/add-postgres"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.grafana[ "name" ]}"
     admin_password = var.grafana[ "password" ]
     prometheus_endpoint = module.prometheus.endpoint
}
