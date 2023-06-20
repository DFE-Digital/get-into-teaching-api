TERRAFILE_VERSION=0.8
ARM_TEMPLATE_TAG=1.1.0
RG_TAGS={"Product" : "Get into teaching"}
SERVICE_SHORT=gitapi
SERVICE_NAME=getintoteachingapi

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

install-terrafile: ## Install terrafile to manage terraform modules
	[ ! -f bin/terrafile ] \
		&& curl -sL https://github.com/coretech/terrafile/releases/download/v${TERRAFILE_VERSION}/terrafile_${TERRAFILE_VERSION}_$$(uname)_x86_64.tar.gz \
		| tar xz -C ./bin terrafile \
		|| true

.PHONY: development
development:
	$(eval export KEY_VAULT=s146d01-kv)
	$(eval export AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

development_aks:
	$(eval include global_config/development.sh)

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

set-azure-account: ${environment}
	az account set -s ${AZ_SUBSCRIPTION}

clean:
	[ ! -f fetch_config.rb ]  \
	    rm -f fetch_config.rb \
	    || true

install-fetch-config:
	[ ! -f fetch_config.rb ]  \
	    && echo "Installing fetch_config.rb" \
	    && curl -s https://raw.githubusercontent.com/DFE-Digital/bat-platform-building-blocks/master/scripts/fetch_config/fetch_config.rb -o fetch_config.rb \
	    && chmod +x fetch_config.rb \
	    || true

edit-app-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS} -e -d azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS} -f yaml -c

print-app-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS}  -f yaml

edit-monitoring-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS} -e -d azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS} -f yaml -c

print-monitoring-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS}  -f yaml

edit-infrastructure-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${INFRASTRUCTURE_SECRETS} -e -d azure-key-vault-secret:${KEY_VAULT}/${INFRASTRUCTURE_SECRETS} -f yaml -c

print-infrastructure-secrets: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${INFRASTRUCTURE_SECRETS}  -f yaml

setup-local-env: install-fetch-config set-azure-account
	./fetch_config.rb -s azure-key-vault-secret:s146d01-local2-kv/${APPLICATION_SECRETS} -f shell-env-var > GetIntoTeachingApi/env.local

terraform-init: install-terrafile set-azure-account
	./bin/terrafile -p terraform/aks/vendor/modules -f terraform/aks/config/$(CONFIG)_Terrafile
	terraform -chdir=terraform/aks init -upgrade -reconfigure \
		-backend-config=resource_group_name=${RESOURCE_GROUP_NAME} \
		-backend-config=storage_account_name=${STORAGE_ACCOUNT_NAME} \
		-backend-config=key=${CONFIG}.tfstate

	$(eval export TF_VAR_azure_resource_prefix=$(AZURE_RESOURCE_PREFIX))
	$(eval export TF_VAR_config_short=$(CONFIG_SHORT))
	$(eval export TF_VAR_service_short=$(SERVICE_SHORT))
	$(eval export TF_VAR_rg_name=$(RESOURCE_GROUP_NAME))
	$(eval export TF_VAR_service_name=$(SERVICE_NAME))

terraform-plan: terraform-init
	terraform -chdir=terraform/aks plan -var-file "config/${CONFIG}.tfvars.json"

terraform-apply: terraform-init
	terraform -chdir=terraform/aks apply -var-file "config/${CONFIG}.tfvars.json"

set-what-if:
	$(eval WHAT_IF=--what-if)

arm-deployment: set-azure-account
	az deployment sub create --name "resourcedeploy-tsc-$(shell date +%Y%m%d%H%M%S)" \
		-l "UK South" --template-uri "https://raw.githubusercontent.com/DFE-Digital/tra-shared-services/${ARM_TEMPLATE_TAG}/azure/resourcedeploy.json" \
		--parameters "resourceGroupName=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-rg" 'tags=${RG_TAGS}' \
			"tfStorageAccountName=${AZURE_RESOURCE_PREFIX}${SERVICE_SHORT}tfstate${CONFIG_SHORT}sa" "tfStorageContainerName=${SERVICE_SHORT}-tfstate" \
			"keyVaultName=${AZURE_RESOURCE_PREFIX}-${SERVICE_SHORT}-${CONFIG_SHORT}-kv" ${WHAT_IF}

deploy-arm-resources: arm-deployment

validate-arm-resources: set-what-if arm-deployment
