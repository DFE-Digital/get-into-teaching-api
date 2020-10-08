resource "cloudfoundry_app" "api_application" {
    name =  var.paas_api_application_name
    space = data.cloudfoundry_space.space.id
    docker_image = var.paas_api_docker_image
    stopped = var.application_stopped
    instances = var.application_instances
    memory = var.application_memory
    disk_quota = var.application_disk
    strategy = var.strategy

    routes {
        route = cloudfoundry_route.api_route_cloud.id
    }

    routes {
        route = cloudfoundry_route.api_route_internal.id
    }

    service_binding  { 
            service_instance = cloudfoundry_service_instance.hangfire.id
    } 
    service_binding  { 
            service_instance = cloudfoundry_service_instance.postgres2.id
    } 
    dynamic "service_binding" {
      for_each = cloudfoundry_user_provided_service.logging
      content {
        service_instance = service_binding.value["id"]
      }
    }
    environment = {
         CRM_CLIENT_ID          = var.CRM_CLIENT_ID
         CRM_CLIENT_SECRET      = var.CRM_CLIENT_SECRET
         CRM_SERVICE_URL        = var.CRM_SERVICE_URL
         CRM_TENANT_ID          = var.CRM_TENANT_ID
         NOTIFY_API_KEY         = var.NOTIFY_API_KEY
         TOTP_SECRET_KEY        = var.TOTP_SECRET_KEY
         SHARED_SECRET          = var.SHARED_SECRET
         PEN_TEST_SHARED_SECRET = var.PEN_TEST_SHARED_SECRET
         Sentry__Dsn            = var.SENTRY_DSN
         GOOGLE_API_KEY         = var.GOOGLE_API_KEY
         ASPNETCORE_ENVIRONMENT = var.ASPNETCORE_ENVIRONMENT
         DATABASE_INSTANCE_NAME = cloudfoundry_service_instance.postgres2.name
         HANGFIRE_INSTANCE_NAME = cloudfoundry_service_instance.hangfire.name
    }    
}




