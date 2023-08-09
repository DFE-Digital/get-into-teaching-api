TERRAFILE_VERSION=0.8
ARM_TEMPLATE_TAG=1.1.6
RG_TAGS={"Product" : "Get into teaching"}
SERVICE_SHORT=gitapi
SERVICE_NAME=getintoteachingapi
REGION=UK South

ifndef VERBOSE
.SILENT:
endif

help:
	echo "Secrets:"
	echo "  This makefile gives the user the ability to safely display and edit azure secrets which are used by this project. "
	echo ""
	echo "Commands:"
	echo "  edit-app-secrets  - Edit Application specific Secrets."
	echo "  print-app-secrets - Display Application specific Secrets."
	echo "  edit-monitoring-secrets  - Edit Monitoring specific Secrets."
	echo "  print-monitoring-secrets - Display Monitoring specific Secrets."
	echo "  edit-infrastructure-secrets  - Edit Infrastructure specific Secrets."
	echo "  print-infrastructure-secrets - Display Infrastructure specific Secrets."
	echo ""
	echo "Parameters:"
	echo "All commands take the parameter development|review|test|production"
	echo ""
	echo "Examples:"
	echo ""
	echo "To edit the Application secrets for Development"
	echo "        make  development edit-app-secrets"
	echo ""
	echo "To print the Monitoring secrets for Production"
	echo "        make  production print-monitoring-secrets"

MONITORING_SECRETS=MONITORING-KEYS
APPLICATION_SECRETS=API-KEYS
INFRASTRUCTURE_SECRETS=INFRA-KEYS

bin/terrafile: ## Install terrafile to manage terraform modules
	mkdir -p bin | curl -sL https://github.com/coretech/terrafile/releases/download/v${TERRAFILE_VERSION}/terrafile_${TERRAFILE_VERSION}_$$(uname)_x86_64.tar.gz \
		| tar xz -C ./bin terrafile

bin/yaq:
	mkdir -p bin | curl -sL https://github.com/uk-devops/yaq/releases/download/v0.0.3/yaq_linux_amd64_v0.0.3.zip -o yaq.zip && unzip -o yaq.zip -d ./bin/ && rm yaq.zip

.PHONY: development
development:
	$(eval export KEY_VAULT=s146d01-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

development_aks:
	$(eval include global_config/development_aks.sh)

test_aks:
	$(eval include global_config/test_aks.sh)

.PHONY: set-key-vault-names
set-key-vault-names:
	$(eval KEY_VAULT_APPLICATION_NAME=$(AZURE_RESOURCE_PREFIX)-$(SERVICE_SHORT)-$(CONFIG_SHORT)-app-kv)
	$(eval KEY_VAULT_INFRASTRUCTURE_NAME=$(AZURE_RESOURCE_PREFIX)-$(SERVICE_SHORT)-$(CONFIG_SHORT)-inf-kv)

.PHONY: local
local:
	$(eval export KEY_VAULT=s146d01-local2-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

.PHONY: review
review:
	$(eval export KEY_VAULT=s146d01-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

.PHONY: test
test:
	$(eval export KEY_VAULT=s146t01-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-test)

.PHONY: production
production:
	$(eval export KEY_VAULT=s146p01-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-production)

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

setup-local-env: bin/yaq set-azure-account
	yaq -i keyvault-secret-map:s146d01-local2-kv/${APPLICATION_SECRETS}  -d yaml > GetIntoTeachingApi/env.local

setup-aks-local-env: bin/yaq set-azure-account set-key-vault-names
	yaq -i keyvault-secrets:${KEY_VAULT_APPLICATION_NAME} -d yaml -d yaml > GetIntoTeachingApi/env.local


terraform-init: bin/terrafile set-azure-account
	./bin/terrafile -p terraform/aks/vendor/modules -f terraform/aks/config/$(CONFIG)_Terrafile
	terraform -chdir=terraform/aks init -upgrade -reconfigure \
		-backend-config=resource_group_name=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-rg \
		-backend-config=storage_account_name=${AZURE_RESOURCE_PREFIX}${SERVICE_SHORT}tfstate${CONFIG_SHORT}sa \
		-backend-config=key=${CONFIG}.tfstate

	$(if $(IMAGE_TAG), , $(eval export IMAGE_TAG=sha-194cbc9))
	$(eval export TF_VAR_paas_app_docker_image=ghcr.io/dfe-digital/get-into-teaching-api:$(IMAGE_TAG))
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
