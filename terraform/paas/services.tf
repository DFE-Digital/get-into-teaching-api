data cloudfoundry_service postgres {
    name = "postgres"
}

resource "cloudfoundry_service_instance" "postgres1" {
  name = var.paas_postgres_1_name
  space = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans["tiny-unencrypted-9_5"]
}

resource "cloudfoundry_service_instance" "postgres2" {
  name = var.paas_postgres_2_name
  space = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans["tiny-unencrypted-9_5"]
}
