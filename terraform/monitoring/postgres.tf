
module "postgres-1" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/postgres_exporter"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_exporter?ref=devops/get-into-teaching/add-postgres"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_exporter?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.postgres1[ "name" ]}"
     postgres-service-name = var.postgres1["service"] 
     prometheus = module.prometheus
}

module "postgres-2" {
     source = "/Users/stevenfawcett/DFE/bat-platform-building-blocks/terraform/modules/postgres_exporter"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_exporter?ref=devops/get-into-teaching/add-postgres"
//        source = "git::https://github.com/DFE-Digital/bat-platform-building-blocks.git//terraform/modules/postgres_exporter?ref=master"

     space_id = data.cloudfoundry_space.space.id
     name = "${var.environment}-${var.postgres2[ "name" ]}"
     postgres-service-name = var.postgres2["service"] 
     prometheus = module.prometheus
}



