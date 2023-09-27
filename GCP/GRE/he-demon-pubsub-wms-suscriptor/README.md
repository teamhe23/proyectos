# Descripcion del repositorio
Servicio que extrae del pubsub las interfaces output de WMS

# Consideraciones de Despliegue PRD/Prueba
|Ambiente | Cluster Rancher | Projecto Rancher | Namespace | Workload |
|-----------|-----------|-----------|-----------|-----------|
|Producción| production-ec | Integraciones WMS | he-ti-wms | he-ti-demon-pubsub-wms-suscriptor |
|Prueba| production-ec | Desarrollo | he-ti-wms-qa | he-ti-demon-pubsub-wms-suscriptor |

# Consideraciones de Desarrollo
Developers: para desarrollos y pruebas en nuevos requerimientos o incidencencias, considerar le siguiente:
1. Tu aplicacion debe considerar lectura de variables de entorno
2. Crear un archivo "appsettings.json" en la raíz del proyecto Facade para desarrollos/pruebas locales, con la siguiente estructura:

appsettings.json
```json
{
  "OracleDatabase": {
    "DataSource": "10.20.11.182:1521/HPEC01",
    "User": "SWMS",
    "Password": "xxxxx"
  },
  "PubSub": {
    "Proyecto": "he-wms-qa",
    "Suscripcion": "he-import-wms"
  },
  "Settings": {
    "TiempoEsperaSegundos": 10,
    "RutaGCPCredenciales": "CredencialesGCP.json"
  }
}
```

3. Crear un archivo "CredencialesGCP.json" con la cuenta de servicio para conectar a GCP.

CredencialesGCP.json
```json
{
  "type": "service_account",
  "project_id": "he-wms-qa",
  "private_key_id": "xxx",
  "private_key": "xxx",
  "client_email": "he-wms-user-qa@he-wms-qa.iam.gserviceaccount.com",
  "client_id": "104441213662147387575",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/he-wms-user-qa%40he-wms-qa.iam.gserviceaccount.com",
  "universe_domain": "googleapis.com"
}
```
