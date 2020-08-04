
data "cloudfoundry_space" "redis-monitor" {
    name = var.redis["space"]
    org_name =  var.paas_org_name
}

module "redis" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/redis_prometheus_exporter"
//       source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/redis_prometheus_exporter?ref=master"

     monitor_space_id = data.cloudfoundry_space.redis-monitor.id
     install_space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.redis[ "name" ]}"
     redis-service-name = var.redis["service"] 
}


