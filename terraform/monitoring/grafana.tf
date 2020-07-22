locals {
   dashboard_directory  = "${path.module}/${var.grafana["dashboard_directory"]}"
   datasource_directory = "${path.module}/${var.grafana["datasource_directory"]}"
   configuration_file   = "${path.module}/${var.grafana["configuration_file"]}"

   additional_variable_map = {
    elastic_user  =  var.elasticsearch_user
    elastic_pass  =  var.elasticsearch_password
    google_client_id = var.google_client_id 
    google_client_secret = var.google_client_secret
   }
}

module "grafana" {
//     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/grafana"
       source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/grafana?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.grafana[ "name" ]}"
     admin_password = var.grafana[ "password" ]
     dashboard_directory = local.dashboard_directory
     datasource_directory = local.datasource_directory
     configuration_file = local.configuration_file
     prometheus_endpoint = module.prometheus.endpoint
     additional_variable_map = local.additional_variable_map

}

