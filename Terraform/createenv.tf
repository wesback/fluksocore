#Main Resource Group and services
resource "azurerm_resource_group" "rgflukso" {
  name     = "${var.rgname}"
  location = "${var.location}"
}

resource "azurerm_eventhub_namespace" "ehnsflusko" {
  name                = "${var.ehnsname}"
  location            = "${azurerm_resource_group.rgflukso.location}"
  resource_group_name = "${azurerm_resource_group.rgflukso.name}"
  sku                 = "Standard"
  capacity            = 1

  tags {
    environment = "Production"
  }
}

resource "azurerm_eventhub" "ehflukso" {
  name                = "${var.ehname}"
  namespace_name      = "${azurerm_eventhub_namespace.ehnsflusko.name}"
  resource_group_name = "${azurerm_resource_group.rgflukso.name}"
  partition_count     = 2
  message_retention   = 7

  capture_description {
    enabled             = "true"
    encoding            = "Avro"
    interval_in_seconds = 300

    destination {
      name                = "EventHubArchive.AzureBlockBlob"
      archive_name_format = "{Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}"
      blob_container_name = "fluksooutput"
      storage_account_id  = "${azurerm_storage_account.fluksostorage.id}"
    }
  }
}

resource "azurerm_eventhub_authorization_rule" "ehfluksoauth" {
  name                = "flukso"
  namespace_name      = "${azurerm_eventhub_namespace.ehnsflusko.name}"
  eventhub_name       = "${azurerm_eventhub.ehflukso.name}"
  resource_group_name = "${azurerm_resource_group.rgflukso.name}"
  listen              = true
  send                = true
  manage              = false
}

resource "azurerm_sql_server" "sqlsrvr" {
  name                         = "${var.sqlsrvrname}"
  resource_group_name          = "${azurerm_resource_group.rgflukso.name}"
  location                     = "${azurerm_resource_group.rgflukso.location}"
  version                      = "12.0"
  administrator_login          = "${var.sqluser}"
  administrator_login_password = "${var.sqlpassword}"
}

resource "azurerm_sql_database" "sqldb" {
  name                = "fluksodb"
  resource_group_name = "${azurerm_resource_group.rgflukso.name}"
  location            = "${azurerm_sql_server.sqlsrvr.location}"
  server_name         = "${azurerm_sql_server.sqlsrvr.name}"
  edition             = "Basic"

  provisioner "local-exec" {
    command = "sqlcmd -S ${azurerm_sql_server.sqlsrvr.fully_qualified_domain_name} -d ${azurerm_sql_database.sqldb.name} -U ${var.sqluser} -P ${var.sqlpassword} -i './SQL/sensordata.sql'"
  }
}

resource "azurerm_storage_account" "fluksostorage" {
  name                     = "${var.storagename}"
  resource_group_name      = "${azurerm_resource_group.rgflukso.name}"
  location                 = "${azurerm_resource_group.rgflukso.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_container_registry" "acr" {
  name                = "${var.acrname}"
  resource_group_name = "${azurerm_resource_group.rgflukso.name}"
  location            = "${azurerm_resource_group.rgflukso.location}"
  admin_enabled       = true
  sku                 = "Classic"
  storage_account_id  = "${azurerm_storage_account.fluksostorage.id}"
}

output "acrlogin" {
  value = "${azurerm_container_registry.acr.admin_username}"
}

output "acrpassword" {
  value = "${azurerm_container_registry.acr.admin_password}"
}

output "storageconnection" {
  value = "${azurerm_storage_account.fluksostorage.primary_connection_string}"
}

output "eventhubconnectionstring" {
  value = "${azurerm_eventhub_authorization_rule.ehfluksoauth.primary_connection_string}"
}
