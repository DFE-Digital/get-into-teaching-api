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
	echo ""
	echo "Parameters:"
	echo "All commands take the parameter -e environment=development|review|test|production"
	echo ""
	echo "Examples:"
	echo ""
	echo "To edit the Application secrets for Development"
	echo "        make  -e environment=development edit-app-secrets"
	echo ""
	echo "To print the Monitoring secrets for Production"
	echo "        make  -e environment=production print-monitoring-secrets"

MONITORING_SECRETS=MONITORING-KEYS
APPLICATION_SECRETS=API-KEYS

.PHONY: development
development:
	$(eval KEY_VAULT=s146d01-kv)
	$(eval AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

.PHONY: review
review:
	$(eval KEY_VAULT=s146d01-kv)
	$(eval AZ_SUBSCRIPTION=s146-getintoteachingwebsite-development)

.PHONY: test
test:
	$(eval KEY_VAULT=s146t01-kv)
	$(eval AZ_SUBSCRIPTION=s146-getintoteachingwebsite-test)

.PHONY: production
production:
	$(eval KEY_VAULT=s146p01-kv)
	$(eval AZ_SUBSCRIPTION=s146-getintoteachingwebsite-production)

set-azure-account: ${environment}
	echo "Logging on to ${AZ_SUBSCRIPTION}"
	az account set -s ${AZ_SUBSCRIPTION}

install-fetch-config: 
	echo "Installing fetch_config.rb"
	mkdir -p bin
	curl -s https://raw.githubusercontent.com/DFE-Digital/bat-platform-building-blocks/master/scripts/fetch_config/fetch_config.rb -o bin/fetch_config.rb 
	chmod +x bin/fetch_config.rb 

edit-app-secrets: install-fetch-config set-azure-account
	bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS} -e -d azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS} -f yaml -c

print-app-secrets: install-fetch-config set-azure-account 
	echo "bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS}  -f yaml"
	bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${APPLICATION_SECRETS}  -f yaml

edit-monitoring-secrets: install-fetch-config set-azure-account
	bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS} -e -d azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS} -f yaml -c

print-monitoring-secrets: install-fetch-config set-azure-account 
	echo "bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS}  -f yaml"
	bin/fetch_config.rb -s azure-key-vault-secret:${KEY_VAULT}/${MONITORING_SECRETS}  -f yaml
