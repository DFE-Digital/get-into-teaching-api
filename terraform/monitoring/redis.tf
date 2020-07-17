
module "redis" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/redis_exporter"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/redis_exporter?ref=devops/get-into-teaching/add-postgres"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/redis_exporter?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.redis[ "name" ]}"
     redis-service-name = var.redis["service"] 
}


