locals {
  environment_map = { DATABASE_INSTANCE_NAME = cloudfoundry_service_instance.postgres2.name,
                      HANGFIRE_INSTANCE_NAME = cloudfoundry_service_instance.hangfire.name }
}

resource "cloudfoundry_app" "api_application" {
  name         = var.paas_api_application_name
  space        = data.cloudfoundry_space.space.id
  docker_image = var.paas_api_docker_image
  stopped      = var.application_stopped
  instances    = var.application_instances
  memory       = var.application_memory
  disk_quota   = var.application_disk
  strategy     = var.strategy

  routes {
    route = cloudfoundry_route.api_route_cloud.id
  }

  routes {
    route = cloudfoundry_route.api_route_internal.id
  }

  docker_credentials = {
    username = data.azurerm_key_vault_secret.docker_username.value
    password = data.azurerm_key_vault_secret.docker_password.value
  }

  service_binding {
    service_instance = cloudfoundry_service_instance.redis.id
  }
  service_binding {
    service_instance = cloudfoundry_service_instance.hangfire.id
  }
  service_binding {
    service_instance = cloudfoundry_service_instance.postgres2.id
  }
  dynamic "service_binding" {
    for_each = cloudfoundry_user_provided_service.logging
    content {
      service_instance = service_binding.value["id"]
    }
  }

  environment = merge(local.application_secrets, local.environment_map)

}




