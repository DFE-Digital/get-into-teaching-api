data cloudfoundry_service postgres {
    name = "postgres"
}

resource "cloudfoundry_service_instance" "postgres" {
  name = var.paas_postgres_name
  space = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.postgres.service_plans["tiny-unencrypted-9_5"]
}
