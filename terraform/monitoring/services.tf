data "cloudfoundry_service_instance" "redis" {
    name_or_id = var.redis_service
    space      = data.cloudfoundry_space.application_space.id
}
