provider "azurerm" {
  features {}

  skip_provider_registration = true
}

provider "kubernetes" {
  host                   = module.cluster_data.kubernetes_host
  cluster_ca_certificate = module.cluster_data.kubernetes_cluster_ca_certificate

  exec {
    api_version = "client.authentication.k8s.io/v1beta1"
    command     = "kubelogin"
    args        = module.cluster_data.kubelogin_args
  }

}

provider "statuscake" {
  api_token = module.infrastructure_secrets.map.STATUSCAKE-API-TOKEN
}
