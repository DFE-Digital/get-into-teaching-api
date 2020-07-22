paas_space="get-into-teaching-monitoring"
api_application_name="get-into-teaching-api"
environment="test"

variable "redis" {
    type = map
    default = {
        "name" = "get-into-teaching"
        "space" = "get-into-teaching-test"
        "service" = "get-into-teaching-adviser-test-red-svc"
    }
}

variable "postgres1" {
    type = map
    default = {
        "name" = "get-into-teaching-1"
        "space" = "get-into-teaching-test"
        "service" = "get-into-teaching-api-test-pg1-svc"
    }
}

variable "postgres2" {
    type = map
    default = {
        "name" = "get-into-teaching-2"
        "space" = "get-into-teaching-test"
        "service" = "get-into-teaching-api-test-pg2-svc"
    }
}
