ARM_TEMPLATE_TAG=1.1.6
RG_TAGS={"Product" : "Get into teaching website"}
SERVICE_SHORT=gitapi
SERVICE_NAME=getintoteachingapi
REGION=UK South

ifndef VERBOSE
.SILENT:
endif

MONITORING_SECRETS=MONITORING-KEYS
APPLICATION_SECRETS=API-KEYS
INFRASTRUCTURE_SECRETS=INFRA-KEYS

install-fetch-config:
	[ ! -f fetch_config.rb ]  \
	    && echo "Installing fetch_config.rb" \
	    && curl -s https://raw.githubusercontent.com/DFE-Digital/bat-platform-building-blocks/master/scripts/fetch_config/fetch_config.rb -o fetch_config.rb \
	    && chmod +x fetch_config.rb \
	    || true

bin/yaq:
	mkdir -p bin | curl -sL https://github.com/uk-devops/yaq/releases/download/v0.0.3/yaq_linux_amd64_v0.0.3.zip -o yaq.zip && unzip -o yaq.zip -d ./bin/ && rm yaq.zip

development_aks: test-cluster
	$(eval include global_config/development_aks.sh)

test_aks: test-cluster
	$(eval include global_config/test_aks.sh)

production_aks: production-cluster
	$(eval include global_config/production_aks.sh)

local_aks:
	$(eval export KEY_VAULT=s189t01-gitapi-dv-loc-kv)
	$(eval export AZ_SUBSCRIPTION=s189-teacher-services-cloud-test)

.PHONY: set-key-vault-names
set-key-vault-names:
	$(eval KEY_VAULT_APPLICATION_NAME=$(AZURE_RESOURCE_PREFIX)-$(SERVICE_SHORT)-$(CONFIG_SHORT)-app-kv)
	$(eval KEY_VAULT_INFRASTRUCTURE_NAME=$(AZURE_RESOURCE_PREFIX)-$(SERVICE_SHORT)-$(CONFIG_SHORT)-inf-kv)

.PHONY: ci
ci:	## Run in automation environment
	$(eval export DISABLE_PASSCODE=true)
	$(eval export AUTO_APPROVE=-auto-approve)

set-azure-account: ${environment}
	az account set -s ${AZ_SUBSCRIPTION}

print-app-secrets: bin/yaq set-azure-account set-key-vault-names
	yaq -i keyvault-secrets:${KEY_VAULT_APPLICATION_NAME} -d yaml

print-infra-secrets: bin/yaq set-azure-account set-key-vault-names
	yaq -i keyvault-secrets:${KEY_VAULT_INFRASTRUCTURE_NAME} -d yaml

setup-local-env: local_aks install-fetch-config set-azure-account set-key-vault-names
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/API-KEYS -f shell-env-var > GetIntoTeachingApi/env.local

edit-local-app-secrets: local_aks install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/API-KEYS -e -d azure-key-vault-secret:${KEY_VAULT}/API-KEYS -f yaml -c

.PHONY: vendor-modules
vendor-modules:
	rm -rf terraform/aks/vendor/modules/aks
	git -c advice.detachedHead=false clone --depth=1 --single-branch --branch ${TERRAFORM_MODULES_TAG} https://github.com/DFE-Digital/terraform-modules.git terraform/aks/vendor/modules/aks

terraform-init: vendor-modules set-azure-account
	terraform -chdir=terraform/aks init -upgrade -reconfigure \
		-backend-config=resource_group_name=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-rg \
		-backend-config=storage_account_name=${AZURE_RESOURCE_PREFIX}${SERVICE_SHORT}tfstate${CONFIG_SHORT}sa \
		-backend-config=key=${CONFIG}.tfstate

	$(if $(IMAGE_TAG), , $(error The IMAGE_TAG variable must be provided))
	$(eval export TF_VAR_app_docker_image=ghcr.io/dfe-digital/get-into-teaching-api:$(IMAGE_TAG))
	$(eval export TF_VAR_azure_resource_prefix=$(AZURE_RESOURCE_PREFIX))
	$(eval export TF_VAR_config_short=$(CONFIG_SHORT))
	$(eval export TF_VAR_service_short=$(SERVICE_SHORT))
	$(eval export TF_VAR_rg_name=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-rg)
	$(eval export TF_VAR_service_name=$(SERVICE_NAME))

terraform-plan: terraform-init
	terraform -chdir=terraform/aks plan -var-file "config/${CONFIG}.tfvars.json"

terraform-apply: terraform-init
	terraform -chdir=terraform/aks apply -var-file "config/${CONFIG}.tfvars.json" $(AUTO_APPROVE)

terraform-destroy: terraform-init
	terraform -chdir=terraform/aks destroy -var-file "config/${CONFIG}.tfvars.json" $(AUTO_APPROVE)

set-what-if:
	$(eval WHAT_IF=--what-if)

arm-deployment: set-azure-account set-key-vault-names
	az deployment sub create --name "resourcedeploy-tsc-$(shell date +%Y%m%d%H%M%S)" \
		-l "${REGION}" --template-uri "https://raw.githubusercontent.com/DFE-Digital/tra-shared-services/${ARM_TEMPLATE_TAG}/azure/resourcedeploy.json" \
		--parameters "resourceGroupName=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-rg" 'tags=${RG_TAGS}' \
			"tfStorageAccountName=${AZURE_RESOURCE_PREFIX}${SERVICE_SHORT}tfstate${CONFIG_SHORT}sa" "tfStorageContainerName=${SERVICE_SHORT}-tfstate" \
			keyVaultNames='("${KEY_VAULT_APPLICATION_NAME}", "${KEY_VAULT_INFRASTRUCTURE_NAME}")' \
			"enableKVPurgeProtection=${KEY_VAULT_PURGE_PROTECTION}" ${WHAT_IF}

deploy-arm-resources: arm-deployment

validate-arm-resources: set-what-if arm-deployment

test-cluster:
	$(eval CLUSTER_RESOURCE_GROUP_NAME=s189t01-tsc-ts-rg)
	$(eval CLUSTER_NAME=s189t01-tsc-test-aks)

production-cluster:
	$(eval CLUSTER_RESOURCE_GROUP_NAME=s189p01-tsc-pd-rg)
	$(eval CLUSTER_NAME=s189p01-tsc-production-aks)

get-cluster-credentials: set-azure-account
	az aks get-credentials --overwrite-existing -g ${CLUSTER_RESOURCE_GROUP_NAME} -n ${CLUSTER_NAME}
	kubelogin convert-kubeconfig -l $(if ${GITHUB_ACTIONS},spn,azurecli)
